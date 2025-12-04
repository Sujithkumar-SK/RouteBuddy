CREATE PROCEDURE [dbo].[sp_GetVendorRecentBookings]
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 10 
        b.BookingId,
        CONCAT(c.FirstName, ' ', c.LastName) as CustomerName,
        CONCAT(r.Source, ' to ', r.Destination) as Route,
        b.BookedAt as BookingDate,
        CASE b.Status 
            WHEN 0 THEN 'Pending'
            WHEN 1 THEN 'Confirmed' 
            WHEN 2 THEN 'Cancelled'
            WHEN 3 THEN 'Completed'
            ELSE 'Unknown'
        END as Status
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    INNER JOIN Routes r ON sch.RouteId = r.RouteId
    WHERE bus.VendorId = @VendorId
    ORDER BY b.BookedAt DESC;
END