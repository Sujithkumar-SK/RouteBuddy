using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.SmartEngine;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services.SmartEnigne;

public class SmartEngineService : ISmartEngineService
{
    private readonly ISmartEngineRepository _smartEngineRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<SmartEngineService> _logger;

    public SmartEngineService(
        ISmartEngineRepository smartEngineRepository,
        IMapper mapper,
        ILogger<SmartEngineService> logger
    )
    {
        _smartEngineRepository = smartEngineRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<ConnectingRouteResponseDto>>> FindConnectingRoutesAsync(
        ConnectingSearchRequestDto request
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingRoutesSearchStarted,
                request.Source,
                request.Destination,
                request.TravelDate
            );

            // Validation
            if (request.TravelDate.Date < DateTime.Today)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<List<ConnectingRouteResponseDto>>(
                    Error.Failure(
                        "ConnectingRoutes.InvalidDate",
                        MagicStrings.ErrorMessages.TravelDateInvalid
                    )
                );
            }

            if (
                request
                    .Source.Trim()
                    .Equals(request.Destination.Trim(), StringComparison.OrdinalIgnoreCase)
            )
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<List<ConnectingRouteResponseDto>>(
                    Error.Failure(
                        "ConnectingRoutes.SameSourceDestination",
                        MagicStrings.ErrorMessages.SameStopError
                    )
                );
            }

            if (!new[] { "cheapest", "fastest" }.Contains(request.Toggle.ToLower()))
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<List<ConnectingRouteResponseDto>>(
                    Error.Failure(
                        "ConnectingRoutes.InvalidToggle",
                        MagicStrings.ErrorMessages.InvalidToggleOption
                    )
                );
            }

            var result = await _smartEngineRepository.FindConnectingRoutesAsync(
                request.Source.Trim(),
                request.Destination.Trim(),
                request.TravelDate,
                request.Toggle.ToLower()
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ConnectingRoutesSearchFailed,
                    result.Error.Description
                );
                return Result.Failure<List<ConnectingRouteResponseDto>>(result.Error);
            }

            if (!result.Value.Any())
            {
                _logger.LogInformation(MagicStrings.LogMessages.ConnectingRoutesSearchCompleted, 0);
                return Result.Failure<List<ConnectingRouteResponseDto>>(
                    Error.NotFound(
                        "ConnectingRoutes.NotFound",
                        MagicStrings.ErrorMessages.NoConnectingRoutesFound
                    )
                );
            }

            // Group schedules into connecting routes and map using AutoMapper
            var connectingRoutes = new List<ConnectingRouteResponseDto>();
            for (int i = 0; i < result.Value.Count; i += 2)
            {
                if (i + 1 < result.Value.Count)
                {
                    var firstSegment = _mapper.Map<ConnectingRouteSegmentDto>(result.Value[i]);
                    var secondSegment = _mapper.Map<ConnectingRouteSegmentDto>(result.Value[i + 1]);

                    var connectingRoute = new ConnectingRouteResponseDto
                    {
                        Segments = new List<ConnectingRouteSegmentDto>
                        {
                            firstSegment,
                            secondSegment,
                        },
                        TotalPrice = firstSegment.Price + secondSegment.Price,
                        NumberOfTransfers = 1,
                        RouteDescription =
                            $"{firstSegment.Source} → {firstSegment.Destination} → {secondSegment.Destination}",
                    };

                    // Calculate total duration
                    var totalMinutes = (int)
                        (secondSegment.ArrivalTime - firstSegment.DepartureTime).TotalMinutes;
                    connectingRoute.TotalDurationMinutes = totalMinutes;
                    var hours = totalMinutes / 60;
                    var minutes = totalMinutes % 60;
                    connectingRoute.TotalDuration = $"{hours}h {minutes}m";

                    connectingRoutes.Add(connectingRoute);
                }
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingRoutesSearchCompleted,
                connectingRoutes.Count
            );
            return Result.Success(connectingRoutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ConnectingRoutesSearchFailed, ex.Message);
            return Result.Failure<List<ConnectingRouteResponseDto>>(
                Error.Failure(
                    "ConnectingRoutes.UnexpectedError",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<ConnectingBookingResponseDto>> BookConnectingRouteAsync(
        ConnectingBookingRequestDto request
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingStarted,
                request.CustomerId,
                request.Segments.Count,
                request.TravelDate
            );

            // Validation
            if (request.TravelDate.Date < DateTime.Today)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<ConnectingBookingResponseDto>(
                    Error.Failure(
                        "ConnectingBooking.InvalidDate",
                        MagicStrings.ErrorMessages.TravelDateInvalid
                    )
                );
            }

            // Validate passenger consistency across segments
            var firstSegmentPassengerCount = request.Segments[0].Passengers.Count;
            if (request.Segments.Any(s => s.Passengers.Count != firstSegmentPassengerCount))
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<ConnectingBookingResponseDto>(
                    Error.Failure(
                        "ConnectingBooking.PassengerMismatch",
                        MagicStrings.ErrorMessages.PassengerMismatchAcrossSegments
                    )
                );
            }

            // Validate seat-passenger count match for each segment
            foreach (var segment in request.Segments)
            {
                if (segment.SeatNumbers.Count != segment.Passengers.Count)
                {
                    _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                    return Result.Failure<ConnectingBookingResponseDto>(
                        Error.Failure(
                            "ConnectingBooking.SeatPassengerMismatch",
                            MagicStrings.ErrorMessages.SeatPassengerMismatch
                        )
                    );
                }

                if (segment.BoardingStopId == segment.DroppingStopId)
                {
                    _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                    return Result.Failure<ConnectingBookingResponseDto>(
                        Error.Failure(
                            "ConnectingBooking.SameStopError",
                            MagicStrings.ErrorMessages.SameStopError
                        )
                    );
                }
            }

            // Calculate total amount (simplified - in real implementation, get from seat pricing)
            decimal totalAmount = request.Segments.Sum(s => s.Passengers.Count * 500); // Mock pricing

            // Serialize segment data for stored procedure
            var segmentData = System.Text.Json.JsonSerializer.Serialize(request.Segments);

            var result = await _smartEngineRepository.BookConnectingRouteAsync(
                request.CustomerId,
                request.TravelDate,
                totalAmount,
                segmentData
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ConnectingBookingFailed,
                    result.Error.Description
                );
                return Result.Failure<ConnectingBookingResponseDto>(result.Error);
            }

            // Map to response DTO using AutoMapper
            var responseDto = _mapper.Map<ConnectingBookingResponseDto>(result.Value);
            responseDto.Segments = request.Segments;
            responseDto.RouteDescription = string.Join(
                " → ",
                request.Segments.Select(s => $"Segment {s.ScheduleId}")
            );
            responseDto.Message =
                "Connecting route booked successfully. Complete payment within 10 minutes.";

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingCompleted,
                responseDto.PNR,
                responseDto.BookingId
            );
            return Result.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ConnectingBookingFailed, ex.Message);
            return Result.Failure<ConnectingBookingResponseDto>(
                Error.Failure(
                    "ConnectingBooking.UnexpectedError",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<string>> ConfirmConnectingBookingAsync(BookingConfirmationDto request)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingConfirmationStarted,
                request.BookingId
            );

            var result = await _smartEngineRepository.ConfirmConnectingBookingAsync(
                request.BookingId,
                request.PaymentReferenceId,
                request.IsPaymentSuccessful
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ConnectingBookingConfirmationFailed,
                    result.Error.Description
                );
                return Result.Failure<string>(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingConfirmationCompleted,
                request.BookingId
            );
            return Result.Success(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ConnectingBookingConfirmationFailed,
                ex.Message
            );
            return Result.Failure<string>(
                Error.Failure(
                    "ConnectingBooking.ConfirmationUnexpectedError",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }
}
