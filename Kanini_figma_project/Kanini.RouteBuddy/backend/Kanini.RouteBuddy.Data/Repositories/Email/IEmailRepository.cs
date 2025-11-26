using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.Email;

public interface IEmailRepository
{
    Task<BookingEmailData?> GetBookingDetailsForEmailAsync(int bookingId);
    Task<ConnectingBookingEmailData?> GetConnectingBookingDetailsForEmailAsync(int bookingId);
}

public class BookingEmailData
{
    public int BookingId { get; set; }
    public string PNRNo { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime TravelDate { get; set; }
    public DateTime BookedAt { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public int BusType { get; set; }
    public string RegistrationNo { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public int PaymentMethod { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string BoardingStopName { get; set; } = string.Empty;
    public string BoardingStopLandmark { get; set; } = string.Empty;
    public string DroppingStopName { get; set; } = string.Empty;
    public string DroppingStopLandmark { get; set; } = string.Empty;
    public List<PassengerEmailData> Passengers { get; set; } = new();
}

public class PassengerEmailData
{
    public string SeatNumber { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public int PassengerAge { get; set; }
    public int PassengerGender { get; set; }
    public int SeatType { get; set; }
    public int SeatPosition { get; set; }
    public int SegmentOrder { get; set; }
}

public class ConnectingBookingEmailData
{
    public int BookingId { get; set; }
    public string PNRNo { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime TravelDate { get; set; }
    public DateTime BookedAt { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string OverallSource { get; set; } = string.Empty;
    public string OverallDestination { get; set; } = string.Empty;
    public int PaymentMethod { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public List<SegmentEmailData> Segments { get; set; } = new();
}

public class SegmentEmailData
{
    public int SegmentOrder { get; set; }
    public string BusName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public decimal SegmentAmount { get; set; }
    public List<string> SeatNumbers { get; set; } = new();
}
