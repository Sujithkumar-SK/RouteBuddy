using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.BusPhoto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class BusPhotoMappingProfile : Profile
{
    public BusPhotoMappingProfile()
    {
        CreateMap<BusPhoto, BusPhotoDto>()
            .ForMember(dest => dest.BusPhotoId, opt => opt.MapFrom(src => src.BusPhotoId))
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath))
            .ForMember(dest => dest.Caption, opt => opt.MapFrom(src => src.Caption))
            .ForMember(dest => dest.BusId, opt => opt.MapFrom(src => src.BusId));

        CreateMap<CreateBusPhotoDto, BusPhoto>()
            .ForMember(dest => dest.BusId, opt => opt.MapFrom(src => src.BusId))
            .ForMember(dest => dest.Caption, opt => opt.MapFrom(src => src.Caption))
            .ForMember(dest => dest.BusPhotoId, opt => opt.Ignore())
            .ForMember(dest => dest.ImagePath, opt => opt.Ignore())
            .ForMember(dest => dest.Bus, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());
    }
}