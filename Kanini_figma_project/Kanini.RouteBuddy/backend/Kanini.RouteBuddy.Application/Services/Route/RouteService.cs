using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Data.Repositories.Route;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Data.Models;

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

    public async Task<Result<List<RouteSearchDto>>> SearchRoutesAsync(string? source, string? destination, int limit)
    {
        try
        {
            _logger.LogInformation("Searching routes - Source: {Source}, Destination: {Destination}, Limit: {Limit}", 
                source, destination, limit);

            var result = await _routeRepository.SearchRoutesAsync(source, destination, limit);
            if (result.IsFailure)
            {
                _logger.LogError("Route search failed: {Error}", result.Error.Description);
                return Result.Failure<List<RouteSearchDto>>(result.Error);
            }

            var searchDtos = _mapper.Map<List<RouteSearchDto>>(result.Value);
            _logger.LogInformation("Route search completed: {Count} routes found", searchDtos.Count);
            return Result.Success(searchDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route search failed: {Message}", ex.Message);
            return Result.Failure<List<RouteSearchDto>>(
                Error.Failure("RouteSearch.Error", "Route search failed")
            );
        }
    }

    public async Task<Result<List<RouteSearchDto>>> GetAllActiveRoutesAsync()
    {
        try
        {
            _logger.LogInformation("Getting all active routes for schedule creation");

            var result = await _routeRepository.GetAllActiveRoutesAsync();
            if (result.IsFailure)
            {
                _logger.LogError("Get all active routes failed: {Error}", result.Error.Description);
                return Result.Failure<List<RouteSearchDto>>(result.Error);
            }

            var routeDtos = _mapper.Map<List<RouteSearchDto>>(result.Value);
            _logger.LogInformation("All active routes retrieved: {Count} routes found", routeDtos.Count);
            return Result.Success(routeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get all active routes failed: {Message}", ex.Message);
            return Result.Failure<List<RouteSearchDto>>(
                Error.Failure("AllActiveRoutes.Error", "Failed to retrieve all active routes")
            );
        }
    }

    public async Task<Result<List<RouteStopDetailDto>>> GetRouteStopsWithDetailsAsync(int routeId)
    {
        try
        {
            _logger.LogInformation("Getting detailed route stops for route: {RouteId}", routeId);

            var result = await _routeRepository.GetRouteStopsWithDetailsAsync(routeId);
            if (result.IsFailure)
            {
                _logger.LogError("Route stops with details failed: {Error}", result.Error.Description);
                return Result.Failure<List<RouteStopDetailDto>>(result.Error);
            }

            var detailDtos = _mapper.Map<List<RouteStopDetailDto>>(result.Value);
            _logger.LogInformation("Route stops with details completed: {Count} stops found", detailDtos.Count);
            return Result.Success(detailDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route stops with details failed: {Message}", ex.Message);
            return Result.Failure<List<RouteStopDetailDto>>(
                Error.Failure("RouteStopsDetail.Error", "Route stops details retrieval failed")
            );
        }
    }

    public async Task<Result<List<StopResponseDto>>> GetAllActiveStopsAsync()
    {
        try
        {
            _logger.LogInformation("Getting all active stops");

            var result = await _routeRepository.GetAllActiveStopsAsync();
            if (result.IsFailure)
            {
                _logger.LogError("Get all active stops failed: {Error}", result.Error.Description);
                return Result.Failure<List<StopResponseDto>>(result.Error);
            }

            var stopDtos = _mapper.Map<List<StopResponseDto>>(result.Value);
            _logger.LogInformation("All active stops retrieved: {Count} stops found", stopDtos.Count);
            return Result.Success(stopDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get all active stops failed: {Message}", ex.Message);
            return Result.Failure<List<StopResponseDto>>(
                Error.Failure("AllActiveStops.Error", "Failed to retrieve all active stops")
            );
        }
    }

    public async Task<Result<bool>> CreateRouteStopsAsync(int routeId, List<CreateRouteStopRequest> stops, int vendorId)
    {
        try
        {
            _logger.LogInformation("Creating route stops for route: {RouteId}", routeId);

            // Convert DTO to domain model
            var domainStops = stops.Select(s => new RouteStopCreationModel
            {
                StopId = s.StopId,
                OrderNumber = s.OrderNumber,
                ArrivalTime = s.ArrivalTime,
                DepartureTime = s.DepartureTime
            }).ToList();

            var result = await _routeRepository.CreateRouteStopsAsync(routeId, domainStops, vendorId);
            if (result.IsFailure)
            {
                _logger.LogError("Create route stops failed: {Error}", result.Error.Description);
                return Result.Failure<bool>(result.Error);
            }

            _logger.LogInformation("Route stops created successfully for route: {RouteId}", routeId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create route stops failed: {Message}", ex.Message);
            return Result.Failure<bool>(
                Error.Failure("CreateRouteStops.Error", "Failed to create route stops")
            );
        }
    }

    public async Task<Result<bool>> CreateScheduleRouteStopsAsync(int scheduleId, List<CreateRouteStopRequest> stops)
    {
        try
        {
            _logger.LogInformation("Creating schedule-specific route stops for schedule: {ScheduleId}", scheduleId);

            // Convert DTO to domain model
            var domainStops = stops.Select(s => new RouteStopCreationModel
            {
                StopId = s.StopId,
                OrderNumber = s.OrderNumber,
                ArrivalTime = s.ArrivalTime,
                DepartureTime = s.DepartureTime
            }).ToList();

            var result = await _routeRepository.CreateScheduleRouteStopsAsync(scheduleId, domainStops);
            if (result.IsFailure)
            {
                _logger.LogError("Create schedule route stops failed: {Error}", result.Error.Description);
                return Result.Failure<bool>(result.Error);
            }

            _logger.LogInformation("Schedule route stops created successfully for schedule: {ScheduleId}", scheduleId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create schedule route stops failed: {Message}", ex.Message);
            return Result.Failure<bool>(
                Error.Failure("CreateScheduleRouteStops.Error", "Failed to create schedule route stops")
            );
        }
    }
}