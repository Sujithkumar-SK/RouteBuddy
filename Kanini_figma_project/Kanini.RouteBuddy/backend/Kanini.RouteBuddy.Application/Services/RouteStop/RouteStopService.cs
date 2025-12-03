using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Data.Repositories.RouteStop;
using Kanini.RouteBuddy.Data.Repositories.Route;
using Kanini.RouteBuddy.Data.Repositories.Stop;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Application.Services.RouteStop;

public class RouteStopService : IRouteStopService
{
    private readonly IRouteStopRepository _routeStopRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IStopRepository _stopRepository;
    private readonly IMapper _mapper;

    public RouteStopService(IRouteStopRepository routeStopRepository, IRouteRepository routeRepository, IStopRepository stopRepository, IMapper mapper)
    {
        _routeStopRepository = routeStopRepository;
        _routeRepository = routeRepository;
        _stopRepository = stopRepository;
        _mapper = mapper;
    }

    public async Task<Result<RouteStopDto>> CreateRouteStopAsync(int routeId, CreateRouteStopDto dto)
    {
        try
        {
            var route = await _routeRepository.GetByIdAsync(routeId);
            if (route == null)
                return Result.Failure<RouteStopDto>(Error.NotFound("Route.NotFound", RouteStopMessages.RouteNotFound));

            var stopResult = await _stopRepository.GetByIdAsync(dto.StopId);
            if (stopResult.IsFailure)
                return Result.Failure<RouteStopDto>(Error.NotFound("Stop.NotFound", RouteStopMessages.StopNotFound));

            // Check for route-level template stops only (ScheduleId = NULL)
            if (await _routeStopRepository.ExistsByRouteAndOrderAsync(routeId, dto.OrderNumber))
                return Result.Failure<RouteStopDto>(Error.Conflict("Order.Exists", RouteStopMessages.DuplicateOrderNumber));

            var routeStop = new Domain.Entities.RouteStop
            {
                RouteId = routeId,
                StopId = dto.StopId,
                OrderNumber = dto.OrderNumber,
                ArrivalTime = dto.ArrivalTime,
                DepartureTime = dto.DepartureTime,
                ScheduleId = null, // Route-level template
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var created = await _routeStopRepository.CreateAsync(routeStop);
            var response = _mapper.Map<RouteStopDto>(created);
            response.StopName = stopResult.Value.Name;
            
            return Result.Success(response);
        }
        catch (SqlException)
        {
            return Result.Failure<RouteStopDto>(Error.Failure("Database.Error", RouteStopMessages.DatabaseError));
        }
        catch (DbUpdateException)
        {
            return Result.Failure<RouteStopDto>(Error.Failure("Database.Error", RouteStopMessages.DatabaseError));
        }
        catch (Exception)
        {
            return Result.Failure<RouteStopDto>(Error.Failure("Unexpected.Error", RouteStopMessages.UnexpectedError));
        }
    }

    public async Task<Result<RouteStopDto>> GetRouteStopByIdAsync(int routeStopId)
    {
        try
        {
            var routeStop = await _routeStopRepository.GetByIdAsync(routeStopId);
            if (routeStop == null)
                return Result.Failure<RouteStopDto>(Error.NotFound("RouteStop.NotFound", RouteStopMessages.RouteStopNotFound));

            var response = _mapper.Map<RouteStopDto>(routeStop);
            response.StopName = routeStop.Stop?.Name ?? "";
            
            return Result.Success(response);
        }
        catch (SqlException)
        {
            return Result.Failure<RouteStopDto>(Error.Failure("Database.Error", RouteStopMessages.DatabaseError));
        }
        catch (Exception)
        {
            return Result.Failure<RouteStopDto>(Error.Failure("Unexpected.Error", RouteStopMessages.UnexpectedError));
        }
    }

    public async Task<IEnumerable<RouteStopDto>> GetRouteStopsByRouteIdAsync(int routeId)
    {
        try
        {
            var routeStops = await _routeStopRepository.GetByRouteIdAsync(routeId);
            var response = _mapper.Map<IEnumerable<RouteStopDto>>(routeStops);
            
            foreach (var dto in response)
            {
                var routeStop = routeStops.FirstOrDefault(rs => rs.RouteStopId == dto.RouteStopId);
                dto.StopName = routeStop?.Stop?.Name ?? "";
            }
            
            return response;
        }
        catch (SqlException)
        {
            return new List<RouteStopDto>();
        }
        catch (Exception)
        {
            return new List<RouteStopDto>();
        }
    }

    public async Task<Result<RouteStopDto>> UpdateRouteStopAsync(int routeStopId, UpdateRouteStopDto dto)
    {
        try
        {
            var routeStop = await _routeStopRepository.GetByIdAsync(routeStopId);
            if (routeStop == null)
                return Result.Failure<RouteStopDto>(Error.NotFound("RouteStop.NotFound", RouteStopMessages.RouteStopNotFound));

            if (routeStop.OrderNumber != dto.OrderNumber && 
                await _routeStopRepository.ExistsByRouteAndOrderAsync(routeStop.RouteId, dto.OrderNumber))
                return Result.Failure<RouteStopDto>(Error.Conflict("Order.Exists", RouteStopMessages.DuplicateOrderNumber));

            routeStop.OrderNumber = dto.OrderNumber;
            routeStop.ArrivalTime = dto.ArrivalTime;
            routeStop.DepartureTime = dto.DepartureTime;

            var updated = await _routeStopRepository.UpdateAsync(routeStop);
            var response = _mapper.Map<RouteStopDto>(updated);
            response.StopName = updated.Stop?.Name ?? "";
            
            return Result.Success(response);
        }
        catch (SqlException)
        {
            return Result.Failure<RouteStopDto>(Error.Failure("Database.Error", RouteStopMessages.DatabaseError));
        }
        catch (DbUpdateException)
        {
            return Result.Failure<RouteStopDto>(Error.Failure("Database.Error", RouteStopMessages.DatabaseError));
        }
        catch (Exception)
        {
            return Result.Failure<RouteStopDto>(Error.Failure("Unexpected.Error", RouteStopMessages.UnexpectedError));
        }
    }

    public async Task<Result<bool>> DeleteRouteStopAsync(int routeStopId)
    {
        try
        {
            var deleted = await _routeStopRepository.DeleteAsync(routeStopId);
            if (!deleted)
                return Result.Failure<bool>(Error.NotFound("RouteStop.NotFound", RouteStopMessages.RouteStopNotFound));

            return Result.Success(true);
        }
        catch (SqlException)
        {
            return Result.Failure<bool>(Error.Failure("Database.Error", RouteStopMessages.DatabaseError));
        }
        catch (Exception)
        {
            return Result.Failure<bool>(Error.Failure("Unexpected.Error", RouteStopMessages.UnexpectedError));
        }
    }
}