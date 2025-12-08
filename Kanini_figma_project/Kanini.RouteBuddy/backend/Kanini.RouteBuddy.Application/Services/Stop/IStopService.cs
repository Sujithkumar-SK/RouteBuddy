using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Stop;

public interface IStopService
{
    Task<Result<StopResponseDto>> CreateStopAsync(CreateStopDto dto);
    Task<Result<StopResponseDto>> GetStopByIdAsync(int stopId);
    Task<Result<PagedResultDto<StopResponseDto>>> GetAllStopsAsync(int pageNumber, int pageSize);
    Task<Result<StopResponseDto>> UpdateStopAsync(int stopId, UpdateStopDto dto);
    Task<Result<bool>> DeleteStopAsync(int stopId);
    Task<Result<List<PlaceAutocompleteResponseDto>>> GetPlaceAutocompleteAsync(PlaceAutocompleteRequestDto request);
    Task<Result<bool>> ValidatePlaceExistsAsync(string placeName);
}