using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class ConnectingBookingProfile : Profile
{
    public ConnectingBookingProfile()
    {
        CreateMap<Booking, ConnectingBookingResponseDto>()
            .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
            .ForMember(dest => dest.PNR, opt => opt.MapFrom(src => src.PNRNo))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.TravelDate, opt => opt.MapFrom(src => src.TravelDate))
            .ForMember(
                dest => dest.ReservationExpiryTime,
                opt => opt.MapFrom(src => src.CreatedOn.AddMinutes(10))
            )
            .ForMember(dest => dest.RouteDescription, opt => opt.Ignore())
            .ForMember(dest => dest.Segments, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore());
    }
}
