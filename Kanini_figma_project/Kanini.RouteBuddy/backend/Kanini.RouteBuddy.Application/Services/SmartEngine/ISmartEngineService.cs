using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.SmartEnigne;

public interface ISmartEngineService
{
    Task<Result<List<ConnectingRouteResponseDto>>> FindConnectingRoutesAsync(
        ConnectingSearchRequestDto request
    );
    Task<Result<ConnectingBookingResponseDto>> BookConnectingRouteAsync(
        ConnectingBookingRequestDto request
    );
    Task<Result<string>> ConfirmConnectingBookingAsync(BookingConfirmationDto request);
}
