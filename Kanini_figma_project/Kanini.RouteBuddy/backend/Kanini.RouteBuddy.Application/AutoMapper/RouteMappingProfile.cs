using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class RouteMappingProfile : Profile
{
    public RouteMappingProfile()
    {
        CreateMap<CreateRouteDto, Domain.Entities.Route>()
            .ForMember(dest => dest.RouteStops, opt => opt.Ignore());
        CreateMap<CreateRouteStopDto, RouteStop>();
        CreateMap<UpdateRouteDto, Domain.Entities.Route>();
        CreateMap<Domain.Entities.Route, RouteResponseDto>()
            .ForMember(dest => dest.RouteStops, opt => opt.MapFrom(src => src.RouteStops));
        CreateMap<RouteStop, RouteStopDto>()
            .ForMember(dest => dest.StopName, opt => opt.MapFrom(src => src.Stop.Name))
            .ForMember(dest => dest.Landmark, opt => opt.MapFrom(src => src.Stop.Landmark));
        CreateMap<UpdateRouteStopDto, RouteStop>();
    }
}