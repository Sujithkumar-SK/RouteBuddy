using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Data.Models;

/// <summary>
/// Data transfer model to hold booking entity with additional details from stored procedure
/// This is NOT a domain entity and will NOT create a database table
/// </summary>
public class BookingWithDetails
{
    public Booking Booking { get; set; } = null!;
    public string BusName { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public bool IsPaymentCompleted { get; set; }
}