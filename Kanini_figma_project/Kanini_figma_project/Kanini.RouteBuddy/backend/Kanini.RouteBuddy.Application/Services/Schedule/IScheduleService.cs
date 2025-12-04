using Kanini.RouteBuddy.Application.Dto.Schedule;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Schedule;

public interface IScheduleService
{
    Task<Result<ScheduleResponseDto>> CreateScheduleAsync(CreateScheduleDto dto, int vendorId);
    Task<Result<ScheduleResponseDto>> GetScheduleByIdAsync(int scheduleId);
    Task<Result<PagedResultDto<ScheduleResponseDto>>> GetSchedulesByVendorAsync(int vendorId, int pageNumber, int pageSize);
    Task<Result<ScheduleResponseDto>> UpdateScheduleAsync(int scheduleId, UpdateScheduleDto dto);
    Task<Result<bool>> DeleteScheduleAsync(int scheduleId);
    Task<Result<List<ScheduleResponseDto>>> CreateBulkScheduleAsync(CreateBulkScheduleDto dto, int vendorId);
}