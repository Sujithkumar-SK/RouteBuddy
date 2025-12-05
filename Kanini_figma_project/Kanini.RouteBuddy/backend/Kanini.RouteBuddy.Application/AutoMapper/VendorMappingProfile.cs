using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Application.Dto.Admin;
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
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn));
        
        CreateMap<Vendor, AdminVendorDTO>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User != null ? src.User.Phone : string.Empty))
            .ForMember(dest => dest.TotalBuses, opt => opt.MapFrom(src => src.FleetSize));
        

    }
}