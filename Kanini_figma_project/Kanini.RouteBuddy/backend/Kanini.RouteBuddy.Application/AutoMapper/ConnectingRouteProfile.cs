using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class ConnectingRouteProfile : Profile
{
    public ConnectingRouteProfile()
    {
        CreateMap<BusSchedule, ConnectingRouteSegmentDto>()
            .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
            .ForMember(dest => dest.BusName, opt => opt.MapFrom(src => src.Bus.BusName))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Route.Source))
            .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Route.Destination))
            .ForMember(dest => dest.TravelDate, opt => opt.MapFrom(src => src.TravelDate))
            .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime))
            .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => src.ArrivalTime))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Route.BasePrice))
            .ForMember(
                dest => dest.VendorName,
                opt => opt.MapFrom(src => src.Bus.Vendor.AgencyName)
            )
            .ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.AvailableSeats));
    }
}
