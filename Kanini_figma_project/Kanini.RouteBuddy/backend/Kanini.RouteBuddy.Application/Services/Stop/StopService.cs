using AutoMapper;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Data.Repositories.Stop;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.Services.Stop;

public class StopService : IStopService
{
    private readonly IStopRepository _stopRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<StopService> _logger;

    public StopService(IStopRepository stopRepository, IMapper mapper, ILogger<StopService> logger)
    {
        _stopRepository = stopRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StopResponseDto>> CreateStopAsync(CreateStopDto dto)
    {
        try
        {
            _logger.LogInformation(StopMessages.LogMessages.CreatingStop, dto.Name);

            var existsResult = await _stopRepository.ExistsByNameAsync(dto.Name);
            if (existsResult.IsFailure)
                return Result.Failure<StopResponseDto>(existsResult.Error);

            if (existsResult.Value)
            {
                _logger.LogWarning("Stop already exists: {Name}", dto.Name);
                return Result.Failure<StopResponseDto>(Error.Conflict(StopMessages.ErrorCodes.StopExists, StopMessages.ErrorMessages.StopAlreadyExists));
            }

            var stop = _mapper.Map<Domain.Entities.Stop>(dto);
            stop.IsActive = true;
            stop.CreatedBy = "System";
            stop.CreatedOn = DateTime.UtcNow;

            var createResult = await _stopRepository.CreateAsync(stop);
            if (createResult.IsFailure)
            {
                _logger.LogError("Stop creation failed: {Error}", createResult.Error.Description);
                return Result.Failure<StopResponseDto>(createResult.Error);
            }

            var response = _mapper.Map<StopResponseDto>(createResult.Value);
            _logger.LogInformation(StopMessages.LogMessages.StopCreatedSuccessfully, createResult.Value.StopId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop creation failed: {Message}", ex.Message);
            return Result.Failure<StopResponseDto>(
                Error.Failure(StopMessages.ErrorCodes.UnexpectedError, StopMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<StopResponseDto>> GetStopByIdAsync(int stopId)
    {
        try
        {
            _logger.LogInformation(StopMessages.LogMessages.GettingStopById, stopId);

            var result = await _stopRepository.GetByIdAsync(stopId);
            if (result.IsFailure)
            {
                _logger.LogError("Stop retrieval failed: {Error}", result.Error.Description);
                return Result.Failure<StopResponseDto>(result.Error);
            }

            var response = _mapper.Map<StopResponseDto>(result.Value);
            _logger.LogInformation(StopMessages.LogMessages.StopRetrievedSuccessfully, stopId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop retrieval failed: {Message}", ex.Message);
            return Result.Failure<StopResponseDto>(
                Error.Failure(StopMessages.ErrorCodes.UnexpectedError, StopMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<PagedResultDto<StopResponseDto>>> GetAllStopsAsync(int pageNumber, int pageSize)
    {
        try
        {
            _logger.LogInformation(StopMessages.LogMessages.GettingAllStops, pageNumber, pageSize);

            var stopsResult = await _stopRepository.GetAllAsync(pageNumber, pageSize);
            if (stopsResult.IsFailure)
            {
                _logger.LogError("Stop list failed: {Error}", stopsResult.Error.Description);
                return Result.Failure<PagedResultDto<StopResponseDto>>(stopsResult.Error);
            }

            var countResult = await _stopRepository.GetCountAsync();
            if (countResult.IsFailure)
            {
                _logger.LogError("Stop count failed: {Error}", countResult.Error.Description);
                return Result.Failure<PagedResultDto<StopResponseDto>>(countResult.Error);
            }

            var stopDtos = _mapper.Map<List<StopResponseDto>>(stopsResult.Value);
            var pagedResult = new PagedResultDto<StopResponseDto>
            {
                Data = stopDtos,
                TotalCount = countResult.Value,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            _logger.LogInformation(StopMessages.LogMessages.StopsRetrievedSuccessfully, stopDtos.Count);
            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop list failed: {Message}", ex.Message);
            return Result.Failure<PagedResultDto<StopResponseDto>>(
                Error.Failure(StopMessages.ErrorCodes.UnexpectedError, StopMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<StopResponseDto>> UpdateStopAsync(int stopId, UpdateStopDto dto)
    {
        try
        {
            _logger.LogInformation(StopMessages.LogMessages.UpdatingStop, stopId);

            var getResult = await _stopRepository.GetByIdAsync(stopId);
            if (getResult.IsFailure)
            {
                _logger.LogError("Stop update failed: {Error}", getResult.Error.Description);
                return Result.Failure<StopResponseDto>(getResult.Error);
            }

            var stop = getResult.Value;
            _mapper.Map(dto, stop);
            stop.UpdatedBy = "System";
            stop.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _stopRepository.UpdateAsync(stop);
            if (updateResult.IsFailure)
            {
                _logger.LogError("Stop update failed: {Error}", updateResult.Error.Description);
                return Result.Failure<StopResponseDto>(updateResult.Error);
            }

            var response = _mapper.Map<StopResponseDto>(updateResult.Value);
            _logger.LogInformation(StopMessages.LogMessages.StopUpdatedSuccessfully, stopId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop update failed: {Message}", ex.Message);
            return Result.Failure<StopResponseDto>(
                Error.Failure(StopMessages.ErrorCodes.UnexpectedError, StopMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<bool>> DeleteStopAsync(int stopId)
    {
        try
        {
            _logger.LogInformation(StopMessages.LogMessages.DeletingStop, stopId);

            var result = await _stopRepository.DeleteAsync(stopId);
            if (result.IsFailure)
            {
                _logger.LogError("Stop delete failed: {Error}", result.Error.Description);
                return Result.Failure<bool>(result.Error);
            }

            _logger.LogInformation(StopMessages.LogMessages.StopDeletedSuccessfully, stopId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop delete failed: {Message}", ex.Message);
            return Result.Failure<bool>(
                Error.Failure(StopMessages.ErrorCodes.UnexpectedError, StopMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<List<PlaceAutocompleteResponseDto>>> GetPlaceAutocompleteAsync(PlaceAutocompleteRequestDto request)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.PlaceAutocompleteStarted, request.Query);

            var result = await _stopRepository.GetPlaceAutocompleteAsync(request.Query, request.Limit);
            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.PlaceAutocompleteFailed, result.Error.Description);
                return Result.Failure<List<PlaceAutocompleteResponseDto>>(result.Error);
            }

            var response = _mapper.Map<List<PlaceAutocompleteResponseDto>>(result.Value);
            _logger.LogInformation(MagicStrings.LogMessages.PlaceAutocompleteCompleted, response.Count);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PlaceAutocompleteFailed, ex.Message);
            return Result.Failure<List<PlaceAutocompleteResponseDto>>(
                Error.Failure("PLACE_AUTOCOMPLETE_FAILED", MagicStrings.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<bool>> ValidatePlaceExistsAsync(string placeName)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.PlaceValidationStarted, placeName, placeName);

            var result = await _stopRepository.ValidatePlaceExistsAsync(placeName);
            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.PlaceValidationFailed, result.Error.Description);
                return Result.Failure<bool>(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.PlaceValidationCompleted);
            return Result.Success(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PlaceValidationFailed, ex.Message);
            return Result.Failure<bool>(
                Error.Failure("PLACE_VALIDATION_FAILED", MagicStrings.ErrorMessages.UnexpectedError)
            );
        }
    }
}