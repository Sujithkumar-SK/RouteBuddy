-- Stored Procedure: Get Vendor Recent Bookings
CREATE OR ALTER PROCEDURE sp_GetVendorRecentBookings
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 10
        b.BookingId,
        ISNULL(c.FirstName + ' ' + c.LastName, 'Unknown Customer') AS CustomerName,
        'Route Booking' AS Route,
        b.CreatedOn AS BookingDate,
        CASE b.Status
            WHEN 1 THEN 'Pending'
            WHEN 2 THEN 'Confirmed'
            WHEN 3 THEN 'Cancelled'
            ELSE 'Unknown'
        END AS Status
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND b.IsActive = 1
    ORDER BY b.CreatedOn DESC;
END