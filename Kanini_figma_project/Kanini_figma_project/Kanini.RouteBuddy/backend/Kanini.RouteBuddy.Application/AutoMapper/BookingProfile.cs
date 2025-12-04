using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<PassengerDto, BookedSeat>()
            .ForMember(dest => dest.PassengerName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.PassengerAge, opt => opt.MapFrom(src => src.Age))
            .ForMember(dest => dest.PassengerGender, opt => opt.MapFrom(src => src.Gender));

        CreateMap<Booking, BookingResponseDto>()
            .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
            .ForMember(dest => dest.PNR, opt => opt.MapFrom(src => src.PNRNo))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.BookedAt, opt => opt.MapFrom(src => src.BookedAt))
            .ForMember(dest => dest.TravelDate, opt => opt.MapFrom(src => src.TravelDate));

        CreateMap<Booking, AdminBookingDTO>()
            .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
            .ForMember(dest => dest.PNRNo, opt => opt.MapFrom(src => src.PNRNo))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => $"{src.Customer.FirstName} {src.Customer.LastName}"))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.User.Email))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer.User.Phone))
            .ForMember(dest => dest.TotalSeats, opt => opt.MapFrom(src => src.TotalSeats))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.TravelDate, opt => opt.MapFrom(src => src.TravelDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.BookedAt, opt => opt.MapFrom(src => src.BookedAt))
            .ForMember(dest => dest.BusName, opt => opt.MapFrom(src => src.Segments.FirstOrDefault().Schedule.Bus.BusName))
            .ForMember(dest => dest.Route, opt => opt.MapFrom(src => $"{src.Segments.FirstOrDefault().Schedule.Route.Source} - {src.Segments.FirstOrDefault().Schedule.Route.Destination}"))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Payment.PaymentStatus));
    }
}
