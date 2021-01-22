using System.Linq;
using AutoMapper;
using Jokizilla.Models.Models;
using Jokizilla.Models.ViewModels;

namespace Jokizilla.Api.Misc
{
    /// <summary>
    /// AutoMapper mapping profile.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Creates AutoMapper mapping profile.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<AdditionalService, AdditionalServiceViewDto>();
            CreateMap<AdditionalServiceUpdateDto, AdditionalService>();
            CreateMap<AdditionalService, AdditionalServiceUpdateDto>();

            CreateMap<Applicant, ApplicantViewDto>();
            CreateMap<ApplicantUpdateDto, Applicant>();
            CreateMap<Applicant, ApplicantUpdateDto>();

            CreateMap<ApplicantStatus, ApplicantStatusViewDto>();
            CreateMap<ApplicantStatusUpdateDto, ApplicantStatus>();
            CreateMap<ApplicantStatus, ApplicantStatusUpdateDto>();

            CreateMap<Country, CountryViewDto>();
            CreateMap<CountryUpdateDto, Country>();
            CreateMap<Country, CountryUpdateDto>();

            CreateMap<PriceType, PriceTypeViewDto>();
            CreateMap<PriceTypeUpdateDto, PriceType>();
            CreateMap<PriceType, PriceTypeUpdateDto>();

            CreateMap<ReferralSource, ReferralSourceViewDto>();
            CreateMap<ReferralSourceUpdateDto, ReferralSource>();
            CreateMap<ReferralSource, ReferralSourceUpdateDto>();

            CreateMap<Service, ServiceViewDto>()
                .ForMember(
                    dto => dto.AdditionalServices,
                    opt => opt.MapFrom(
                        src => src.ServiceTagAdditionalServices.Select(e => e.AdditionalService)));
            CreateMap<ServiceUpdateDto, Service>();
            CreateMap<Service, ServiceUpdateDto>();

            CreateMap<Urgency, UrgencyViewDto>();
            CreateMap<UrgencyUpdateDto, Urgency>();
            CreateMap<Urgency, UrgencyUpdateDto>();

            CreateMap<WorkLevel, WorkLevelViewDto>();
            CreateMap<WorkLevelUpdateDto, WorkLevel>();
            CreateMap<WorkLevel, WorkLevelUpdateDto>();
        }
    }
}
