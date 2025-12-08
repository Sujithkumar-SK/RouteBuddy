using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Application.Services.Admin;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services.Admin;

public class AdminBusService : IAdminBusService
{
    private readonly IBusRepository _busRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminBusService> _logger;

    public AdminBusService(
        IBusRepository busRepository,
        IMapper mapper,
        ILogger<AdminBusService> logger)
    {
        _busRepository = busRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<BusResponseDto>>> GetAllBusesAsync()
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.GettingAllBuses);
            
            var buses = await _busRepository.GetAllBusesForAdminAsync();
            var busesDto = _mapper.Map<List<BusResponseDto>>(buses);

            _logger.LogInformation(BusMessages.LogMessages.AllBusesRetrievedSuccessfully, busesDto.Count);
            return Result.Success(busesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.AllBusesRetrievalException);
            return Result.Failure<List<BusResponseDto>>(
                Error.Failure("Bus.DatabaseError", BusMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<List<BusResponseDto>>> GetPendingBusesAsync()
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.GettingPendingBuses);
            
            var buses = await _busRepository.GetBusesByStatusAsync(BusStatus.PendingApproval);
            var busesDto = _mapper.Map<List<BusResponseDto>>(buses);

            _logger.LogInformation(BusMessages.LogMessages.PendingBusesRetrievedSuccessfully, busesDto.Count);
            return Result.Success(busesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.PendingBusesRetrievalException);
            return Result.Failure<List<BusResponseDto>>(
                Error.Failure("Bus.DatabaseError", BusMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<List<BusResponseDto>>> FilterBusesAsync(string? searchName, int? status, bool? isActive)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.FilteringBuses, searchName, status, isActive);
            
            var buses = await _busRepository.FilterBusesForAdminAsync(searchName, status, isActive);
            var busesDto = _mapper.Map<List<BusResponseDto>>(buses);

            _logger.LogInformation(BusMessages.LogMessages.BusesFilteredSuccessfully, busesDto.Count);
            return Result.Success(busesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusesFilterException);
            return Result.Failure<List<BusResponseDto>>(
                Error.Failure("Bus.DatabaseError", BusMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<BusResponseDto>> GetBusDetailsAsync(int busId)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.GettingBusById, busId);
            
            var bus = await _busRepository.GetBusDetailsForAdminAsync(busId);
            if (bus == null)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, busId);
                return Result.Failure<BusResponseDto>(
                    Error.NotFound("Bus.NotFound", BusMessages.ErrorMessages.BusNotFound));
            }

            var busDto = _mapper.Map<BusResponseDto>(bus);
            _logger.LogInformation(BusMessages.LogMessages.BusRetrievedSuccessfully, busId);
            return Result.Success(busDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusRetrievalException, busId);
            return Result.Failure<BusResponseDto>(
                Error.Failure("Bus.DatabaseError", BusMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<bool>> ApproveBusAsync(int busId)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.ApprovingBus, busId);
            
            var busResult = await _busRepository.GetByIdAsync(busId);
            if (busResult.IsFailure)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, busId);
                return Result.Failure<bool>(
                    Error.NotFound("Bus.NotFound", BusMessages.ErrorMessages.BusNotFound));
            }

            var bus = busResult.Value;
            bus.Status = BusStatus.Active;
            bus.UpdatedBy = "Admin";
            bus.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _busRepository.UpdateAsync(bus);
            if (updateResult.IsFailure)
            {
                return Result.Failure<bool>(updateResult.Error);
            }
            
            _logger.LogInformation(BusMessages.LogMessages.BusApprovedSuccessfully, busId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusApprovalException, busId);
            return Result.Failure<bool>(
                Error.Failure("Bus.DatabaseError", BusMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<bool>> RejectBusAsync(int busId, string rejectionReason)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.RejectingBus, busId);
            
            var busResult = await _busRepository.GetByIdAsync(busId);
            if (busResult.IsFailure)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, busId);
                return Result.Failure<bool>(
                    Error.NotFound("Bus.NotFound", BusMessages.ErrorMessages.BusNotFound));
            }

            var bus = busResult.Value;
            bus.Status = BusStatus.Rejected;
            bus.UpdatedBy = "Admin";
            bus.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _busRepository.UpdateAsync(bus);
            if (updateResult.IsFailure)
            {
                return Result.Failure<bool>(updateResult.Error);
            }
            
            _logger.LogInformation(BusMessages.LogMessages.BusRejectedSuccessfully, busId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusRejectionException, busId);
            return Result.Failure<bool>(
                Error.Failure("Bus.DatabaseError", BusMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<bool>> DeactivateBusAsync(int busId, string reason)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.DeactivatingBus, busId);
            
            var busResult = await _busRepository.GetByIdAsync(busId);
            if (busResult.IsFailure)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, busId);
                return Result.Failure<bool>(
                    Error.NotFound("Bus.NotFound", BusMessages.ErrorMessages.BusNotFound));
            }

            var bus = busResult.Value;
            bus.IsActive = false;
            bus.UpdatedBy = "Admin";
            bus.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _busRepository.UpdateAsync(bus);
            if (updateResult.IsFailure)
            {
                return Result.Failure<bool>(updateResult.Error);
            }
            
            _logger.LogInformation(BusMessages.LogMessages.BusDeactivatedSuccessfully, busId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusDeactivationException, busId);
            return Result.Failure<bool>(
                Error.Failure("Bus.DatabaseError", BusMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<bool>> ReactivateBusAsync(int busId, string reason)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.ReactivatingBus, busId);
            
            var busResult = await _busRepository.GetByIdAsync(busId);
            if (busResult.IsFailure)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, busId);
                return Result.Failure<bool>(
                    Error.NotFound("Bus.NotFound", BusMessages.ErrorMessages.BusNotFound));
            }

            var bus = busResult.Value;
            bus.IsActive = true;
            bus.UpdatedBy = "Admin";
            bus.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _busRepository.UpdateAsync(bus);
            if (updateResult.IsFailure)
            {
                return Result.Failure<bool>(updateResult.Error);
            }
            
            _logger.LogInformation(BusMessages.LogMessages.BusReactivatedSuccessfully, busId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusReactivationException, busId);
            return Result.Failure<bool>(
                Error.Failure("Bus.DatabaseError", BusMessages.ErrorMessages.DatabaseError));
        }
    }
}