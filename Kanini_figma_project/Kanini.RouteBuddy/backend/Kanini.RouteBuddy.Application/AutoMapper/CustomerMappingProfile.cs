using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Application.Dto.Review;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, AdminCustomerDTO>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src =>
                        $"{src.FirstName} {(string.IsNullOrEmpty(src.MiddleName) ? "" : src.MiddleName + " ")}{src.LastName}".Trim()))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src =>
                        DateTime.Today.Year - src.DateOfBirth.Year -
                        (DateTime.Today.DayOfYear < src.DateOfBirth.DayOfYear ? 1 : 0)))
                .ForMember(dest => dest.TotalBookings,
                    opt => opt.MapFrom(src => src.Bookings.Count))
                .ForMember(dest => dest.TotalReviews,
                    opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.LastBookingDate,
                    opt => opt.MapFrom(src => src.Bookings
                        .OrderByDescending(b => b.TravelDate)
                        .Select(b => (DateTime?)b.TravelDate)
                        .FirstOrDefault()))
                .ForMember(dest => dest.RecentBookings,
                    opt => opt.MapFrom(src => src.Bookings))
                .ForMember(dest => dest.RecentReviews,
                    opt => opt.MapFrom(src => src.Reviews));

            CreateMap<Booking, BookingSummaryDTO>();

            CreateMap<Review, ReviewSummaryDTO>()
                .ForMember(dest => dest.BusName,
                    opt => opt.MapFrom(src => src.Bus != null ? src.Bus.BusName : null));
        }
    }
}