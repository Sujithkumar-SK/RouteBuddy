using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class BusProfile : Profile
{
    public BusProfile()
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

        CreateMap<SeatLayoutDetail, SeatDto>()
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => !src.IsBooked))
            .ForMember(dest => dest.IsBooked, opt => opt.MapFrom(src => src.IsBooked))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => 0));

        CreateMap<List<SeatLayoutDetail>, SeatLayoutResponseDto>()
            .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Bus, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.TravelDate, opt => opt.Ignore());

        // Map controller parameters to request DTO
        CreateMap<(int ScheduleId, DateTime TravelDate), SeatLayoutRequestDto>()
            .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
            .ForMember(dest => dest.TravelDate, opt => opt.MapFrom(src => src.TravelDate));

        // Map Booking entity to BookingResponseDto
        CreateMap<Booking, BookingResponseDto>()
            .ForMember(dest => dest.PNR, opt => opt.MapFrom(src => src.PNRNo))
            .ForMember(dest => dest.BusName, opt => opt.Ignore())
            .ForMember(dest => dest.Route, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.TravelDate, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.BoardingPoint, opt => opt.Ignore())
            .ForMember(dest => dest.DroppingPoint, opt => opt.Ignore())
            .ForMember(dest => dest.BoardingTime, opt => opt.Ignore())
            .ForMember(dest => dest.DroppingTime, opt => opt.Ignore())
            .ForMember(dest => dest.ReservationExpiryTime, opt => opt.Ignore());
    }
}
