CREATE PROCEDURE [dbo].[sp_ConfirmBooking]
    @BookingId INT,
    @PaymentReferenceId NVARCHAR(100),
    @IsPaymentSuccessful BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentStatus INT;
    DECLARE @CreatedOn DATETIME2;
    DECLARE @ExpiryTime DATETIME2;
    
    -- Get current booking status and creation time
    SELECT 
        @CurrentStatus = Status,
        @CreatedOn = CreatedOn
    FROM Bookings 
    WHERE BookingId = @BookingId AND IsActive = 1;
    
    -- Check if booking exists
    IF @CurrentStatus IS NULL
    BEGIN
        SELECT 'BOOKING_NOT_FOUND' AS Result;
        RETURN;
    END
    
    -- Check if already confirmed
    IF @CurrentStatus = 2 -- Confirmed
    BEGIN
        SELECT 'ALREADY_CONFIRMED' AS Result;
        RETURN;
    END
    
    -- Check if expired (10 minutes)
    SET @ExpiryTime = DATEADD(MINUTE, 10, @CreatedOn);
    IF GETUTCDATE() > @ExpiryTime
    BEGIN
        -- Update to cancelled status
        UPDATE Bookings 
        SET Status = 3, -- Cancelled
            UpdatedBy = 'System',
            UpdatedOn = GETUTCDATE()
        WHERE BookingId = @BookingId;
        
        SELECT 'BOOKING_EXPIRED' AS Result;
        RETURN;
    END
    
    -- Update booking status based on payment result
    IF @IsPaymentSuccessful = 1
    BEGIN
        UPDATE Bookings 
        SET Status = 2, -- Confirmed
            UpdatedBy = 'PaymentSystem',
            UpdatedOn = GETUTCDATE()
        WHERE BookingId = @BookingId;
        
        -- Update payment record if exists
        UPDATE Payments 
        SET PaymentStatus = 2, -- Success
            TransactionId = @PaymentReferenceId,
            PaymentDate = GETUTCDATE(),
            UpdatedBy = 'PaymentSystem',
            UpdatedOn = GETUTCDATE()
        WHERE BookingId = @BookingId;
        
        SELECT 'CONFIRMED' AS Result;
    END
    ELSE
    BEGIN
        UPDATE Bookings 
        SET Status = 3, -- Cancelled
            UpdatedBy = 'PaymentSystem',
            UpdatedOn = GETUTCDATE()
        WHERE BookingId = @BookingId;
        
        -- Release seats back to schedule
        DECLARE @SeatsToRelease INT;
        SELECT @SeatsToRelease = TotalSeats FROM Bookings WHERE BookingId = @BookingId;
        
        UPDATE BusSchedules 
        SET AvailableSeats = AvailableSeats + @SeatsToRelease,
            UpdatedBy = 'PaymentSystem',
            UpdatedOn = GETUTCDATE()
        WHERE ScheduleId IN (
            SELECT ScheduleId FROM BookingSegments WHERE BookingId = @BookingId
        );
        
        SELECT 'PAYMENT_FAILED' AS Result;
    END
END