CREATE PROCEDURE [dbo].[sp_ExpirePendingBookings]
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ExpiredBookings TABLE (
        BookingId INT,
        TotalSeats INT,
        ScheduleId INT
    );
    
    -- Find expired pending bookings (older than 10 minutes)
    INSERT INTO @ExpiredBookings (BookingId, TotalSeats, ScheduleId)
    SELECT 
        b.BookingId,
        b.TotalSeats,
        bs.ScheduleId
    FROM Bookings b
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    WHERE b.Status = 1 -- Pending
        AND b.IsActive = 1
        AND DATEADD(MINUTE, 10, b.CreatedOn) < GETUTCDATE();
    
    -- Update expired bookings to cancelled
    UPDATE Bookings 
    SET Status = 3, -- Cancelled
        UpdatedBy = 'AutoExpiry',
        UpdatedOn = GETUTCDATE()
    WHERE BookingId IN (SELECT BookingId FROM @ExpiredBookings);
    
    -- Release seats back to schedules
    UPDATE BusSchedules 
    SET AvailableSeats = AvailableSeats + eb.TotalSeats,
        UpdatedBy = 'AutoExpiry',
        UpdatedOn = GETUTCDATE()
    FROM BusSchedules bs
    INNER JOIN @ExpiredBookings eb ON bs.ScheduleId = eb.ScheduleId;
    
    -- Return count of expired bookings
    SELECT COUNT(*) AS ExpiredCount FROM @ExpiredBookings;
END