using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class BusMappingProfile : Profile
{
    public BusMappingProfile()
    {
        CreateMap<CreateBusDto, Bus>()
            .ForSourceMember(src => src.RegistrationCertificate, opt => opt.DoNotValidate());
        CreateMap<UpdateBusDto, Bus>()
            .ForMember(dest => dest.RegistrationPath, opt => opt.Ignore())
            .ForMember(dest => dest.RegistrationNo, opt => opt.Ignore())
            .ForMember(dest => dest.VendorId, opt => opt.Ignore())
            .ForMember(dest => dest.BusId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.SeatLayoutTemplateId, opt => opt.Ignore());
        CreateMap<Bus, BusResponseDto>()
            .ForMember(dest => dest.VendorName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn));
    }
}