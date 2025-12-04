-- Stored Procedure: Get Vendor Maintenance Schedule
CREATE OR ALTER PROCEDURE sp_GetVendorMaintenanceSchedule
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.RegistrationNo AS BusNumber,
        'Regular Service' AS Type,
        DATEADD(DAY, 30, GETDATE()) AS DueDate,
        'Medium' AS Priority
    FROM Buses b
    WHERE b.VendorId = @VendorId 
    AND b.IsActive = 1
    AND b.Status = 1 -- Active buses that need maintenance
    
    UNION ALL
    
    SELECT 
        b.RegistrationNo AS BusNumber,
        'Engine Check' AS Type,
        DATEADD(DAY, 15, GETDATE()) AS DueDate,
        'High' AS Priority
    FROM Buses b
    WHERE b.VendorId = @VendorId 
    AND b.IsActive = 1
    AND b.Status = 3 -- Maintenance status
    
    ORDER BY DueDate ASC;
END