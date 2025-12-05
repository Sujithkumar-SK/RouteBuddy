-- Stored Procedure: Get Vendor Quick Stats
CREATE OR ALTER PROCEDURE sp_GetVendorQuickStats
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalBookings INT = 0;
    DECLARE @ActiveRoutes INT = 0;
    DECLARE @TotalRevenue DECIMAL(18,2) = 0;
    
    -- Get total bookings
    SELECT @TotalBookings = COUNT(*)
    FROM Bookings b
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND b.Status = 2 -- Confirmed
    AND b.IsActive = 1;
    
    -- Get active routes
    SELECT @ActiveRoutes = COUNT(DISTINCT bs.RouteId)
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    WHERE b.VendorId = @VendorId 
    AND b.IsActive = 1 
    AND b.Status = 1 -- Active
    AND bs.IsActive = 1;
    
    -- Get total revenue
    SELECT @TotalRevenue = ISNULL(SUM(p.Amount), 0)
    FROM Payments p
    INNER JOIN Bookings b ON p.BookingId = b.BookingId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND p.PaymentStatus = 2 -- Success
    AND b.IsActive = 1;
    
    -- Return results
    SELECT 
        @TotalBookings AS TotalBookings,
        @ActiveRoutes AS ActiveRoutes,
        @TotalRevenue AS TotalRevenue;
END