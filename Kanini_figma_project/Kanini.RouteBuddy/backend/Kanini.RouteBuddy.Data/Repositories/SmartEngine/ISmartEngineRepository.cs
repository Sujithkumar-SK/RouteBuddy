using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;
using BookingEntity = Kanini.RouteBuddy.Domain.Entities.Booking;

namespace Kanini.RouteBuddy.Data.Repositories.SmartEngine;

public interface ISmartEngineRepository
{
    Task<Result<List<BusSchedule>>> FindConnectingRoutesAsync(
        string source,
        string destination,
        DateTime travelDate,
        string toggle
    );
    Task<Result<BookingEntity>> BookConnectingRouteAsync(
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
