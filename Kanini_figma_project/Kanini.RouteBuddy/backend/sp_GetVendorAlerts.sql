-- Stored Procedure: Get Vendor Alerts
CREATE OR ALTER PROCEDURE sp_GetVendorAlerts
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return sample alerts (can be enhanced with actual alert system)
    SELECT 
        ROW_NUMBER() OVER (ORDER BY NEWID()) AS AlertId,
        'System' AS Type,
        'Bus #' + CAST(b.BusId AS NVARCHAR(10)) + ' requires immediate maintenance' AS Message,
        'High' AS Severity,
        DATEADD(HOUR, -2, GETDATE()) AS CreatedAt
    FROM Buses b
    WHERE b.VendorId = @VendorId 
    AND b.Status = 3 -- Maintenance
    AND b.IsActive = 1
    
    UNION ALL
    
    SELECT 
        ROW_NUMBER() OVER (ORDER BY NEWID()) + 100 AS AlertId,
        'Revenue' AS Type,
        'Monthly revenue target 80% achieved' AS Message,
        'Medium' AS Severity,
        DATEADD(DAY, -1, GETDATE()) AS CreatedAt
    WHERE EXISTS (SELECT 1 FROM Buses WHERE VendorId = @VendorId)
    
    ORDER BY CreatedAt DESC;
END