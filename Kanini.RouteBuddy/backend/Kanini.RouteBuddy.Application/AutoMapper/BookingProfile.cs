using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
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
    }
}
