using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.SmartEngine;

public interface ISmartEngineRepository
{
    Task<Result<List<BusSchedule>>> FindConnectingRoutesAsync(
        string source,
        string destination,
        DateTime travelDate,
        string toggle
    );
    Task<Result<Booking>> BookConnectingRouteAsync(
        int customerId,
        DateTime travelDate,
        decimal totalAmount,
        string segmentData
    );
    Task<Result<string>> ConfirmConnectingBookingAsync(
        int bookingId,
        string paymentReferenceId,
        bool isPaymentSuccessful
    );
}
