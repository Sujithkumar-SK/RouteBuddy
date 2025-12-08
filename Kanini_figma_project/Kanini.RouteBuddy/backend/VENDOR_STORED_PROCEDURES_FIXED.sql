-- =============================================
-- VENDOR DASHBOARD STORED PROCEDURES - FIXED
-- =============================================

-- 1. Get Vendor Dashboard Summary
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
    
    -- Get route count (from schedules)
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
GO

-- 2. Get Vendor Revenue Analytics
CREATE OR ALTER PROCEDURE sp_GetVendorRevenueAnalytics
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalRevenue DECIMAL(18,2) = 0;
    DECLARE @MonthlyRevenue DECIMAL(18,2) = 0;
    DECLARE @WeeklyRevenue DECIMAL(18,2) = 0;
    
    -- Get total revenue
    SELECT @TotalRevenue = ISNULL(SUM(p.Amount), 0)
    FROM Payments p
    INNER JOIN Bookings b ON p.BookingId = b.BookingId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND p.Status = 2 -- Success
    AND b.IsActive = 1;
    
    -- Get monthly revenue (current month)
    SELECT @MonthlyRevenue = ISNULL(SUM(p.Amount), 0)
    FROM Payments p
    INNER JOIN Bookings b ON p.BookingId = b.BookingId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND p.Status = 2 -- Success
    AND b.IsActive = 1
    AND MONTH(p.CreatedOn) = MONTH(GETDATE())
    AND YEAR(p.CreatedOn) = YEAR(GETDATE());
    
    -- Get weekly revenue (last 7 days)
    SELECT @WeeklyRevenue = ISNULL(SUM(p.Amount), 0)
    FROM Payments p
    INNER JOIN Bookings b ON p.BookingId = b.BookingId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND p.Status = 2 -- Success
    AND b.IsActive = 1
    AND p.CreatedOn >= DATEADD(DAY, -7, GETDATE());
    
    -- Return results
    SELECT 
        @TotalRevenue AS TotalRevenue,
        @MonthlyRevenue AS MonthlyRevenue,
        @WeeklyRevenue AS WeeklyRevenue;
END
GO

-- 3. Get Vendor Performance Metrics
CREATE OR ALTER PROCEDURE sp_GetVendorPerformanceMetrics
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @MonthlyBookings INT = 0;
    DECLARE @OnTimePerformance DECIMAL(5,2) = 0;
    
    -- Get monthly bookings count
    SELECT @MonthlyBookings = COUNT(*)
    FROM Bookings b
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND b.Status = 2 -- Confirmed
    AND b.IsActive = 1
    AND MONTH(b.CreatedOn) = MONTH(GETDATE())
    AND YEAR(b.CreatedOn) = YEAR(GETDATE());
    
    -- Calculate on-time performance (95% default)
    SET @OnTimePerformance = 95.0;
    
    -- Return results
    SELECT 
        @MonthlyBookings AS MonthlyBookings,
        @OnTimePerformance AS OnTimePerformance;
END
GO

-- 4. Get Vendor Fleet Status
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
GO

-- 5. Get Vendor Quick Stats
CREATE OR ALTER PROCEDURE sp_GetVendorQuickStats
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalBookings INT = 0;
    DECLARE @ActiveRoutes INT = 0;
    DECLARE @TotalRevenue DECIMAL(18,2) = 0;
    
    -- Get total bookings
    SELECT @TotalBookings = COUNT(*)
    FROM Bookings b
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND b.Status = 2 -- Confirmed
    AND b.IsActive = 1;
    
    -- Get active routes
    SELECT @ActiveRoutes = COUNT(DISTINCT bs.RouteId)
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    WHERE b.VendorId = @VendorId 
    AND b.IsActive = 1 
    AND b.Status = 1 -- Active
    AND bs.IsActive = 1;
    
    -- Get total revenue
    SELECT @TotalRevenue = ISNULL(SUM(p.Amount), 0)
    FROM Payments p
    INNER JOIN Bookings b ON p.BookingId = b.BookingId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND p.Status = 2 -- Success
    AND b.IsActive = 1;
    
    -- Return results
    SELECT 
        @TotalBookings AS TotalBookings,
        @ActiveRoutes AS ActiveRoutes,
        @TotalRevenue AS TotalRevenue;
END
GO

-- 6. Get Vendor Recent Bookings
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
GO

-- 7. Get Vendor Notifications
CREATE OR ALTER PROCEDURE sp_GetVendorNotifications
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return sample notifications
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
GO

-- 8. Get Vendor Maintenance Schedule
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
GO

-- 9. Get Vendor Alerts
CREATE OR ALTER PROCEDURE sp_GetVendorAlerts
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return sample alerts
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
GO

-- 10. Get Vendor By ID
CREATE OR ALTER PROCEDURE sp_GetVendorById
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.VendorId,
        v.UserId,
        v.AgencyName,
        v.OwnerName,
        v.BusinessLicenseNumber,
        v.OfficeAddress,
        v.FleetSize,
        v.TaxRegistrationNumber,
        v.Status,
        v.IsActive,
        u.Email,
        u.Phone
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.VendorId = @VendorId
    AND v.IsActive = 1;
END
GO

-- 11. Check Vendor Exists by Email
CREATE OR ALTER PROCEDURE sp_CheckVendorExistsByEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE u.Email = @Email
    AND v.IsActive = 1;
END
GO

-- 12. Check Vendor Exists by ID
CREATE OR ALTER PROCEDURE sp_CheckVendorExistsById
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM Vendors
    WHERE VendorId = @VendorId
    AND IsActive = 1;
END
GO

PRINT 'All Vendor Stored Procedures Created Successfully!'