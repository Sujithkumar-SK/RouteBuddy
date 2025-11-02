using AutoMapper;
using Kanini.RouteBuddy.Data.Repositories.Email;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class SmartEmailProfile : Profile
{
    public SmartEmailProfile()
    {
        CreateMap<ConnectingBookingEmailData, ConnectingBookingEmailData>()
            .ForMember(dest => dest.Segments, opt => opt.MapFrom(src => src.Segments.OrderBy(s => s.SegmentOrder)));

        CreateMap<ConnectingSegmentEmailData, ConnectingSegmentEmailData>()
            .ForMember(dest => dest.SeatNumbers, opt => opt.MapFrom(src => src.SeatNumbers.OrderBy(s => s)))
            .ForMember(dest => dest.Passengers, opt => opt.MapFrom(src => src.Passengers.OrderBy(p => p.SeatNumber)));

        CreateMap<ConnectingPassengerEmailData, ConnectingPassengerEmailData>();
    }
}