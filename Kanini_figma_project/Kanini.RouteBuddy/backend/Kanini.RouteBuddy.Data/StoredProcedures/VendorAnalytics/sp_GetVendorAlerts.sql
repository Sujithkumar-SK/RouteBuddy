ALTER PROCEDURE [dbo].[sp_GetVendorAlerts]
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Maintenance alerts
    SELECT 
        b.BusId as AlertId,
        'Maintenance' as Type,
        CONCAT('Bus ', b.RegistrationNo, ' is under maintenance') as Message,
        'High' as Severity,
        b.UpdatedOn as CreatedAt
    FROM Buses b
    WHERE b.VendorId = @VendorId AND b.Status = 3
    
    UNION ALL
    
    -- Pending approval alerts
    SELECT 
        b.BusId + 1000 as AlertId,
        'Approval' as Type,
        CONCAT('Bus ', b.RegistrationNo, ' pending approval') as Message,
        'Medium' as Severity,
        b.CreatedOn as CreatedAt
    FROM Buses b
    WHERE b.VendorId = @VendorId AND b.Status = 0
    
    UNION ALL
    
    -- Recent booking alerts
    SELECT 
        b.BookingId + 2000 as AlertId,
        'Booking' as Type,
        CONCAT('New booking received - PNR: ', b.PNRNo) as Message,
        'Low' as Severity,
        b.BookedAt as CreatedAt
    FROM Bookings b
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND b.BookedAt >= DATEADD(DAY, -7, GETDATE())
    
    ORDER BY CreatedAt DESC;
END