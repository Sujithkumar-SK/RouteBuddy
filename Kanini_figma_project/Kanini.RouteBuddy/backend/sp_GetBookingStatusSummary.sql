CREATE PROCEDURE sp_GetBookingStatusSummary
AS
BEGIN
    SELECT 
        SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS PendingBookings,
        SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) AS ConfirmedBookings,
        SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) AS CancelledBookings,
        COUNT(*) AS TotalBookings,
        SUM(CASE WHEN Status = 2 THEN TotalAmount ELSE 0 END) AS TotalRevenue,
        (
            SELECT COUNT(*) 
            FROM Bookings b 
            INNER JOIN Payments p ON b.BookingId = p.BookingId 
            WHERE p.PaymentStatus = 2
        ) AS SuccessfulPayments,
        (
            SELECT COUNT(*) 
            FROM Bookings b 
            INNER JOIN Payments p ON b.BookingId = p.BookingId 
            WHERE p.PaymentStatus = 1
        ) AS PendingPayments
    FROM Bookings
END