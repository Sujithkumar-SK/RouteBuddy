ALTER PROCEDURE sp_GetVendorDashboardSummary
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        -- Bus counts
        ISNULL((SELECT COUNT(*) FROM Buses WHERE VendorId = @VendorId), 0) AS TotalBuses,
        ISNULL((SELECT COUNT(*) FROM Buses WHERE VendorId = @VendorId AND Status = 1 AND IsActive = 1), 0) AS ActiveBuses,
        ISNULL((SELECT COUNT(*) FROM Buses WHERE VendorId = @VendorId AND Status = 0), 0) AS PendingBuses,
        
        -- Route count
        ISNULL((SELECT COUNT(DISTINCT bs.RouteId) 
                FROM BusSchedules bs
                INNER JOIN Buses b ON bs.BusId = b.BusId
                WHERE b.VendorId = @VendorId), 0) AS TotalRoutes,
        
        -- Schedule counts
        ISNULL((SELECT COUNT(*) 
                FROM BusSchedules bs
                INNER JOIN Buses b ON bs.BusId = b.BusId
                WHERE b.VendorId = @VendorId AND bs.IsActive = 1), 0) AS TotalSchedules,
        
        ISNULL((SELECT COUNT(*) 
                FROM BusSchedules bs
                INNER JOIN Buses b ON bs.BusId = b.BusId
                WHERE b.VendorId = @VendorId AND bs.IsActive = 1 
                AND bs.TravelDate >= CAST(GETDATE() AS DATE)), 0) AS UpcomingSchedules,
        
        -- Vendor status
        ISNULL((SELECT 
                    CASE Status 
                        WHEN 0 THEN 'Pending Approval'
                        WHEN 1 THEN 'Active'
                        WHEN 2 THEN 'Suspended'
                        WHEN 3 THEN 'Rejected'
                        ELSE 'Unknown'
                    END
                FROM Vendors 
                WHERE VendorId = @VendorId), 'Unknown') AS VendorStatus;
END