using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class BusMappingProfile : Profile
{
    public BusMappingProfile()
    {
        CreateMap<CreateBusDto, Bus>();
        CreateMap<UpdateBusDto, Bus>();
        CreateMap<Bus, BusResponseDto>()
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor != null ? src.Vendor.AgencyName : string.Empty))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy ?? string.Empty))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            .ForMember(dest => dest.SeatLayoutTemplateId, opt => opt.MapFrom(src => src.SeatLayoutTemplateId));
    }
}