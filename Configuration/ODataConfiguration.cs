using Jokizilla.Models.Models;
using Jokizilla.Models.ViewModels;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Jokizilla.Api.Configuration
{
    /// <summary>
    /// Represents the model configuration.
    /// </summary>
    public class ODataConfiguration : IModelConfiguration
    {
        /// <inheritdoc />
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
        {
            builder.ComplexType<AdditionalServiceUpdateDto>();
            EntityTypeConfiguration<AdditionalServiceViewDto> additionalService = builder
                .EntitySet<AdditionalServiceViewDto>(nameof(AdditionalService))
                .EntityType;

            additionalService.HasKey(p => p.Id);
            additionalService
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            builder.ComplexType<ApplicantUpdateDto>();
            EntityTypeConfiguration<ApplicantViewDto> applicant = builder
                .EntitySet<ApplicantViewDto>(nameof(Applicant))
                .EntityType;

            applicant.HasKey(p => p.Id);
            applicant
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            builder.ComplexType<ApplicantStatusUpdateDto>();
            EntityTypeConfiguration<ApplicantStatusViewDto> applicantStatus = builder
                .EntitySet<ApplicantStatusViewDto>(nameof(ApplicantStatus))
                .EntityType;

            applicantStatus.HasKey(p => p.Id);
            applicantStatus
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            builder.ComplexType<CountryUpdateDto>();
            EntityTypeConfiguration<CountryViewDto> country = builder
                .EntitySet<CountryViewDto>(nameof(Country))
                .EntityType;

            country.HasKey(p => p.Id);
            country
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            builder.ComplexType<PriceTypeUpdateDto>();
            EntityTypeConfiguration<PriceTypeViewDto> priceType = builder
                .EntitySet<PriceTypeViewDto>(nameof(PriceType))
                .EntityType;

            priceType.HasKey(p => p.Id);
            priceType
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            builder.ComplexType<ReferralSourceUpdateDto>();
            EntityTypeConfiguration<ReferralSourceViewDto> referralSource = builder
                .EntitySet<ReferralSourceViewDto>(nameof(ReferralSource))
                .EntityType;

            referralSource.HasKey(p => p.Id);
            referralSource
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            builder.ComplexType<ServiceUpdateDto>();
            EntityTypeConfiguration<ServiceViewDto> service = builder
                .EntitySet<ServiceViewDto>(nameof(Service))
                .EntityType;

            service.HasKey(p => p.Id);
            service
                .Expand()
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            builder.ComplexType<UrgencyUpdateDto>();
            EntityTypeConfiguration<UrgencyViewDto> urgency = builder
                .EntitySet<UrgencyViewDto>(nameof(Urgency))
                .EntityType;

            urgency.HasKey(p => p.Id);
            urgency
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            builder.ComplexType<WorkLevelUpdateDto>();
            EntityTypeConfiguration<WorkLevelViewDto> workLevel = builder
                .EntitySet<WorkLevelViewDto>(nameof(WorkLevel))
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
