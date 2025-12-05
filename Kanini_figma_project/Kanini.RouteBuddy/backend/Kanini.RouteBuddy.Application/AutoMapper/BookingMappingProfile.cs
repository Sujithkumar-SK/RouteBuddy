using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<Booking, AdminBookingDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => 
                    src.Customer != null ? (src.Customer.FirstName ?? "") + " " + (src.Customer.LastName ?? "") : ""))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => 
                    src.Customer != null && src.Customer.User != null ? src.Customer.User.Email : ""))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => 
                    src.Customer != null && src.Customer.User != null ? src.Customer.User.Phone : ""))
                .ForMember(dest => dest.BusName, opt => opt.MapFrom(src => 
                    src.Segments != null && src.Segments.Any() && src.Segments.First().Schedule != null && src.Segments.First().Schedule.Bus != null 
                        ? src.Segments.First().Schedule.Bus.BusName ?? "" : ""))
                .ForMember(dest => dest.Route, opt => opt.MapFrom(src => 
                    src.Segments != null && src.Segments.Any() && src.Segments.First().Schedule != null && src.Segments.First().Schedule.Route != null 
                        ? (src.Segments.First().Schedule.Route.Source ?? "") + " - " + (src.Segments.First().Schedule.Route.Destination ?? "") : ""))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => 
                    src.Payment != null ? src.Payment.PaymentStatus : Domain.Enums.PaymentStatus.Pending));
        }
    }
}