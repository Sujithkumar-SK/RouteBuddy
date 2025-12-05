using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Auth;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        // Registration DTO to RegistrationData
        CreateMap<RegisterWithOtpRequestDto, RegistrationDataDto>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // RegistrationData to User Entity
        CreateMap<RegistrationDataDto, User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.IsEmailVerified, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
            .ForMember(dest => dest.FailedLoginAttempts, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Vendor, opt => opt.Ignore());

        // CompleteCustomerProfileDto to Customer Entity
        CreateMap<CompleteCustomerProfileDto, Customer>()
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (Gender)src.Gender))
            .ForMember(dest => dest.ProfilePicture, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());

        // CompleteVendorProfileDto to Vendor Entity
        CreateMap<CompleteVendorProfileDto, Vendor>()
            .ForMember(dest => dest.VendorId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Buses, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore());

        // User Entity to LoginResponse DTO
        CreateMap<User, LoginResponseDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.AccessTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore());

        // User Entity to RegistrationResponse DTO
        CreateMap<User, RegistrationResponseDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.RequiresVendorProfile, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore());
    }
}
