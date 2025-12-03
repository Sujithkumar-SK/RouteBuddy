using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Application.Services.Stop;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Data.Repositories.Customer;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Logging;
using BookingEntity = Kanini.RouteBuddy.Domain.Entities.Booking;
using CustomerEntity = Kanini.RouteBuddy.Domain.Entities.Customer;

namespace Kanini.RouteBuddy.Application.Services.Buses;

public class Bus_Search_Book_Service : IBus_Search_Book_Service
{
    private readonly IBus_Search_Book_Repository _busRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IStopService _stopService;
    private readonly IMapper _mapper;
    private readonly ILogger<Bus_Search_Book_Service> _logger;

    public Bus_Search_Book_Service(
        IBus_Search_Book_Repository busRepository,
        ICustomerRepository customerRepository,
        IStopService stopService,
        IMapper mapper,
        ILogger<Bus_Search_Book_Service> logger
    )
    {
        _busRepository = busRepository;
        _customerRepository = customerRepository;
        _stopService = stopService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<BusSearchResponseDto>>> SearchBusesAsync(
        BusSearchRequestDto request
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.BusSearchStarted,
                request.Source,
                request.Destination,
                request.TravelDate
            );

            if (request.TravelDate.Date < DateTime.Today)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<List<BusSearchResponseDto>>(
                    Error.Failure(
                        "BusSearch.InvalidDate",
                        MagicStrings.ErrorMessages.TravelDateInvalid
                    )
                );
            }

            // Validate source place exists
            var sourceValidation = await _stopService.ValidatePlaceExistsAsync(request.Source.Trim());
            if (sourceValidation.IsFailure || !sourceValidation.Value)
            {
                _logger.LogWarning(MagicStrings.LogMessages.PlaceValidationFailed, "Invalid source place");
                return Result.Failure<List<BusSearchResponseDto>>(
                    Error.Failure(
                        "BusSearch.InvalidSource",
                        MagicStrings.ErrorMessages.InvalidPlace
                    )
                );
            }

            // Validate destination place exists
            var destinationValidation = await _stopService.ValidatePlaceExistsAsync(request.Destination.Trim());
            if (destinationValidation.IsFailure || !destinationValidation.Value)
            {
                _logger.LogWarning(MagicStrings.LogMessages.PlaceValidationFailed, "Invalid destination place");
                return Result.Failure<List<BusSearchResponseDto>>(
                    Error.Failure(
                        "BusSearch.InvalidDestination",
                        MagicStrings.ErrorMessages.InvalidPlace
                    )
                );
            }

