using Kanini.RouteBuddy.Application.Dto.BusPhoto;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.BusPhoto;

public interface IBusPhotoService
{
    Task<Result<BusPhotoDto>> CreateAsync(CreateBusPhotoDto createDto);
    Task<Result<BusPhotoDto>> GetByIdAsync(int busPhotoId);
    Task<Result<IEnumerable<BusPhotoDto>>> GetByBusIdAsync(int busId);
    Task<Result<BusPhotoDto>> UpdateAsync(int busPhotoId, UpdateBusPhotoDto updateDto);
    Task<Result<bool>> DeleteAsync(int busPhotoId);
}