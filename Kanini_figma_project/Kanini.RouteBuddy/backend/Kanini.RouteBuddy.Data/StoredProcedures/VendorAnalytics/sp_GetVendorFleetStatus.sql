ALTER PROCEDURE [dbo].[sp_GetVendorFleetStatus]
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Debug: Show actual status values
    -- SELECT BusId, Status, IsActive FROM Buses WHERE VendorId = @VendorId;
    
    SELECT 
        COUNT(*) as TotalBuses,
        COUNT(CASE WHEN b.Status = 1 AND b.IsActive = 1 THEN 1 END) as ActiveBuses,
        COUNT(CASE WHEN b.Status = 3 THEN 1 END) as MaintenanceBuses
    FROM Buses b
    WHERE b.VendorId = @VendorId;
END