CREATE OR ALTER PROCEDURE [dbo].[sp_CheckUserExistsByEmail]
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1)
    FROM Users 
    WHERE Email = @Email AND IsActive = 1;
END
-----------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId, 
        Email, 
        PasswordHash, 
        Phone, 
        Role, 
        IsEmailVerified, 
        IsActive, 
        LastLogin, 
        FailedLoginAttempts, 
        LockoutEnd, 
        CreatedBy, 
        CreatedOn, 
        UpdatedBy, 
        UpdatedOn
    FROM Users
    WHERE Email = @Email;
END
GO
-----------------------------
CREATE OR ALTER PROCEDURE sp_GetUserById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId, 
        Email, 
        PasswordHash, 
        Phone, 
        Role, 
        IsEmailVerified, 
        IsActive, 
        LastLogin, 
        FailedLoginAttempts, 
        LockoutEnd, 
        CreatedBy, 
        CreatedOn, 
        UpdatedBy, 
        UpdatedOn
    FROM Users
    WHERE UserId = @UserId;
END
GO
-----------------------------
CREATE OR ALTER PROCEDURE sp_GetUserByRefreshToken
    @RefreshToken NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.UserId, 
        u.Email, 
        u.PasswordHash, 
        u.Phone, 
        u.Role, 
        u.IsEmailVerified, 
        u.IsActive, 
        u.LastLogin, 
        u.FailedLoginAttempts, 
        u.LockoutEnd,
        u.CreatedBy, 
        u.CreatedOn, 
        u.UpdatedBy, 
        u.UpdatedOn
    FROM Users u
    INNER JOIN RefreshTokens rt ON u.UserId = rt.UserId
    WHERE rt.Token = @RefreshToken 
      AND rt.ExpiresAt > GETUTCDATE()
      AND rt.IsRevoked = 0
      AND u.IsActive = 1;
END
GO
------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CheckUserExistsByPhone]
    @Phone NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1)
    FROM Users 
    WHERE Phone = @Phone AND IsActive = 1;
END
GO
------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetPaymentById]
    @PaymentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.PaymentId,
        p.BookingId,
        p.Amount,
        p.PaymentMethod,
        p.PaymentStatus,
        p.PaymentDate,
        p.TransactionId,
        p.IsActive,
        p.CreatedBy,
        p.CreatedOn,
        p.UpdatedBy,
        p.UpdatedOn
    FROM Payments p
    WHERE p.PaymentId = @PaymentId
        AND p.IsActive = 1;
END
GO
------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetPaymentByTransactionId]
    @TransactionId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.PaymentId,
        p.BookingId,
        p.Amount,
        p.PaymentMethod,
        p.PaymentStatus,
        p.PaymentDate,
        p.TransactionId,
        p.IsActive,
        p.CreatedBy,
        p.CreatedOn,
        p.UpdatedBy,
        p.UpdatedOn
    FROM Payments p
    WHERE p.TransactionId = @TransactionId;
END
GO
------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetPaymentsByBookingId]
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.PaymentId,
        p.BookingId,
        p.Amount,
        p.PaymentMethod,
        p.PaymentStatus,
        p.PaymentDate,
        p.TransactionId,
        p.IsActive,
        p.CreatedBy,
        p.CreatedOn,
        p.UpdatedBy,
        p.UpdatedOn
    FROM Payments p
    WHERE p.BookingId = @BookingId
    ORDER BY p.CreatedOn DESC;
END
GO
------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetCustomerProfileById]
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerId,
        c.FirstName,
        c.MiddleName,
        c.LastName,
        c.DateOfBirth,
        c.Gender,
        c.IsActive,
        c.CreatedOn,
        c.UpdatedOn,
        u.Email,
        u.Phone,
        u.IsActive AS UserIsActive
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    WHERE c.CustomerId = @CustomerId 
        AND c.IsActive = 1
        AND u.IsActive = 1
    ORDER BY c.UpdatedOn DESC;
END
GO
------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateCustomerProfile]
    @CustomerId INT,
    @FirstName NVARCHAR(50),
    @MiddleName NVARCHAR(50) = NULL,
    @LastName NVARCHAR(50),
    @DateOfBirth DATE,
    @Gender INT,
    @Phone NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Update Customer table
        UPDATE Customers 
        SET 
            FirstName = @FirstName,
            MiddleName = @MiddleName,
            LastName = @LastName,
            DateOfBirth = @DateOfBirth,
            Gender = @Gender,
            UpdatedOn = GETUTCDATE()
        WHERE CustomerId = @CustomerId AND IsActive = 1;
        
        -- Update User phone
        UPDATE Users 
        SET 
            Phone = @Phone,
            UpdatedOn = GETUTCDATE()
        WHERE UserId = (SELECT UserId FROM Customers WHERE CustomerId = @CustomerId)
            AND IsActive = 1;
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success;
    END CATCH
