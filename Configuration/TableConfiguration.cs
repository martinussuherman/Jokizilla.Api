using Jokizilla.Models.Models;
using Jokizilla.Models.ViewModels;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Jokizilla.Api.Configuration
{
    /// <summary>
    /// Represents the model configuration for Price Type.
    /// </summary>
    public class TableConfiguration : IModelConfiguration
    {
        /// <inheritdoc />
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
        {
            ComplexTypeConfiguration<AdditionalServiceUpdateDto> additionalServiceUpdate = builder
                .ComplexType<AdditionalServiceUpdateDto>();
            EntityTypeConfiguration<AdditionalServiceViewDto> additionalService = builder
                .EntitySet<AdditionalServiceViewDto>(nameof(AppDbContext.AdditionalServices))
                .EntityType;

            additionalService.HasKey(p => p.Id);
            additionalService
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            ComplexTypeConfiguration<PriceTypeUpdateDto> priceTypeUpdate = builder
                .ComplexType<PriceTypeUpdateDto>();
            EntityTypeConfiguration<PriceTypeViewDto> priceType = builder
                .EntitySet<PriceTypeViewDto>(nameof(AppDbContext.PriceTypes))
                .EntityType;

            priceType.HasKey(p => p.Id);
            priceType
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            ComplexTypeConfiguration<ServiceUpdateDto> serviceUpdate = builder
                .ComplexType<ServiceUpdateDto>();
            EntityTypeConfiguration<ServiceViewDto> service = builder
                .EntitySet<ServiceViewDto>(nameof(AppDbContext.Services))
                .EntityType;

            service.HasKey(p => p.Id);
            service
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            ComplexTypeConfiguration<UrgencyUpdateDto> urgencyUpdate = builder
                .ComplexType<UrgencyUpdateDto>();
            EntityTypeConfiguration<UrgencyViewDto> urgency = builder
                .EntitySet<UrgencyViewDto>(nameof(AppDbContext.Urgencies))
                .EntityType;

            urgency.HasKey(p => p.Id);
            urgency
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            ComplexTypeConfiguration<WorkLevelUpdateDto> workLevelUpdate = builder
                .ComplexType<WorkLevelUpdateDto>();
            EntityTypeConfiguration<WorkLevelViewDto> workLevel = builder
                .EntitySet<WorkLevelViewDto>(nameof(AppDbContext.WorkLevels))
                .EntityType;

            workLevel.HasKey(p => p.Id);
            workLevel
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();
        }
    }
}
