using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Data.Repositories.Vendor;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class VendorApprovalMappingProfile : Profile
{
    public VendorApprovalMappingProfile()
    {
        CreateMap<VendorApprovalData, VendorApprovalDTO>()
            .ForMember(dest => dest.VendorId, opt => opt.MapFrom(src => src.Vendor.VendorId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Vendor.UserId))
            .ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Vendor.AgencyName))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Vendor.OwnerName))
            .ForMember(dest => dest.BusinessLicenseNumber, opt => opt.MapFrom(src => src.Vendor.BusinessLicenseNumber))
            .ForMember(dest => dest.OfficeAddress, opt => opt.MapFrom(src => src.Vendor.OfficeAddress))
            .ForMember(dest => dest.FleetSize, opt => opt.MapFrom(src => src.Vendor.FleetSize))
            .ForMember(dest => dest.TaxRegistrationNumber, opt => opt.MapFrom(src => src.Vendor.TaxRegistrationNumber))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Vendor.Status))
            .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => src.Vendor.Status.ToString()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Vendor.IsActive))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.Vendor.CreatedOn))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Vendor.User.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Vendor.User.Phone))
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents));
    }
}