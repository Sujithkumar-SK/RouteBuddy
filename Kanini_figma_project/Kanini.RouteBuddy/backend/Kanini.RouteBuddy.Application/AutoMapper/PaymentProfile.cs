using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentResponseDto>()
            .ForMember(dest => dest.PNR, opt => opt.MapFrom(src => src.Booking.PNRNo));
    }
}