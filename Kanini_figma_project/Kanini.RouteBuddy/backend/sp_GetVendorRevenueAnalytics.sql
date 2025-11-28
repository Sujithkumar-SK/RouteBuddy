-- Stored Procedure: Get Vendor Revenue Analytics
CREATE OR ALTER PROCEDURE sp_GetVendorRevenueAnalytics
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalRevenue DECIMAL(18,2) = 0;
    DECLARE @MonthlyRevenue DECIMAL(18,2) = 0;
    DECLARE @WeeklyRevenue DECIMAL(18,2) = 0;
    
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
    
    -- Get monthly revenue (current month)
    SELECT @MonthlyRevenue = ISNULL(SUM(p.Amount), 0)
    FROM Payments p
    INNER JOIN Bookings b ON p.BookingId = b.BookingId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND p.PaymentStatus = 2 -- Success
    AND b.IsActive = 1
    AND MONTH(p.CreatedOn) = MONTH(GETDATE())
    AND YEAR(p.CreatedOn) = YEAR(GETDATE());
    
    -- Get weekly revenue (last 7 days)
    SELECT @WeeklyRevenue = ISNULL(SUM(p.Amount), 0)
    FROM Payments p
    INNER JOIN Bookings b ON p.BookingId = b.BookingId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND p.PaymentStatus = 2 -- Success
    AND b.IsActive = 1
    AND p.CreatedOn >= DATEADD(DAY, -7, GETDATE());
    
    -- Return results
    SELECT 
        @TotalRevenue AS TotalRevenue,
        @MonthlyRevenue AS MonthlyRevenue,
        @WeeklyRevenue AS WeeklyRevenue;
END