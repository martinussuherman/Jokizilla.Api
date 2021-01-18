using AutoMapper;
using Jokizilla.Api.Misc;
using Jokizilla.Models.Models;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Jokizilla.Api
{
    /// <summary>
    /// Represents the startup process for the application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Application startup.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Application configuration.`
        /// </summary>
        /// <value>Application configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures services for the application.
        /// </summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApiSecurityOptions>(Configuration.GetSection(ApiSecurityOptions.OptionsName));
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<FileOperation>();
            services.AddControllers();
            services.AddApiVersioning(options => options.ReportApiVersions = true);

            ConfigureDatabase(services);
            ApiSecurityOptions apiSecurityOptions = ReadApiSecurityOptions();
            ConfigureOData(services);
            ConfigureSwagger(services, apiSecurityOptions);
            ConfigureAuth(services, apiSecurityOptions);
        }

        /// <summary>
        /// Configures the application using the provided builder, hosting environment, and logging factory.
        /// </summary>
        /// <param name="app">The current application builder.</param>
        /// <param name="modelBuilder">The <see cref="VersionedODataModelBuilder">model builder</see> used to create OData entity data models (EDMs).</param>
        /// <param name="provider">The API version descriptor provider used to enumerate defined API versions.</param>
        public void Configure(
            IApplicationBuilder app,
            VersionedODataModelBuilder modelBuilder,
            IApiVersionDescriptionProvider provider)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.Count();
                    endpoints.MapVersionedODataRoute("odata", "api", modelBuilder);
                });

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.OAuthClientId(Configuration.GetValue<string>("ClientId"));
                    options.OAuthAppName("Jokizilla Api Swagger");
                    options.OAuthUsePkce();

                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            MariaDbServerVersion version = new(new Version(10, 5, 5));

            services.AddDbContextPool<AppDbContext>(
                options => options.UseMySql(
                    Configuration.GetConnectionString("AppConnection"),
                    version,
                    sqlOptions => sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null)),
                16);
        }
        private ApiSecurityOptions ReadApiSecurityOptions()
        {
            IConfigurationSection options = Configuration.GetSection(ApiSecurityOptions.OptionsName);

            return new ApiSecurityOptions
            {
                Audience = options.GetValue<string>(nameof(ApiSecurityOptions.Audience)),
                Authority = options.GetValue<string>(nameof(ApiSecurityOptions.Authority))
            };
        }
        private static void ConfigureOData(IServiceCollection services)
        {
            services.AddOData().EnableApiVersioning();
            services.AddODataApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
        }
        private static void ConfigureSwagger(
            IServiceCollection services,
            ApiSecurityOptions apiSecurityOptions)
        {
            OpenApiOAuthFlow authCodeFlow = new()
            {
                AuthorizationUrl = new Uri($"{apiSecurityOptions.Authority}/connect/authorize"),
                TokenUrl = new Uri($"{apiSecurityOptions.Authority}/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { apiSecurityOptions.Audience, "Api access" }
                }
            };

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(
                options =>
                {
                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();
                    options.OperationFilter<AuthorizeCheckOperationFilter>();

                    // integrate xml comments
                    // options.IncludeXmlComments(XmlCommentsFilePath);

                    options.AddSecurityDefinition(
                        ApiInfo.SchemeOauth2,
                        new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                AuthorizationCode = authCodeFlow
                            }
                        });

                    options.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = ApiInfo.SchemeOauth2
                                    }
                                },
                                new[]
                                {
                                    apiSecurityOptions.Audience
                                }
                            }
                        });
                });
        }
        private static void ConfigureAuth(
            IServiceCollection services,
            ApiSecurityOptions apiSecurityOptions)
        {
            // https://identityserver4.readthedocs.io/en/latest/topics/apis.html
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // base-address of your identityserver
                    options.Authority = apiSecurityOptions.Authority;

                    // if you are using API resources, you can specify the name here
                    options.Audience = apiSecurityOptions.Audience;
                });
        }

        private static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}
