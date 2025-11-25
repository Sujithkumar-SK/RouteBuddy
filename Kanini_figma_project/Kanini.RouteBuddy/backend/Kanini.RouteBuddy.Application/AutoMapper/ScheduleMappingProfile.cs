using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Schedule;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class ScheduleMappingProfile : Profile
{
    public ScheduleMappingProfile()
    {
        CreateMap<CreateScheduleDto, BusSchedule>();
        CreateMap<CreateBulkScheduleDto, BusSchedule>();
        CreateMap<UpdateScheduleDto, BusSchedule>();
        CreateMap<BusSchedule, ScheduleResponseDto>()
            .ForMember(dest => dest.BusName, opt => opt.MapFrom(src => src.Bus != null ? src.Bus.BusName : ""))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Route != null ? src.Route.Source : ""))
            .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Route != null ? src.Route.Destination : ""));
    }
}