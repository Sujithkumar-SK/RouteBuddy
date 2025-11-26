CREATE PROCEDURE sp_GetBookingForCancellation
    @BookingId INT,
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BookingId,
        b.CustomerId,
        b.PNRNo,
        b.TotalAmount,
        b.TravelDate,
        b.BookedAt,
        b.Status,
        p.PaymentMethod,
        p.TransactionId,
        DATEDIFF(HOUR, GETUTCDATE(), b.TravelDate) as HoursUntilTravel
    FROM Bookings b
    LEFT JOIN Payments p ON b.BookingId = p.BookingId
    WHERE b.BookingId = @BookingId 
        AND b.CustomerId = @CustomerId 
        AND b.IsActive = 1
        AND b.Status IN (1, 2); -- Pending or Confirmed only
END