using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.BusPhoto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class BusPhotoMappingProfile : Profile
{
    public BusPhotoMappingProfile()
    {
        CreateMap<CreateBusPhotoDto, BusPhoto>()
            .ForSourceMember(src => src.Photo, opt => opt.DoNotValidate());
        CreateMap<UpdateBusPhotoDto, BusPhoto>();
        CreateMap<BusPhoto, BusPhotoDto>();
    }
}