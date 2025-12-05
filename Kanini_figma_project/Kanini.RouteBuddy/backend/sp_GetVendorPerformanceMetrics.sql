-- Stored Procedure: Get Vendor Performance Metrics
CREATE OR ALTER PROCEDURE sp_GetVendorPerformanceMetrics
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @MonthlyBookings INT = 0;
    DECLARE @OnTimePerformance DECIMAL(5,2) = 0;
    
    -- Get monthly bookings count
    SELECT @MonthlyBookings = COUNT(*)
    FROM Bookings b
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND b.Status = 2 -- Confirmed
    AND b.IsActive = 1
    AND MONTH(b.CreatedOn) = MONTH(GETDATE())
    AND YEAR(b.CreatedOn) = YEAR(GETDATE());
    
    -- Calculate on-time performance (assuming 95% for now as we don't have actual tracking)
    SET @OnTimePerformance = 95.0;
    
    -- Return results
    SELECT 
        @MonthlyBookings AS MonthlyBookings,
        @OnTimePerformance AS OnTimePerformance;
END