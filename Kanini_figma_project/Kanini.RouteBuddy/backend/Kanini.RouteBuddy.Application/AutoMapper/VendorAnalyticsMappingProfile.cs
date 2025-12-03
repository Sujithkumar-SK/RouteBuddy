using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Vendor;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class VendorAnalyticsMappingProfile : Profile
{
    public VendorAnalyticsMappingProfile()
    {
        // Map dynamic objects to DTOs - these will be handled manually in service layer
        // due to the dynamic nature of stored procedure results
    }
}