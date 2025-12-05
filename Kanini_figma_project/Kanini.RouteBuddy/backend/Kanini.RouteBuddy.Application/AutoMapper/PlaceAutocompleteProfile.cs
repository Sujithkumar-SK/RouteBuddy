using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Data.Repositories.Stop;

namespace Kanini.RouteBuddy.Application.AutoMapper;

public class PlaceAutocompleteProfile : Profile
{
    public PlaceAutocompleteProfile()
    {
        CreateMap<PlaceAutocompleteResult, PlaceAutocompleteResponseDto>();
    }
}