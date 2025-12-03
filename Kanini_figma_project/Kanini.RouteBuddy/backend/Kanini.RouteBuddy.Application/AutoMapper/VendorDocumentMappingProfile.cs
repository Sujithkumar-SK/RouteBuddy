using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class VendorDocumentMappingProfile : Profile
{
    public VendorDocumentMappingProfile()
    {
        CreateMap<VendorDocument, VendorDocumentDTO>();
    }
}