            var result = await _busRepository.SearchBusesAsync(
                request.Source.Trim(),
                request.Destination.Trim(),
                request.TravelDate
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.BusSearchFailed,
                    result.Error.Description
                );
                return Result.Failure<List<BusSearchResponseDto>>(result.Error);
            }

            var dtos = _mapper.Map<List<BusSearchResponseDto>>(result.Value);
            _logger.LogInformation(MagicStrings.LogMessages.BusSearchCompleted, dtos.Count);
            return Result.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BusSearchFailed, ex.Message);
            return Result.Failure<List<BusSearchResponseDto>>(
                Error.Failure(
                    "BusSearch.UnexpectedError",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<SeatLayoutResponseDto>> GetSeatLayoutAsync(
        SeatLayoutRequestDto request
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutStarted,
                request.ScheduleId,
                request.TravelDate
            );

            if (request.TravelDate.Date < DateTime.Today)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<SeatLayoutResponseDto>(
                    Error.Failure(
                        "SeatLayout.InvalidDate",
                        MagicStrings.ErrorMessages.TravelDateInvalid
                    )
                );
            }

            var result = await _busRepository.GetSeatLayoutAsync(
                request.ScheduleId,
                request.TravelDate
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutFailed,
                    result.Error.Description
                );
                return Result.Failure<SeatLayoutResponseDto>(result.Error);
            }
            var seatLayoutResponseDto = _mapper.Map<SeatLayoutResponseDto>(result.Value);
            seatLayoutResponseDto.ScheduleId = request.ScheduleId;
            seatLayoutResponseDto.TravelDate = request.TravelDate;

            if (result.Value.Any() && !string.IsNullOrEmpty(result.Value[0].CreatedBy))
            {
                var busInfo = result.Value[0].CreatedBy.Split('|');
                if (busInfo.Length == 6)
                {
                    seatLayoutResponseDto.Bus.BusName = busInfo[0];
                    seatLayoutResponseDto.Bus.BusType = (BusType)int.Parse(busInfo[1]);
                    seatLayoutResponseDto.Bus.BusAmenities = (BusAmenities)int.Parse(busInfo[2]);
                    seatLayoutResponseDto.Bus.BasePrice = decimal.Parse(busInfo[3]);
                    seatLayoutResponseDto.Bus.AvailableSeats = int.Parse(busInfo[4]);
                    seatLayoutResponseDto.Bus.BookedSeats = int.Parse(busInfo[5]);
                    seatLayoutResponseDto.Bus.TotalSeats =
                        seatLayoutResponseDto.Bus.AvailableSeats
                        + seatLayoutResponseDto.Bus.BookedSeats;

                    foreach (var seat in seatLayoutResponseDto.Seats)
                    {
                        seat.Price = CalculateSeatPrice(
                            seatLayoutResponseDto.Bus.BasePrice,
                            seatLayoutResponseDto.Bus.BusType,
                            seatLayoutResponseDto.Bus.BusAmenities,
                            seat.SeatType,
                            seat.PriceTier
                        );
                    }
                }
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutCompleted,
                seatLayoutResponseDto.Seats.Count
            );
            return Result.Success(seatLayoutResponseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.SeatLayoutFailed, ex.Message);
            return Result.Failure<SeatLayoutResponseDto>(
                Error.Failure(
                    "SeatLayout.UnexpectedError",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    private decimal CalculateSeatPrice(
        decimal basePrice,
        BusType busType,
        BusAmenities amenities,
        SeatType seatType,
        PriceTier priceTier
    )
    {
        var price = basePrice;

        price *= busType switch
        {
            BusType.AC => 1.2m,
            BusType.Sleeper => 1.5m,
            BusType.Volvo => 1.8m,
            BusType.Luxury => 2.0m,
            _ => 1.0m,
        };

        var amenityMultiplier = 1.0m;
        if (amenities.HasFlag(BusAmenities.WiFi))
            amenityMultiplier += 0.05m;
        if (amenities.HasFlag(BusAmenities.Charging))
            amenityMultiplier += 0.05m;
        if (amenities.HasFlag(BusAmenities.Meals))
            amenityMultiplier += 0.1m;
        if (amenities.HasFlag(BusAmenities.Entertainment))
            amenityMultiplier += 0.1m;
        price *= amenityMultiplier;

        price *= seatType switch
        {
            SeatType.SleeperLower => 1.3m,
            SeatType.SleeperUpper => 1.1m,
            SeatType.SemiSleeper => 1.2m,
            _ => 1.0m,
        };

        price *= priceTier switch
        {
            PriceTier.Premium => 1.2m,
            PriceTier.Luxury => 1.5m,
            _ => 1.0m,
        };

        return Math.Round(price, 2);
    }

    public async Task<Result<BookingResponseDto>> BookSeatsAsync(BookingRequestDto request)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.BookingStarted,
                request.ScheduleId,
                request.SeatNumbers.Count,
                request.CustomerId
            );

            if (request.SeatNumbers.Count != request.Passengers.Count)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<BookingResponseDto>(
                    Error.Failure(
                        "Booking.ValidationFailed",
                        MagicStrings.ErrorMessages.SeatPassengerMismatch
                    )
                );
            }

            if (request.TravelDate.Date < DateTime.Today)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<BookingResponseDto>(
                    Error.Failure(
                        "Booking.InvalidDate",
                        MagicStrings.ErrorMessages.TravelDateInvalid
                    )
                );
            }

            if (request.BoardingStopId == request.DroppingStopId)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<BookingResponseDto>(
                    Error.Failure(
                        "Booking.ValidationFailed",
                        MagicStrings.ErrorMessages.SameStopError
                    )
                );
            }

            var seatValidationResult = await _busRepository.ValidateSeatsAndStopsAsync(
                request.ScheduleId,
                request.TravelDate,
                request.SeatNumbers,
                request.BoardingStopId,
                request.DroppingStopId
            );

            if (seatValidationResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatValidationFailed,
                    seatValidationResult.Error.Description
                );
                return Result.Failure<BookingResponseDto>(seatValidationResult.Error);
            }

            var totalAmount = CalculateTotalBookingAmount(seatValidationResult.Value);

            // Book seats
            var passengers = request.Passengers.Select(p => (p.Name, p.Age, p.Gender)).ToList();

            var bookingResult = await _busRepository.BookSeatsAsync(
                request.ScheduleId,
                request.CustomerId,
                request.TravelDate,
                request.SeatNumbers,
                passengers,
                totalAmount,
                request.BoardingStopId,
                request.DroppingStopId
            );

            if (bookingResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.BookingFailed,
                    bookingResult.Error.Description
                );
                return Result.Failure<BookingResponseDto>(bookingResult.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.BookingCompleted,
                bookingResult.Value.PNRNo,
                bookingResult.Value.BookingId
            );
            var bookingResponse = _mapper.Map<BookingResponseDto>(bookingResult.Value);
            bookingResponse.ScheduleId = request.ScheduleId;
            bookingResponse.TravelDate = request.TravelDate;
            bookingResponse.TotalAmount = totalAmount;
            bookingResponse.SeatNumbers = request.SeatNumbers;
            bookingResponse.ReservationExpiryTime = bookingResult.Value.CreatedOn.AddMinutes(10);

            var busInfoResult = await _busRepository.GetBusInfoAsync(request.ScheduleId);
            if (busInfoResult.IsSuccess)
            {
                bookingResponse.BusName = busInfoResult.Value.BusName;
                bookingResponse.Route = busInfoResult.Value.Route;
            }

            var routeStopsResult = await _busRepository.GetRouteStopsAsync(request.ScheduleId);
            if (routeStopsResult.IsSuccess)
            {
                var boardingStop = routeStopsResult.Value.FirstOrDefault(rs =>
                    rs.RouteStopId == request.BoardingStopId
                );
                var droppingStop = routeStopsResult.Value.FirstOrDefault(rs =>
                    rs.RouteStopId == request.DroppingStopId
                );

                if (boardingStop != null)
                {
                    bookingResponse.BoardingPoint = boardingStop.Stop?.Name ?? "Unknown";
                    bookingResponse.BoardingTime = boardingStop.DepartureTime;
                }

                if (droppingStop != null)
                {
                    bookingResponse.DroppingPoint = droppingStop.Stop?.Name ?? "Unknown";
                    bookingResponse.DroppingTime = droppingStop.ArrivalTime;
                }
            }

            return Result.Success(bookingResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BookingFailed, ex.Message);
            return Result.Failure<BookingResponseDto>(
                Error.Failure("Booking.UnexpectedError", MagicStrings.ErrorMessages.UnexpectedError)
            );
        }
    }

    private decimal CalculateTotalBookingAmount(List<SeatLayoutDetail> selectedSeats)
    {
        decimal totalAmount = 0;

        foreach (var seat in selectedSeats)
        {
            var seatPrice = CalculateSeatPrice(
                seat.BasePrice,
                seat.BusTypeForPricing,
                seat.AmenitiesForPricing,
                seat.SeatType,
                seat.PriceTier
            );
            totalAmount += seatPrice;
        }

        return totalAmount;
    }

    public async Task<Result<List<RouteStopDto>>> GetRouteStopsAsync(int scheduleId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.RouteStopsStarted, scheduleId);

            var result = await _busRepository.GetRouteStopsAsync(scheduleId);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.RouteStopsFailed,
                    result.Error.Description
                );
                return Result.Failure<List<RouteStopDto>>(result.Error);
            }

            var routeStopDtos = _mapper.Map<List<RouteStopDto>>(result.Value);
            _logger.LogInformation(
                MagicStrings.LogMessages.RouteStopsCompleted,
                routeStopDtos.Count
            );
            return Result.Success(routeStopDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.RouteStopsFailed, ex.Message);
            return Result.Failure<List<RouteStopDto>>(
                Error.Failure(
                    "RouteStops.UnexpectedError",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<string>> ConfirmBookingAsync(BookingConfirmationDto request)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.BookingConfirmationStarted,
                request.BookingId
            );

            var result = await _busRepository.ConfirmBookingAsync(
                request.BookingId,
                request.PaymentReferenceId,
                request.IsPaymentSuccessful
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.BookingConfirmationFailed,
                    result.Error.Description
                );
                return Result.Failure<string>(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.BookingConfirmationCompleted,
                request.BookingId
            );
            return Result.Success(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BookingConfirmationFailed, ex.Message);
            return Result.Failure<string>(
                Error.Failure(
                    "BookingConfirmation.UnexpectedError",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<int>> ExpirePendingBookingsAsync()
    {
        try
        {
            var result = await _busRepository.ExpirePendingBookingsAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BookingExpiryFailed, ex.Message);
            return Result.Failure<int>(
                Error.Failure(
                    "BookingExpiry.UnexpectedError",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<List<BusSearchResponseDto>>> SearchBusesFilteredAsync(
        BusSearchFilterDto request
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.FilteredBusSearchStarted,
                request.Source,
                request.Destination,
                request.TravelDate
            );

            if (request.TravelDate.Date < DateTime.Today)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<List<BusSearchResponseDto>>(
                    Error.Failure(
                        "FilteredBusSearch.InvalidDate",
                        MagicStrings.ErrorMessages.TravelDateInvalid
                    )
                );
            }

            if (
                request.DepartureTimeFrom.HasValue
                && request.DepartureTimeTo.HasValue
                && request.DepartureTimeFrom > request.DepartureTimeTo
            )
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<List<BusSearchResponseDto>>(
                    Error.Failure(
                        "FilteredBusSearch.InvalidTimeRange",
                        MagicStrings.ErrorMessages.InvalidTimeRange
                    )
                );
            }

            if (
                request.MinPrice.HasValue
                && request.MaxPrice.HasValue
                && request.MinPrice > request.MaxPrice
            )
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<List<BusSearchResponseDto>>(
                    Error.Failure(
                        "FilteredBusSearch.InvalidPriceRange",
                        MagicStrings.ErrorMessages.InvalidPriceRange
                    )
                );
            }

            var result = await _busRepository.SearchBusesFilteredAsync(
                request.Source.Trim(),
                request.Destination.Trim(),
                request.TravelDate,
                request.BusTypes,
                request.Amenities,
                request.DepartureTimeFrom,
                request.DepartureTimeTo,
                request.MinPrice,
                request.MaxPrice,
                request.SortBy
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.FilteredBusSearchFailed,
                    result.Error.Description
                );
                return Result.Failure<List<BusSearchResponseDto>>(result.Error);
            }

            var dtos = _mapper.Map<List<BusSearchResponseDto>>(result.Value);
            _logger.LogInformation(MagicStrings.LogMessages.FilteredBusSearchCompleted, dtos.Count);
            return Result.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.FilteredBusSearchFailed, ex.Message);
            return Result.Failure<List<BusSearchResponseDto>>(
                Error.Failure(
                    "FilteredBusSearch.UnexpectedError",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<BookingEntity>> GetBookingDetailsAsync(int bookingId)
    {
        try
        {
            _logger.LogInformation("Getting booking details for BookingId: {BookingId}", bookingId);

            var result = await _busRepository.GetBookingDetailsAsync(bookingId);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get booking details: {Error}", result.Error.Description);
                return Result.Failure<BookingEntity>(result.Error);
            }

            _logger.LogInformation("Successfully retrieved booking details for BookingId: {BookingId}", bookingId);
            return Result.Success(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get booking details for BookingId: {BookingId}", bookingId);
            return Result.Failure<BookingEntity>(
                Error.Failure("GetBookingDetails.Failed", MagicStrings.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<CustomerEntity>> GetCustomerByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Getting customer for UserId: {UserId}", userId);

            var customer = await _customerRepository.GetCustomerProfileByUserIdAsync(userId);
            
            if (customer == null)
            {
                _logger.LogWarning("Customer not found for UserId: {UserId}", userId);
                return Result.Failure<CustomerEntity>(
                    Error.Failure("Customer.NotFound", "Customer profile not found")
                );
            }

            _logger.LogInformation("Successfully retrieved customer for UserId: {UserId}", userId);
            return Result.Success(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer for UserId: {UserId}", userId);
            return Result.Failure<CustomerEntity>(
                Error.Failure("GetCustomer.Failed", MagicStrings.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<List<PlaceAutocompleteResponseDto>>> GetPlaceAutocompleteAsync(PlaceAutocompleteRequestDto request)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.PlaceAutocompleteStarted, request.Query);

            var result = await _stopService.GetPlaceAutocompleteAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.PlaceAutocompleteFailed, result.Error.Description);
                return Result.Failure<List<PlaceAutocompleteResponseDto>>(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.PlaceAutocompleteCompleted, result.Value.Count);
            return Result.Success(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PlaceAutocompleteFailed, ex.Message);
            return Result.Failure<List<PlaceAutocompleteResponseDto>>(
                Error.Failure("PlaceAutocomplete.UnexpectedError", MagicStrings.ErrorMessages.UnexpectedError)
            );
        }
    }
}
