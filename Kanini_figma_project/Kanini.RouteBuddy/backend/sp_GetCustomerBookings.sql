CREATE PROCEDURE [dbo].[sp_GetCustomerBookings]
    @CustomerId INT,
    @Status INT = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BookingId,
        b.PNRNo,
        b.TotalSeats,
        b.TotalAmount,
        b.TravelDate,
        b.Status,
        b.BookedAt,
        bus.BusName,
        CONCAT(r.Source, ' → ', r.Destination) AS Route,
        v.AgencyName AS VendorName,
        bs.DepartureTime,
        bs.ArrivalTime,
        ISNULL(p.PaymentMethod, 0) AS PaymentMethod,
        CASE WHEN p.PaymentId IS NOT NULL AND p.IsActive = 1 THEN 1 ELSE 0 END AS IsPaymentCompleted
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId AND c.IsActive = 1
    INNER JOIN BookingSegments bseg ON b.BookingId = bseg.BookingId
    INNER JOIN BusSchedules bs ON bseg.ScheduleId = bs.ScheduleId AND bs.IsActive = 1
    INNER JOIN Buses bus ON bs.BusId = bus.BusId AND bus.IsActive = 1
    INNER JOIN Routes r ON bs.RouteId = r.RouteId AND r.IsActive = 1
    INNER JOIN Vendors v ON bus.VendorId = v.VendorId
    LEFT JOIN Payments p ON b.BookingId = p.BookingId AND p.IsActive = 1
    WHERE b.CustomerId = @CustomerId
        AND b.IsActive = 1
        AND (@Status IS NULL OR b.Status = @Status)
        AND (@FromDate IS NULL OR b.TravelDate >= @FromDate)
        AND (@ToDate IS NULL OR b.TravelDate <= @ToDate)
    ORDER BY b.BookedAt DESC;
END