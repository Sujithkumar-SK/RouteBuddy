CREATE PROCEDURE [dbo].[sp_GetPaymentsByBookingId]
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.PaymentId,
        p.BookingId,
        p.Amount,
        p.PaymentMethod,
        p.PaymentStatus,
        p.PaymentDate,
        p.TransactionId,
        p.IsActive,
        p.CreatedBy,
        p.CreatedOn,
        p.UpdatedBy,
        p.UpdatedOn
    FROM Payments p
    WHERE p.BookingId = @BookingId
    ORDER BY p.CreatedOn DESC;
END