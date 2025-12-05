using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Schedule;
using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Data.Repositories.Route;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class VendorScheduleMappingProfile : Profile
{
    public VendorScheduleMappingProfile()
    {
        // Route Search Mappings
        CreateMap<RouteSearchResult, RouteSearchDto>();
        
        // Route Stop Detail Mappings
        CreateMap<RouteStopDetailResult, RouteStopDetailDto>();
        
        // Vendor Schedule Creation Mapping
        CreateMap<VendorCreateScheduleDto, CreateScheduleDto>();
    }
}