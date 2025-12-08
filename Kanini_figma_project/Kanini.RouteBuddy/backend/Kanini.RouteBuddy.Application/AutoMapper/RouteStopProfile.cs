using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class RouteStopProfile : Profile
{
    public RouteStopProfile()
    {
        CreateMap<RouteStop, RouteStopDto>()
            .ForMember(dest => dest.StopName, opt => opt.MapFrom(src => src.Stop!.Name))
            .ForMember(dest => dest.Landmark, opt => opt.MapFrom(src => src.Stop!.Landmark));
    }
}
