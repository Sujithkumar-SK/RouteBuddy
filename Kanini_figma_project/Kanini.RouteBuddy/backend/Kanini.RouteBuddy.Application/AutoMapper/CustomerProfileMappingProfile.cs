using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper
{
    public class CustomerProfileMappingProfile : Profile
    {
        public CustomerProfileMappingProfile()
        {
            CreateMap<Customer, CustomerProfileDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.ProfilePictureBase64, opt => opt.MapFrom(src => 
                    src.ProfilePicture != null ? Convert.ToBase64String(src.ProfilePicture) : null));

            CreateMap<UpdateCustomerProfileDto, Customer>()
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}