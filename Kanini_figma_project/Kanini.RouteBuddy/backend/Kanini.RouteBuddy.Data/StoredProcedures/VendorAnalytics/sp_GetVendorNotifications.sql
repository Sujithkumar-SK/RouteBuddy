ALTER PROCEDURE [dbo].[sp_GetVendorNotifications]
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 10
        'Booking' as Type,
        CONCAT('New booking for ', r.Source, ' to ', r.Destination) as Message,
        b.BookedAt as Time
    FROM Bookings b
    INNER JOIN BookingSegments bseg ON b.BookingId = bseg.BookingId
    INNER JOIN BusSchedules bs ON bseg.ScheduleId = bs.ScheduleId
    INNER JOIN Buses bus ON bs.BusId = bus.BusId
    INNER JOIN Routes r ON bs.RouteId = r.RouteId
    WHERE bus.VendorId = @VendorId 
    AND b.BookedAt >= DATEADD(DAY, -30, GETDATE())
    ORDER BY b.BookedAt DESC;
END