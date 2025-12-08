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
        CASE 
            WHEN b.Status = 3 AND rf.RefundStatus = 2 THEN 4  -- Cancelled and Refunded (add new enum value)
            WHEN b.Status = 3 AND rf.RefundStatus = 1 THEN 1  -- Cancelled but refund pending
            WHEN b.Status = 3 AND (rf.RefundStatus IS NULL OR rf.RefundStatus = 3) THEN 3  -- Cancelled, refund failed
            WHEN b.Status = 1 THEN 1  -- Pending booking, payment pending
            ELSE ISNULL(p.PaymentStatus, 1)  -- Use actual payment status or default to pending
        END AS PaymentStatus
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId
    INNER JOIN Users u ON c.UserId = u.UserId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    INNER JOIN Routes r ON sch.RouteId = r.RouteId
    LEFT JOIN Payments p ON b.BookingId = p.BookingId
    LEFT JOIN Cancellations can ON b.BookingId = can.BookingId
    LEFT JOIN Refunds rf ON p.PaymentId = rf.PaymentId
    ORDER BY b.BookedAt DESC
END