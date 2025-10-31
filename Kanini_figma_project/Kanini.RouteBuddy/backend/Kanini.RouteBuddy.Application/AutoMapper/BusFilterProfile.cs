using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class BusFilterProfile : Profile
{
    public BusFilterProfile()
    {
        CreateMap<BusSchedule, BusSearchResponseDto>()
            .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
            .ForMember(dest => dest.BusId, opt => opt.MapFrom(src => src.Bus.BusId))
            .ForMember(dest => dest.BusName, opt => opt.MapFrom(src => src.Bus.BusName))
            .ForMember(dest => dest.BusType, opt => opt.MapFrom(src => src.Bus.BusType))
            .ForMember(dest => dest.TotalSeats, opt => opt.MapFrom(src => src.Bus.TotalSeats))
            .ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.AvailableSeats))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Route.Source))
            .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Route.Destination))
            .ForMember(dest => dest.TravelDate, opt => opt.MapFrom(src => src.TravelDate))
            .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime))
            .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => src.ArrivalTime))
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.Route.BasePrice))
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.Bus.Amenities))
            .ForMember(
                dest => dest.VendorName,
                opt => opt.MapFrom(src => src.Bus.Vendor.AgencyName)
            );
    }
}
