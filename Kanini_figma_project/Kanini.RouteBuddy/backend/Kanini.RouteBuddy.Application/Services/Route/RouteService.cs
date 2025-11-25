using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Data.Repositories.Route;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services.Route;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _routeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RouteService> _logger;

    public RouteService(IRouteRepository routeRepository, IMapper mapper, ILogger<RouteService> logger)
    {
        _routeRepository = routeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<RouteResponseDto>> CreateRouteAsync(CreateRouteDto dto)
    {
        try
        {
            _logger.LogInformation(RouteMessages.LogMessages.RouteCreationStarted, dto.Source, dto.Destination);

            var existsResult = await _routeRepository.ExistsBySourceDestinationAsync(dto.Source, dto.Destination);
            if (existsResult.IsFailure)
                return Result.Failure<RouteResponseDto>(existsResult.Error);

            if (existsResult.Value)
            {
                _logger.LogWarning(RouteMessages.LogMessages.RouteAlreadyExistsWarning);
                return Result.Failure<RouteResponseDto>(
                    Error.Conflict(RouteMessages.ErrorCodes.RouteExists, RouteMessages.ErrorMessages.RouteAlreadyExists)
                );
            }

            var route = _mapper.Map<Domain.Entities.Route>(dto);
            route.IsActive = true;
            route.CreatedBy = "System";
            route.CreatedOn = DateTime.UtcNow;

            var createResult = await _routeRepository.CreateAsync(route);
            if (createResult.IsFailure)
            {
                _logger.LogError(RouteMessages.LogMessages.RouteCreationFailed, createResult.Error.Description);
                return Result.Failure<RouteResponseDto>(createResult.Error);
            }

            var response = _mapper.Map<RouteResponseDto>(createResult.Value);
            _logger.LogInformation(RouteMessages.LogMessages.RouteCreatedSuccessfully, createResult.Value.RouteId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route creation failed: {Message}", ex.Message);
            return Result.Failure<RouteResponseDto>(
                Error.Failure(RouteMessages.ErrorCodes.RouteUnexpectedError, RouteMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<RouteResponseDto>> GetRouteByIdAsync(int routeId)
    {
        try
        {
            _logger.LogInformation(RouteMessages.LogMessages.RouteRetrievalStarted, routeId);

            var result = await _routeRepository.GetByIdAsync(routeId);
            if (result.IsFailure)
            {
                _logger.LogError(RouteMessages.LogMessages.RouteRetrievalFailed, result.Error.Description);
                return Result.Failure<RouteResponseDto>(result.Error);
            }

            var response = _mapper.Map<RouteResponseDto>(result.Value);
            _logger.LogInformation(RouteMessages.LogMessages.RouteRetrievedSuccessfully, routeId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route retrieval failed: {Message}", ex.Message);
            return Result.Failure<RouteResponseDto>(
                Error.Failure(RouteMessages.ErrorCodes.RouteUnexpectedError, RouteMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<PagedResultDto<RouteResponseDto>>> GetAllRoutesAsync(int pageNumber, int pageSize)
    {
        try
        {
            _logger.LogInformation(RouteMessages.LogMessages.RouteListRetrievalStarted, pageNumber, pageSize);

            var routesResult = await _routeRepository.GetAllAsync(pageNumber, pageSize);
            if (routesResult.IsFailure)
            {
                _logger.LogError(RouteMessages.LogMessages.RouteListFailed, routesResult.Error.Description);
                return Result.Failure<PagedResultDto<RouteResponseDto>>(routesResult.Error);
            }

            var countResult = await _routeRepository.GetTotalCountAsync();
            if (countResult.IsFailure)
            {
                _logger.LogError(RouteMessages.LogMessages.RouteListFailed, countResult.Error.Description);
                return Result.Failure<PagedResultDto<RouteResponseDto>>(countResult.Error);
            }

            var routeDtos = _mapper.Map<List<RouteResponseDto>>(routesResult.Value);

            var pagedResult = new PagedResultDto<RouteResponseDto>
            {
                Data = routeDtos,
                TotalCount = countResult.Value,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            _logger.LogInformation(RouteMessages.LogMessages.RouteListRetrievedSuccessfully, routeDtos.Count);
            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route list failed: {Message}", ex.Message);
            return Result.Failure<PagedResultDto<RouteResponseDto>>(
                Error.Failure(RouteMessages.ErrorCodes.RouteUnexpectedError, RouteMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<RouteResponseDto>> UpdateRouteAsync(int routeId, UpdateRouteDto dto)
    {
        try
        {
            _logger.LogInformation(RouteMessages.LogMessages.RouteUpdateStarted, routeId);

            var getResult = await _routeRepository.GetByIdAsync(routeId);
            if (getResult.IsFailure)
            {
                _logger.LogError(RouteMessages.LogMessages.RouteUpdateFailed, getResult.Error.Description);
                return Result.Failure<RouteResponseDto>(getResult.Error);
            }

            _mapper.Map(dto, getResult.Value);
            
            var updateResult = await _routeRepository.UpdateAsync(getResult.Value);
            if (updateResult.IsFailure)
            {
                _logger.LogError(RouteMessages.LogMessages.RouteUpdateFailed, updateResult.Error.Description);
                return Result.Failure<RouteResponseDto>(updateResult.Error);
            }

            var response = _mapper.Map<RouteResponseDto>(updateResult.Value);
            _logger.LogInformation(RouteMessages.LogMessages.RouteUpdatedSuccessfully, routeId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route update failed: {Message}", ex.Message);
            return Result.Failure<RouteResponseDto>(
                Error.Failure(RouteMessages.ErrorCodes.RouteUnexpectedError, RouteMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<bool>> DeleteRouteAsync(int routeId)
    {
        try
        {
            _logger.LogInformation(RouteMessages.LogMessages.RouteDeleteStarted, routeId);

            var result = await _routeRepository.DeleteAsync(routeId);
            if (result.IsFailure)
            {
                _logger.LogError(RouteMessages.LogMessages.RouteDeleteFailed, result.Error.Description);
                return Result.Failure<bool>(result.Error);
            }

            _logger.LogInformation(RouteMessages.LogMessages.RouteDeletedSuccessfully, routeId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route delete failed: {Message}", ex.Message);
            return Result.Failure<bool>(
                Error.Failure(RouteMessages.ErrorCodes.RouteUnexpectedError, RouteMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<List<RouteStopDto>>> GetRouteStopsAsync(int routeId)
    {
        try
        {
            _logger.LogInformation(RouteMessages.LogMessages.RouteStopsRetrievalStarted, routeId);

            var result = await _routeRepository.GetRouteStopsAsync(routeId);
            if (result.IsFailure)
            {
                _logger.LogError(RouteMessages.LogMessages.RouteStopsFailed, result.Error.Description);
                return Result.Failure<List<RouteStopDto>>(result.Error);
            }

            var routeStopDtos = _mapper.Map<List<RouteStopDto>>(result.Value);
            _logger.LogInformation(RouteMessages.LogMessages.RouteStopsRetrievedSuccessfully, routeStopDtos.Count);
            return Result.Success(routeStopDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route stops failed: {Message}", ex.Message);
            return Result.Failure<List<RouteStopDto>>(
                Error.Failure(RouteMessages.ErrorCodes.RouteStopsUnexpectedError, RouteMessages.ErrorMessages.UnexpectedError)
            );
        }
    }
}