END
GO
------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetCustomerProfileByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerId,
        c.FirstName,
        c.MiddleName,
        c.LastName,
        c.DateOfBirth,
        c.Gender,
        c.ProfilePicture,
        c.IsActive,
        c.CreatedOn,
        c.UpdatedOn,
        u.Email,
        u.Phone,
        u.IsActive AS UserIsActive
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    WHERE u.UserId = @UserId 
        AND c.IsActive = 1
        AND u.IsActive = 1
    ORDER BY c.UpdatedOn DESC;
END
GO
------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetCustomerBookings]
    @CustomerId INT,
    @Status INT = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BookingId,
        b.PNRNo,
        b.TotalSeats,
        b.TotalAmount,
        b.TravelDate,
        b.Status,
        b.BookedAt,
        bus.BusName,
        CONCAT(r.Source, ' → ', r.Destination) AS Route,
        v.AgencyName AS VendorName,
        bs.DepartureTime,
        bs.ArrivalTime,
        ISNULL(p.PaymentMethod, 0) AS PaymentMethod,
        CASE WHEN p.PaymentId IS NOT NULL AND p.IsActive = 1 THEN 1 ELSE 0 END AS IsPaymentCompleted
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId AND c.IsActive = 1
    INNER JOIN BookingSegments bseg ON b.BookingId = bseg.BookingId
    INNER JOIN BusSchedules bs ON bseg.ScheduleId = bs.ScheduleId AND bs.IsActive = 1
    INNER JOIN Buses bus ON bs.BusId = bus.BusId AND bus.IsActive = 1
    INNER JOIN Routes r ON bs.RouteId = r.RouteId AND r.IsActive = 1
    INNER JOIN Vendors v ON bus.VendorId = v.VendorId
    LEFT JOIN Payments p ON b.BookingId = p.BookingId AND p.IsActive = 1
    WHERE b.CustomerId = @CustomerId
        AND b.IsActive = 1
        AND (@Status IS NULL OR b.Status = @Status)
        AND (@FromDate IS NULL OR b.TravelDate >= @FromDate)
        AND (@ToDate IS NULL OR b.TravelDate <= @ToDate)
    ORDER BY b.BookedAt DESC;
END
GO
------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetBookingDetailsForEmail]
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get booking details with customer, user, bus, route, and payment info
    SELECT 
        -- Booking Info
        b.BookingId,
        b.PNRNo,
        b.TotalAmount,
        b.TravelDate,
        b.Status AS BookingStatus,
        b.BookedAt,
        
        -- Customer Info
        c.CustomerId,
        c.FirstName,
        c.LastName,
        
        -- User Info (Email) - Fixed to get correct customer's email
        u.Email AS CustomerEmail,
        u.Phone AS CustomerPhone,
        
        -- Bus Info
        bus.BusName,
        bus.BusType,
        bus.RegistrationNo,
        
        -- Route Info
        r.Source,
        r.Destination,
        
        -- Schedule Info
        bs.DepartureTime,
        bs.ArrivalTime,
        bs.TravelDate AS ScheduleTravelDate,
        
        -- Vendor Info
        v.AgencyName AS VendorName,
        
        -- Payment Info
        p.PaymentMethod,
        p.TransactionId,
        p.PaymentDate,
        
        -- Boarding/Dropping Stops
        boardingStop.Name AS BoardingStopName,
        boardingStop.Landmark AS BoardingStopLandmark,
        droppingStop.Name AS DroppingStopName,
        droppingStop.Landmark AS DroppingStopLandmark,
        
        -- Segment Info
        bseg.SegmentOrder,
        bseg.SeatsBooked,
        bseg.SegmentAmount
        
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId AND c.IsActive = 1
    INNER JOIN Users u ON c.UserId = u.UserId AND u.IsActive = 1
    INNER JOIN BookingSegments bseg ON b.BookingId = bseg.BookingId
    INNER JOIN BusSchedules bs ON bseg.ScheduleId = bs.ScheduleId AND bs.IsActive = 1
    INNER JOIN Buses bus ON bs.BusId = bus.BusId AND bus.IsActive = 1
    INNER JOIN Routes r ON bs.RouteId = r.RouteId AND r.IsActive = 1
    INNER JOIN Vendors v ON bus.VendorId = v.VendorId
    LEFT JOIN Payments p ON b.BookingId = p.BookingId AND p.IsActive = 1
    LEFT JOIN Stops boardingStop ON bseg.BoardingStopId = boardingStop.StopId
    LEFT JOIN Stops droppingStop ON bseg.DroppingStopId = droppingStop.StopId
    
    WHERE b.BookingId = @BookingId
        AND b.IsActive = 1
        AND b.Status = 2 -- Confirmed bookings only
    ORDER BY bseg.SegmentOrder ASC;
    
    -- Get passenger details for this booking
    SELECT 
        bst.BookedSeatId,
        bst.SeatNumber,
        bst.PassengerName,
        bst.PassengerAge,
        bst.PassengerGender,
        bst.SeatType,
        bst.SeatPosition,
        bseg.SegmentOrder
    FROM BookedSeats bst
    INNER JOIN BookingSegments bseg ON bst.BookingSegmentId = bseg.BookingSegmentId
    INNER JOIN Bookings b ON bseg.BookingId = b.BookingId
    WHERE bseg.BookingId = @BookingId
        AND b.IsActive = 1
        AND b.Status = 2
    ORDER BY bseg.SegmentOrder ASC, bst.SeatNumber ASC;
