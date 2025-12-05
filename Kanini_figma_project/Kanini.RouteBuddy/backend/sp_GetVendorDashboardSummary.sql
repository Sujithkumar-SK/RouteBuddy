-- Stored Procedure: Get Vendor Dashboard Summary
CREATE OR ALTER PROCEDURE sp_GetVendorDashboardSummary
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalBuses INT = 0;
    DECLARE @ActiveBuses INT = 0;
    DECLARE @PendingBuses INT = 0;
    DECLARE @TotalRoutes INT = 0;
    DECLARE @TotalSchedules INT = 0;
    DECLARE @UpcomingSchedules INT = 0;
    DECLARE @VendorStatus NVARCHAR(50) = '';
    
    -- Get bus counts
    SELECT 
        @TotalBuses = COUNT(*),
        @ActiveBuses = SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END),
        @PendingBuses = SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END)
    FROM Buses 
    WHERE VendorId = @VendorId AND IsActive = 1;
    
    -- Get route count
    SELECT @TotalRoutes = COUNT(DISTINCT bs.RouteId)
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    WHERE b.VendorId = @VendorId AND b.IsActive = 1 AND bs.IsActive = 1;
    
    -- Get schedule counts
    SELECT 
        @TotalSchedules = COUNT(*),
        @UpcomingSchedules = SUM(CASE WHEN TravelDate >= CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END)
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    WHERE b.VendorId = @VendorId AND bs.IsActive = 1;
    
    -- Get vendor status
    SELECT @VendorStatus = 
        CASE Status
            WHEN 0 THEN 'PendingApproval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'Suspended'
            WHEN 4 THEN 'Rejected'
            ELSE 'Unknown'
        END
    FROM Vendors 
    WHERE VendorId = @VendorId;
    
    -- Return results
    SELECT 
        @TotalBuses AS TotalBuses,
        @ActiveBuses AS ActiveBuses,
        @PendingBuses AS PendingBuses,
        @TotalRoutes AS TotalRoutes,
        @TotalSchedules AS TotalSchedules,
        @UpcomingSchedules AS UpcomingSchedules,
        @VendorStatus AS VendorStatus;
END