using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class VendorMappingProfile : Profile
{
    public VendorMappingProfile()
    {
        CreateMap<VendorRegistrationDto, Vendor>();
        CreateMap<UpdateVendorProfileDto, Vendor>();
        CreateMap<Vendor, VendorResponseDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone));
    }
}