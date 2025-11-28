-- Stored Procedure: Get Vendor Fleet Status
CREATE OR ALTER PROCEDURE sp_GetVendorFleetStatus
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalBuses INT = 0;
    DECLARE @ActiveBuses INT = 0;
    DECLARE @MaintenanceBuses INT = 0;
    
    -- Get fleet status counts
    SELECT 
        @TotalBuses = COUNT(*),
        @ActiveBuses = SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END),
        @MaintenanceBuses = SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END)
    FROM Buses 
    WHERE VendorId = @VendorId AND IsActive = 1;
    
    -- Return results
    SELECT 
        @TotalBuses AS TotalBuses,
        @ActiveBuses AS ActiveBuses,
        @MaintenanceBuses AS MaintenanceBuses;
END