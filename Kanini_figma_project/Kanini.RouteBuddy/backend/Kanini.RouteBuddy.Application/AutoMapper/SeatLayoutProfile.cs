using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class SeatLayoutProfile : Profile
{
    public SeatLayoutProfile()
    {
        // Entity to Response DTO mappings
        CreateMap<SeatLayoutTemplate, SeatLayoutTemplateResponseDto>()
            .ForMember(dest => dest.SeatDetails, opt => opt.MapFrom(src => src.SeatLayoutDetails));
        CreateMap<SeatLayoutTemplate, SeatLayoutTemplateListDto>();
        CreateMap<SeatLayoutDetail, SeatLayoutDetailResponseDto>();

        // Request DTO to Entity mappings
        CreateMap<CreateSeatLayoutTemplateRequestDto, SeatLayoutTemplate>()
            .ForMember(dest => dest.SeatLayoutTemplateId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "Admin"))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.SeatLayoutDetails, opt => opt.Ignore())
            .ForMember(dest => dest.Buses, opt => opt.Ignore());

        CreateMap<UpdateSeatLayoutTemplateRequestDto, SeatLayoutTemplate>()
            .ForMember(dest => dest.SeatLayoutTemplateId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => "Admin"))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.SeatLayoutDetails, opt => opt.Ignore())
            .ForMember(dest => dest.Buses, opt => opt.Ignore());

        CreateMap<SeatLayoutDetailRequestDto, SeatLayoutDetail>()
            .ForMember(dest => dest.SeatLayoutDetailId, opt => opt.Ignore())
            .ForMember(dest => dest.SeatLayoutTemplateId, opt => opt.Ignore())
            .ForMember(dest => dest.SeatLayoutTemplate, opt => opt.Ignore())
            .ForMember(dest => dest.IsBooked, opt => opt.Ignore())
            .ForMember(dest => dest.BasePrice, opt => opt.Ignore())
            .ForMember(dest => dest.BusTypeForPricing, opt => opt.Ignore())
            .ForMember(dest => dest.AmenitiesForPricing, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "Admin"))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());
    }
}