CREATE PROCEDURE sp_GetVendorDashboardSummary
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Bus counts
    SELECT 
        COUNT(*) AS TotalBuses,
        SUM(CASE WHEN Status = 1 AND IsActive = 1 THEN 1 ELSE 0 END) AS ActiveBuses,
        SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) AS PendingBuses
    FROM Buses 
    WHERE VendorId = @VendorId;
    
    -- Route count
    SELECT COUNT(DISTINCT bs.RouteId) AS TotalRoutes
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    WHERE b.VendorId = @VendorId;
    
    -- Schedule counts
    SELECT 
        COUNT(*) AS TotalSchedules,
        SUM(CASE WHEN TravelDate >= CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS UpcomingSchedules
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    WHERE b.VendorId = @VendorId AND bs.IsActive = 1;
    
    -- Vendor status
    SELECT 
        CASE Status 
            WHEN 0 THEN 'Pending Approval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Suspended'
            WHEN 3 THEN 'Rejected'
            ELSE 'Unknown'
        END AS VendorStatus
    FROM Vendors 
    WHERE VendorId = @VendorId;
END