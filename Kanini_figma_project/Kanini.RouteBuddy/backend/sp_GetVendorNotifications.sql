-- Stored Procedure: Get Vendor Notifications
CREATE OR ALTER PROCEDURE sp_GetVendorNotifications
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return sample notifications (can be enhanced with actual notification system)
    SELECT 
        'Booking' AS Type,
        'New booking received for Route #' + CAST(ROW_NUMBER() OVER (ORDER BY NEWID()) AS NVARCHAR(10)) AS Message,
        DATEADD(HOUR, -ROW_NUMBER() OVER (ORDER BY NEWID()), GETDATE()) AS Time
    FROM (VALUES (1),(2),(3),(4),(5)) AS T(N)
    
    UNION ALL
    
    SELECT 
        'Maintenance' AS Type,
        'Bus maintenance due for Bus #' + CAST(b.BusId AS NVARCHAR(10)) AS Message,
        DATEADD(DAY, -1, GETDATE()) AS Time
    FROM Buses b
    WHERE b.VendorId = @VendorId 
    AND b.Status = 3 -- Maintenance
    AND b.IsActive = 1
    
    ORDER BY Time DESC;
END