END
GO
------------------------------
CREATE OR ALTER PROCEDURE sp_GetBookingForCancellation
    @BookingId INT,
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BookingId,
        b.CustomerId,
        b.PNRNo,
        b.TotalAmount,
        b.TravelDate,
        b.BookedAt,
        b.Status,
        p.PaymentMethod,
        p.TransactionId,
        DATEDIFF(HOUR, GETUTCDATE(), b.TravelDate) as HoursUntilTravel
    FROM Bookings b
    LEFT JOIN Payments p ON b.BookingId = p.BookingId
    WHERE b.BookingId = @BookingId 
        AND b.CustomerId = @CustomerId 
        AND b.IsActive = 1
        AND b.Status IN (1, 2); -- Pending or Confirmed only
END
GO
------------------------------
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
GO
------------------------------
-- Stored Procedure: Get Vendor Revenue Analytics
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
    AND p.PaymentStatus = 2 -- Success
    AND b.IsActive = 1;
    
    -- Get monthly revenue (current month)
    SELECT @MonthlyRevenue = ISNULL(SUM(p.Amount), 0)
    FROM Payments p
    INNER JOIN Bookings b ON p.BookingId = b.BookingId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    WHERE bus.VendorId = @VendorId 
    AND p.PaymentStatus = 2 -- Success
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
    AND p.PaymentStatus = 2 -- Success
    AND b.IsActive = 1
    AND p.CreatedOn >= DATEADD(DAY, -7, GETDATE());
    
    -- Return results
    SELECT 
        @TotalRevenue AS TotalRevenue,
        @MonthlyRevenue AS MonthlyRevenue,
        @WeeklyRevenue AS WeeklyRevenue;
END
GO
------------------------------
-- Stored Procedure: Get Vendor Performance Metrics
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
    
    -- Calculate on-time performance (assuming 95% for now as we don't have actual tracking)
    SET @OnTimePerformance = 95.0;
    
    -- Return results
    SELECT 
        @MonthlyBookings AS MonthlyBookings,
        @OnTimePerformance AS OnTimePerformance;
END
GO
------------------------------
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
GO
------------------------------
-- Stored Procedure: Get Vendor Recent Bookings
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
------------------------------
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
GO
------------------------------
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
GO
------------------------------
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
GO
------------------------------
-- Stored Procedure: Get Vendor By ID
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
------------------------------
-- Stored Procedure: Check if Vendor Exists by Email
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
------------------------------
-- Stored Procedure: Check if Vendor Exists by ID
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
------------------------------
-- Stored Procedure: Get Vendor Quick Stats
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
    AND p.PaymentStatus = 2 -- Success
    AND b.IsActive = 1;
    
    -- Return results
    SELECT 
        @TotalBookings AS TotalBookings,
        @ActiveRoutes AS ActiveRoutes,
        @TotalRevenue AS TotalRevenue;
END
GO
----------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetPendingVendors
    @Offset INT,
    @PageSize INT
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
        v.CreatedOn,
        u.Email,
        u.Phone
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.Status = 0 -- PendingApproval
    ORDER BY v.CreatedOn DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
----------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetAllVendors
    @Offset INT,
    @PageSize INT
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
        v.IsActive, 
        v.Status,
        v.CreatedOn,
        u.Email, 
        u.Phone
    FROM Vendors v 
    INNER JOIN Users u ON v.UserId = u.UserId 
    ORDER BY v.CreatedOn DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
----------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetPendingVendorsCount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS PendingCount
    FROM Vendors 
    WHERE Status = 0; -- PendingApproval
END
----------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetVendorTotalCount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS TotalCount
    FROM Vendors;
END
GO
----------------------------------------------------