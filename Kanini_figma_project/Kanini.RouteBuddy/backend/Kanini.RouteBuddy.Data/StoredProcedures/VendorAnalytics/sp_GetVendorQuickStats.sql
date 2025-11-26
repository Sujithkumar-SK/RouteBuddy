CREATE PROCEDURE [dbo].[sp_GetVendorQuickStats]
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(DISTINCT b.BookingId) as TotalBookings,
        COUNT(DISTINCT bs.RouteId) as ActiveRoutes,
        COALESCE(SUM(b.TotalAmount), 0) as TotalRevenue
    FROM Bookings b
    INNER JOIN BookingSegments bseg ON b.BookingId = bseg.BookingId
    INNER JOIN BusSchedules bs ON bseg.ScheduleId = bs.ScheduleId
    INNER JOIN Buses bus ON bs.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId;
END