using AutoMapper;
using Jokizilla.Models.Models;
using Jokizilla.Models.ViewModels;

namespace Jokizilla.Api.Misc
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AdditionalService, AdditionalServiceViewDto>();
            CreateMap<AdditionalServiceUpdateDto, AdditionalService>();
            CreateMap<AdditionalService, AdditionalServiceUpdateDto>();

            CreateMap<PriceType, PriceTypeViewDto>();
            CreateMap<PriceTypeUpdateDto, PriceType>();
            CreateMap<PriceType, PriceTypeUpdateDto>();

            CreateMap<Service, ServiceViewDto>();
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
