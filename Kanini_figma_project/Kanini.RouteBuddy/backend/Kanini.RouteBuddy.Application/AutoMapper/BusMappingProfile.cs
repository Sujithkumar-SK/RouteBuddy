using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class BusMappingProfile : Profile
{
    public BusMappingProfile()
    {
        CreateMap<CreateBusDto, Bus>();
        CreateMap<UpdateBusDto, Bus>();
        CreateMap<Bus, BusResponseDto>()
            .ForMember(dest => dest.VendorName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn));
    }
}