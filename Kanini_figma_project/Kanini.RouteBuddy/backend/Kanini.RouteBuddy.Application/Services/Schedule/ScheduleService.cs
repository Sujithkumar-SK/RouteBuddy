using AutoMapper;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Application.Dto.Schedule;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Data.Repositories.Schedule;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Data.Repositories.Route;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using BusSchedule = Kanini.RouteBuddy.Domain.Entities.BusSchedule;

namespace Kanini.RouteBuddy.Application.Services.Schedule;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IBusRepository _busRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ScheduleService> _logger;

    public ScheduleService(IScheduleRepository scheduleRepository, IBusRepository busRepository, IRouteRepository routeRepository, IMapper mapper, ILogger<ScheduleService> logger)
    {
        _scheduleRepository = scheduleRepository;
        _busRepository = busRepository;
        _routeRepository = routeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ScheduleResponseDto>> CreateScheduleAsync(CreateScheduleDto dto, int vendorId)
    {
        try
        {
            _logger.LogInformation(ScheduleMessages.LogMessages.CreatingSchedule, dto.BusId, dto.RouteId, dto.TravelDate);

            var busResult = await _busRepository.GetByIdAsync(dto.BusId);
            if (!busResult.IsSuccess || busResult.Value == null || busResult.Value.VendorId != vendorId)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.BusNotFoundForVendor, dto.BusId, vendorId);
                return Result.Failure<ScheduleResponseDto>(Error.NotFound(ScheduleMessages.ErrorCodes.BusNotFound, ScheduleMessages.ErrorMessages.BusNotAvailable));
            }

            var bus = busResult.Value;
            if (bus.Status != BusStatus.Active || !bus.IsActive)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.BusNotActive, dto.BusId);
                return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.BusNotActive, ScheduleMessages.ErrorMessages.BusNotActive));
            }

            if (dto.TravelDate.Date < DateTime.UtcNow.Date)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.PastDateSchedule, dto.TravelDate);
                return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.PastDate, ScheduleMessages.ErrorMessages.PastDateSchedule));
            }

            // Handle overnight journeys
            var timeDifference = dto.ArrivalTime > dto.DepartureTime 
                ? dto.ArrivalTime - dto.DepartureTime 
                : TimeSpan.FromDays(1) - dto.DepartureTime + dto.ArrivalTime;

            if (timeDifference.TotalMinutes < 30)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.InvalidScheduleTime, dto.DepartureTime, dto.ArrivalTime);
                return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.InvalidTime, "Journey must be at least 30 minutes long"));
            }

            if (timeDifference.TotalHours > 24)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.InvalidScheduleTime, dto.DepartureTime, dto.ArrivalTime);
                return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.InvalidTime, "Journey cannot exceed 24 hours"));
            }

            var routeResult = await _routeRepository.GetByIdAsync(dto.RouteId);
            if (!routeResult.IsSuccess || routeResult.Value == null)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.RouteNotFound, dto.RouteId);
                return Result.Failure<ScheduleResponseDto>(Error.NotFound(ScheduleMessages.ErrorCodes.RouteNotFound, ScheduleMessages.ErrorMessages.RouteNotFound));
            }

            var existsResult = await _scheduleRepository.ExistsByBusRouteAndDateAsync(dto.BusId, dto.RouteId, dto.TravelDate);
            if (!existsResult.IsSuccess)
            {
                _logger.LogError(ScheduleMessages.LogMessages.ScheduleExistenceCheckFailed, dto.BusId, dto.RouteId, dto.TravelDate);
                return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.DatabaseError, ScheduleMessages.ErrorMessages.DatabaseError));
            }

            if (existsResult.Value)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleAlreadyExists, dto.BusId, dto.RouteId, dto.TravelDate);
                return Result.Failure<ScheduleResponseDto>(Error.Conflict(ScheduleMessages.ErrorCodes.ScheduleExists, ScheduleMessages.ErrorMessages.ScheduleAlreadyExists));
            }

            var schedule = _mapper.Map<BusSchedule>(dto);
            schedule.AvailableSeats = bus.TotalSeats;
            schedule.Status = ScheduleStatus.Scheduled;
            schedule.IsActive = true;
            schedule.CreatedBy = $"Vendor-{vendorId}";
            schedule.CreatedOn = DateTime.UtcNow;

            var createResult = await _scheduleRepository.CreateAsync(schedule);
            if (!createResult.IsSuccess)
            {
                _logger.LogError(ScheduleMessages.LogMessages.ScheduleCreationFailed, dto.BusId, dto.RouteId);
                return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.CreationFailed, ScheduleMessages.ErrorMessages.ScheduleCreationFailed));
            }

            // Load related entities for response
            createResult.Value.Bus = bus;
            createResult.Value.Route = routeResult.Value;
            
            var response = _mapper.Map<ScheduleResponseDto>(createResult.Value);
            _logger.LogInformation(ScheduleMessages.LogMessages.ScheduleCreatedSuccessfully, createResult.Value.ScheduleId);
            
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleCreationException, dto.BusId, dto.RouteId);
            return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.UnexpectedError, ScheduleMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<ScheduleResponseDto>> GetScheduleByIdAsync(int scheduleId)
    {
        try
        {
            _logger.LogInformation(ScheduleMessages.LogMessages.GettingScheduleById, scheduleId);

            var result = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (!result.IsSuccess || result.Value == null)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleNotFound, scheduleId);
                return Result.Failure<ScheduleResponseDto>(Error.NotFound(ScheduleMessages.ErrorCodes.ScheduleNotFound, ScheduleMessages.ErrorMessages.ScheduleNotFound));
            }

            var response = _mapper.Map<ScheduleResponseDto>(result.Value);
            _logger.LogInformation(ScheduleMessages.LogMessages.ScheduleRetrievedSuccessfully, scheduleId);
            
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleRetrievalException, scheduleId);
            return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.UnexpectedError, ScheduleMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<PagedResultDto<ScheduleResponseDto>>> GetSchedulesByVendorAsync(int vendorId, int pageNumber, int pageSize)
    {
        try
        {
            _logger.LogInformation(ScheduleMessages.LogMessages.GettingSchedulesByVendor, vendorId, pageNumber, pageSize);

            var schedulesResult = await _scheduleRepository.GetByVendorIdAsync(vendorId, pageNumber, pageSize);
            if (!schedulesResult.IsSuccess)
            {
                _logger.LogError(ScheduleMessages.LogMessages.SchedulesByVendorRetrievalFailed, vendorId);
                return Result.Failure<PagedResultDto<ScheduleResponseDto>>(Error.Failure(ScheduleMessages.ErrorCodes.DatabaseError, ScheduleMessages.ErrorMessages.DatabaseError));
            }

            var countResult = await _scheduleRepository.GetCountByVendorIdAsync(vendorId);
            if (!countResult.IsSuccess)
            {
                _logger.LogError(ScheduleMessages.LogMessages.ScheduleCountRetrievalFailed, vendorId);
                return Result.Failure<PagedResultDto<ScheduleResponseDto>>(Error.Failure(ScheduleMessages.ErrorCodes.DatabaseError, ScheduleMessages.ErrorMessages.DatabaseError));
            }
            
            var scheduleDtos = _mapper.Map<IEnumerable<ScheduleResponseDto>>(schedulesResult.Value);
            _logger.LogInformation(ScheduleMessages.LogMessages.SchedulesByVendorRetrievedSuccessfully, vendorId, schedulesResult.Value.Count());

            var result = new PagedResultDto<ScheduleResponseDto>
            {
                Data = scheduleDtos,
                TotalCount = countResult.Value,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.SchedulesByVendorException, vendorId);
            return Result.Failure<PagedResultDto<ScheduleResponseDto>>(Error.Failure(ScheduleMessages.ErrorCodes.UnexpectedError, ScheduleMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<ScheduleResponseDto>> UpdateScheduleAsync(int scheduleId, UpdateScheduleDto dto)
    {
        try
        {
            _logger.LogInformation(ScheduleMessages.LogMessages.UpdatingSchedule, scheduleId);

            var getResult = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (!getResult.IsSuccess || getResult.Value == null)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleNotFound, scheduleId);
                return Result.Failure<ScheduleResponseDto>(Error.NotFound(ScheduleMessages.ErrorCodes.ScheduleNotFound, ScheduleMessages.ErrorMessages.ScheduleNotFound));
            }

            var schedule = getResult.Value;
            _mapper.Map(dto, schedule);
            
            var updateResult = await _scheduleRepository.UpdateAsync(schedule);
            if (!updateResult.IsSuccess)
            {
                _logger.LogError(ScheduleMessages.LogMessages.ScheduleUpdateFailed, scheduleId);
                return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.UpdateFailed, ScheduleMessages.ErrorMessages.ScheduleUpdateFailed));
            }

            var response = _mapper.Map<ScheduleResponseDto>(updateResult.Value);
            _logger.LogInformation(ScheduleMessages.LogMessages.ScheduleUpdatedSuccessfully, scheduleId);
            
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleUpdateException, scheduleId);
            return Result.Failure<ScheduleResponseDto>(Error.Failure(ScheduleMessages.ErrorCodes.UnexpectedError, ScheduleMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<bool>> DeleteScheduleAsync(int scheduleId)
    {
        try
        {
            _logger.LogInformation(ScheduleMessages.LogMessages.DeletingSchedule, scheduleId);

            var result = await _scheduleRepository.DeleteAsync(scheduleId);
            if (!result.IsSuccess || !result.Value)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleNotFound, scheduleId);
                return Result.Failure<bool>(Error.NotFound(ScheduleMessages.ErrorCodes.ScheduleNotFound, ScheduleMessages.ErrorMessages.ScheduleNotFound));
            }

            _logger.LogInformation(ScheduleMessages.LogMessages.ScheduleDeletedSuccessfully, scheduleId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleDeletionException, scheduleId);
            return Result.Failure<bool>(Error.Failure(ScheduleMessages.ErrorCodes.UnexpectedError, ScheduleMessages.ErrorMessages.UnexpectedError));
        }
    }



    public async Task<Result<List<ScheduleResponseDto>>> CreateBulkScheduleAsync(CreateBulkScheduleDto dto, int vendorId)
    {
        try
        {
            _logger.LogInformation(ScheduleMessages.LogMessages.CreatingBulkSchedule, dto.BusId, dto.RouteId, dto.StartDate, dto.EndDate);

            var busResult = await _busRepository.GetByIdAsync(dto.BusId);
            if (!busResult.IsSuccess || busResult.Value == null || busResult.Value.VendorId != vendorId)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.BusNotFoundForVendor, dto.BusId, vendorId);
                return Result.Failure<List<ScheduleResponseDto>>(Error.NotFound(ScheduleMessages.ErrorCodes.BusNotFound, ScheduleMessages.ErrorMessages.BusNotAvailable));
            }

            var bus = busResult.Value;
            if (bus.Status != BusStatus.Active || !bus.IsActive)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.BusNotActive, dto.BusId);
                return Result.Failure<List<ScheduleResponseDto>>(Error.Failure(ScheduleMessages.ErrorCodes.BusNotActive, ScheduleMessages.ErrorMessages.BusNotActive));
            }

            var routeResult = await _routeRepository.GetByIdAsync(dto.RouteId);
            if (!routeResult.IsSuccess || routeResult.Value == null)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.RouteNotFound, dto.RouteId);
                return Result.Failure<List<ScheduleResponseDto>>(Error.NotFound(ScheduleMessages.ErrorCodes.RouteNotFound, ScheduleMessages.ErrorMessages.RouteNotFound));
            }

            var createdSchedules = new List<ScheduleResponseDto>();
            var currentDate = dto.StartDate.Date;

            while (currentDate <= dto.EndDate.Date)
            {
                if (dto.OperatingDays.Any() && !dto.OperatingDays.Contains(currentDate.DayOfWeek))
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                var existsResult = await _scheduleRepository.ExistsByBusRouteAndDateAsync(dto.BusId, dto.RouteId, currentDate);
                if (!existsResult.IsSuccess)
                {
                    _logger.LogError(ScheduleMessages.LogMessages.ScheduleExistenceCheckFailed, dto.BusId, dto.RouteId, currentDate);
                    return Result.Failure<List<ScheduleResponseDto>>(Error.Failure(ScheduleMessages.ErrorCodes.DatabaseError, ScheduleMessages.ErrorMessages.DatabaseError));
                }

                if (existsResult.Value)
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                var schedule = _mapper.Map<BusSchedule>(dto);
                schedule.TravelDate = currentDate;
                schedule.AvailableSeats = bus.TotalSeats;
                schedule.Status = ScheduleStatus.Scheduled;
                schedule.IsActive = true;

                var createResult = await _scheduleRepository.CreateAsync(schedule);
                if (!createResult.IsSuccess)
                {
                    _logger.LogError(ScheduleMessages.LogMessages.ScheduleCreationFailed, dto.BusId, dto.RouteId);
                    return Result.Failure<List<ScheduleResponseDto>>(Error.Failure(ScheduleMessages.ErrorCodes.CreationFailed, ScheduleMessages.ErrorMessages.ScheduleCreationFailed));
                }

                var scheduleDto = _mapper.Map<ScheduleResponseDto>(createResult.Value);
                createdSchedules.Add(scheduleDto);

                currentDate = currentDate.AddDays(1);
            }

            _logger.LogInformation(ScheduleMessages.LogMessages.BulkScheduleCreatedSuccessfully, createdSchedules.Count);
            return Result.Success(createdSchedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.BulkScheduleCreationException, dto.BusId, dto.RouteId);
            return Result.Failure<List<ScheduleResponseDto>>(Error.Failure(ScheduleMessages.ErrorCodes.UnexpectedError, ScheduleMessages.ErrorMessages.UnexpectedError));
        }
    }
}