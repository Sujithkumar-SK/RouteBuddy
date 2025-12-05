CREATE PROCEDURE [dbo].[sp_GetPaymentById]
    @PaymentId INT
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
    WHERE p.PaymentId = @PaymentId
        AND p.IsActive = 1;
END