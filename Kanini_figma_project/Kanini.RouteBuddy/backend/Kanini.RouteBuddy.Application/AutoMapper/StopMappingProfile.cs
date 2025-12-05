using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class StopMappingProfile : Profile
{
    public StopMappingProfile()
    {
        CreateMap<CreateStopDto, Domain.Entities.Stop>();
        CreateMap<Domain.Entities.Stop, StopResponseDto>();
    }
}