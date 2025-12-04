CREATE PROCEDURE sp_GetAllBookings
AS
BEGIN
    SELECT 
        b.BookingId,
        b.PNRNo,
        b.TotalSeats,
        b.TotalAmount,
        b.TravelDate,
        b.Status,
        b.BookedAt,
        CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
        u.Email AS CustomerEmail,
        u.Phone AS CustomerPhone,
        bus.BusName,
        CONCAT(r.Source, ' - ', r.Destination) AS Route,
        p.PaymentStatus
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId
    INNER JOIN Users u ON c.UserId = u.UserId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    INNER JOIN Routes r ON sch.RouteId = r.RouteId
    LEFT JOIN Payments p ON b.BookingId = p.BookingId
    ORDER BY b.BookedAt DESC
END