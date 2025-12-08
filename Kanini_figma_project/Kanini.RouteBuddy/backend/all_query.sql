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
        v.CreatedOn,
        u.Email,
        u.Phone
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.VendorId = @VendorId
    ORDER BY v.CreatedOn DESC;
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
        v.Status,
        v.IsActive,
        v.CreatedOn,
        u.Email,
        u.Phone
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    ORDER BY v.CreatedOn DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
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
CREATE OR ALTER PROCEDURE sp_GetPlaceAutocomplete
    @Query NVARCHAR(50),
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all unique cities from Routes table when query is empty, otherwise filter
    IF @Query = ''
    BEGIN
        SELECT 
            ROW_NUMBER() OVER (ORDER BY CityName) as StopId,
            CityName as Name,
            NULL as Landmark
        FROM (
            SELECT DISTINCT r.Source as CityName FROM Routes r WHERE r.IsActive = 1
            UNION
            SELECT DISTINCT r.Destination as CityName FROM Routes r WHERE r.IsActive = 1
        ) Cities
        ORDER BY CityName ASC;
    END
    ELSE
    BEGIN
        SELECT TOP (@Limit)
            ROW_NUMBER() OVER (ORDER BY Priority, CityName) as StopId,
            CityName as Name,
            NULL as Landmark
        FROM (
            SELECT DISTINCT 
                r.Source as CityName,
                CASE WHEN r.Source LIKE @Query + '%' THEN 1 ELSE 2 END as Priority
            FROM Routes r 
            WHERE r.IsActive = 1 AND r.Source LIKE '%' + @Query + '%'
            UNION
            SELECT DISTINCT 
                r.Destination as CityName,
                CASE WHEN r.Destination LIKE @Query + '%' THEN 1 ELSE 2 END as Priority
            FROM Routes r 
            WHERE r.IsActive = 1 AND r.Destination LIKE '%' + @Query + '%'
        ) Cities
        ORDER BY Priority, CityName ASC;
    END
END
GO
----------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ValidatePlaceExists
    @PlaceName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if place exists in Routes table (either as source or destination)
    SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END as PlaceExists
    FROM Routes r
    WHERE r.IsActive = 1
        AND (LOWER(TRIM(r.Source)) = LOWER(TRIM(@PlaceName))
             OR LOWER(TRIM(r.Destination)) = LOWER(TRIM(@PlaceName)));
END
GO
------------------------------------------------------
-- Stored Procedure: Get Vendor For Approval with Documents
CREATE OR ALTER PROCEDURE sp_GetVendorForApproval
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get vendor details
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
        u.Phone,
        CASE v.Status
            WHEN 0 THEN 'PendingApproval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Rejected'
        END as StatusText
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.VendorId = @VendorId;
    
    -- Get vendor documents
    SELECT 
        vd.DocumentId,
        vd.VendorId,
        vd.DocumentFile,
        vd.DocumentPath,
        vd.IssueDate,
        vd.ExpiryDate,
        vd.UploadedAt,
        vd.IsVerified,
        vd.VerifiedAt,
        vd.VerifiedBy,
        vd.RejectedReason,
        vd.Status,
        CASE vd.DocumentFile
            WHEN 0 THEN 'BusinessLicense'
            WHEN 1 THEN 'TaxRegistration'
            WHEN 2 THEN 'IdentityProof'
            WHEN 3 THEN 'AddressProof'
        END as DocumentType
    FROM VendorDocuments vd
    WHERE vd.VendorId = @VendorId
    ORDER BY vd.UploadedAt DESC;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetSeatLayoutTemplatesByBusType
    @BusType INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            SeatLayoutTemplateId,
            TemplateName,
            TotalSeats,
            BusType,
            Description,
            IsActive,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn,
            (SELECT COUNT(*) FROM SeatLayoutDetails WHERE SeatLayoutTemplateId = SLT.SeatLayoutTemplateId) as SeatDetailsCount
        FROM SeatLayoutTemplates SLT
        WHERE IsActive = 1 
        AND (@BusType IS NULL OR BusType = @BusType)
        ORDER BY CreatedOn DESC; -- Rule 9: Recent records first
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ApplyTemplateToLayout
    @BusId INT,
    @TemplateId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Update bus with template ID
        UPDATE Buses 
        SET SeatLayoutTemplateId = @TemplateId,
            UpdatedOn = GETUTCDATE(),
            UpdatedBy = 'System'
        WHERE BusId = @BusId;
        
        COMMIT TRANSACTION;
        
        SELECT 1 as Success;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetBusPhotoById
    @BusPhotoId INT
AS
BEGIN
    SELECT BusPhotoId, BusId, ImagePath, Caption, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn 
    FROM BusPhotos 
    WHERE BusPhotoId = @BusPhotoId
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetBusPhotosByBusId
    @BusId INT
AS
BEGIN
    SELECT BusPhotoId, BusId, ImagePath, Caption, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn 
    FROM BusPhotos 
    WHERE BusId = @BusId 
    ORDER BY CreatedOn
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_CheckBusPhotoExists
    @BusPhotoId INT
AS
BEGIN
    SELECT COUNT(1) 
    FROM BusPhotos 
    WHERE BusPhotoId = @BusPhotoId
END
GO
------------------------------------------------------
-- Stored Procedure: Search Routes with Real-time Filtering
CREATE OR ALTER PROCEDURE sp_SearchRoutes
    @Source NVARCHAR(100) = NULL,
    @Destination NVARCHAR(100) = NULL,
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate parameters
    IF @Limit <= 0 OR @Limit > 50
        SET @Limit = 10;
    
    SELECT TOP (@Limit)
        r.RouteId,
        r.Source,
        r.Destination,
        r.Distance,
        r.Duration,
        r.BasePrice,
        COUNT(rs.RouteStopId) as TotalStops,
        r.IsActive
    FROM Routes r
    LEFT JOIN RouteStops rs ON r.RouteId = rs.RouteId
    WHERE r.IsActive = 1
        AND (@Source IS NULL OR r.Source LIKE '%' + @Source + '%')
        AND (@Destination IS NULL OR r.Destination LIKE '%' + @Destination + '%')
    GROUP BY r.RouteId, r.Source, r.Destination, r.Distance, r.Duration, r.BasePrice, r.IsActive, r.CreatedOn
    ORDER BY r.CreatedOn DESC;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetBusPhotoCountByBusId
    @BusId INT
AS
BEGIN
    SELECT COUNT(*) 
    FROM BusPhotos 
    WHERE BusId = @BusId
END
GO
------------------------------------------------------
-- Stored Procedure: Get Route Stops with Detailed Information
CREATE OR ALTER PROCEDURE sp_GetRouteStopsWithDetails
    @RouteId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate route exists
    IF NOT EXISTS (SELECT 1 FROM Routes WHERE RouteId = @RouteId AND IsActive = 1)
    BEGIN
        RAISERROR('Route not found or inactive', 16, 1);
        RETURN;
    END
    
    SELECT 
        rs.RouteStopId,
        rs.StopId,
        s.Name as StopName,
        s.Landmark,
        rs.OrderNumber,
        rs.ArrivalTime,
        rs.DepartureTime
    FROM RouteStops rs
    INNER JOIN Stops s ON rs.StopId = s.StopId
    WHERE rs.RouteId = @RouteId
        AND s.IsActive = 1
    ORDER BY rs.OrderNumber ASC;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetScheduleById]
    @ScheduleId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input
        IF @ScheduleId <= 0
        BEGIN
            SELECT 'ERROR' AS Result, 'INVALID_SCHEDULE_ID' AS ErrorMessage;
            RETURN;
        END
        
        -- Get schedule data
        SELECT ScheduleId, BusId, RouteId, TravelDate, DepartureTime, ArrivalTime, 
               AvailableSeats, IsActive
        FROM BusSchedules 
        WHERE ScheduleId = @ScheduleId AND IsActive = 1;
        
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'ERROR' AS Result, 'SCHEDULE_NOT_FOUND' AS ErrorMessage;
        END
        ELSE
        BEGIN
            SELECT 'SUCCESS' AS Result;
        END
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetSchedulesByVendor]
    @VendorId INT,
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input parameters
        IF @VendorId <= 0 OR @PageNumber <= 0 OR @PageSize <= 0
        BEGIN
            SELECT 'ERROR' AS Result, 'INVALID_PARAMETERS' AS ErrorMessage;
            RETURN;
        END
        
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
        
        -- Get paginated schedules with bus and route information
        SELECT s.ScheduleId, s.BusId, b.BusName, s.RouteId, r.Source, r.Destination,
               s.TravelDate, s.DepartureTime, s.ArrivalTime, s.AvailableSeats, 
               s.Status, s.IsActive
        FROM BusSchedules s
        INNER JOIN Buses b ON s.BusId = b.BusId
        INNER JOIN Routes r ON s.RouteId = r.RouteId
        WHERE b.VendorId = @VendorId AND s.IsActive = 1 
        ORDER BY s.CreatedOn DESC 
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        
        SELECT 'SUCCESS' AS Result;
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetScheduleCountByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF @VendorId IS NULL OR @VendorId <= 0
        BEGIN
            RAISERROR('Invalid VendorId parameter', 16, 1);
            RETURN;
        END
        
        SELECT COUNT(*)
        FROM BusSchedules bs
        INNER JOIN Buses b ON bs.BusId = b.BusId
        WHERE b.VendorId = @VendorId 
        AND bs.IsActive = 1;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_CheckScheduleExists
    @BusId INT,
    @RouteId INT,
    @TravelDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF @BusId IS NULL OR @BusId <= 0
        BEGIN
            RAISERROR('Invalid BusId parameter', 16, 1);
            RETURN;
        END
        
        IF @RouteId IS NULL OR @RouteId <= 0
        BEGIN
            RAISERROR('Invalid RouteId parameter', 16, 1);
            RETURN;
        END
        
        IF @TravelDate IS NULL
        BEGIN
            RAISERROR('Invalid TravelDate parameter', 16, 1);
            RETURN;
        END
        
        SELECT COUNT(*)
        FROM BusSchedules
        WHERE BusId = @BusId 
        AND RouteId = @RouteId 
        AND CAST(TravelDate AS DATE) = @TravelDate;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetAllActiveRoutes
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            r.RouteId,
            r.Source,
            r.Destination,
            r.Distance,
            r.Duration,
            r.BasePrice,
            COUNT(rs.RouteStopId) as TotalStops,
            r.IsActive
        FROM Routes r
        LEFT JOIN RouteStops rs ON r.RouteId = rs.RouteId
        WHERE r.IsActive = 1
        GROUP BY r.RouteId, r.Source, r.Destination, r.Distance, r.Duration, r.BasePrice, r.IsActive, r.CreatedOn
        ORDER BY r.CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllActiveStops]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            StopId,
            Name,
            Landmark,
            IsActive,
            CreatedOn
        FROM Stops 
        WHERE IsActive = 1
        ORDER BY CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetRouteById]
    @RouteId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input
        IF @RouteId <= 0
        BEGIN
            SELECT 'ERROR' AS Result, 'INVALID_ROUTE_ID' AS ErrorMessage;
            RETURN;
        END
        
        -- Get route data
        SELECT RouteId, Source, Destination, Distance, Duration, BasePrice, IsActive 
        FROM Routes 
        WHERE RouteId = @RouteId AND IsActive = 1;
        
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'ERROR' AS Result, 'ROUTE_NOT_FOUND' AS ErrorMessage;
        END
        ELSE
        BEGIN
            SELECT 'SUCCESS' AS Result;
        END
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetRouteStops]
    @ScheduleId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        rs.RouteStopId,
        rs.StopId,
        s.Name AS StopName,
        s.Landmark,
        rs.OrderNumber,
        rs.ArrivalTime,
        rs.DepartureTime
    FROM BusSchedules bs
    INNER JOIN RouteStops rs ON bs.ScheduleId = rs.ScheduleId
    INNER JOIN Stops s ON rs.StopId = s.StopId
    WHERE bs.ScheduleId = @ScheduleId
        AND bs.Status = 1 -- Scheduled
        AND bs.IsActive = 1
        AND s.IsActive = 1
    ORDER BY rs.OrderNumber ASC;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ValidateSeatsAndStops]
    @ScheduleId INT,
    @TravelDate DATE,
    @SeatNumbers NVARCHAR(MAX), -- Comma-separated seat numbers
    @BoardingStopId INT,
    @DroppingStopId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate stops exist in route and order is correct
    DECLARE @BoardingOrder INT, @DroppingOrder INT;
    
    SELECT @BoardingOrder = rs.OrderNumber
    FROM BusSchedules bs
    INNER JOIN RouteStops rs ON bs.ScheduleId = rs.ScheduleId
    WHERE bs.ScheduleId = @ScheduleId AND rs.RouteStopId = @BoardingStopId;
    
    SELECT @DroppingOrder = rs.OrderNumber
    FROM BusSchedules bs
    INNER JOIN RouteStops rs ON bs.ScheduleId = rs.ScheduleId
    WHERE bs.ScheduleId = @ScheduleId AND rs.RouteStopId = @DroppingStopId;
    
    -- Check if stops are valid
    IF @BoardingOrder IS NULL OR @DroppingOrder IS NULL
    BEGIN
        SELECT 'INVALID_STOPS' AS ValidationResult;
        RETURN;
    END
    
    -- Check if dropping stop is after boarding stop
    IF @DroppingOrder <= @BoardingOrder
    BEGIN
        SELECT 'INVALID_STOP_ORDER' AS ValidationResult;
        RETURN;
    END
    
    -- Validate seat availability (reuse existing logic)
    SELECT 
        sld.SeatNumber,
        sld.SeatType,
        sld.SeatPosition,
        sld.PriceTier,
        r.BasePrice,
        b.BusType,
        b.Amenities,
        CASE WHEN bks.BookedSeatId IS NOT NULL THEN 1 ELSE 0 END AS IsBooked
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    INNER JOIN Routes r ON bs.RouteId = r.RouteId
    INNER JOIN SeatLayoutTemplates slt ON b.SeatLayoutTemplateId = slt.SeatLayoutTemplateId
    INNER JOIN SeatLayoutDetails sld ON slt.SeatLayoutTemplateId = sld.SeatLayoutTemplateId
    LEFT JOIN BookedSeats bks ON bks.SeatNumber = sld.SeatNumber 
        AND bks.TravelDate = @TravelDate 
        AND EXISTS (
            SELECT 1 FROM BookingSegments bsg 
            INNER JOIN Bookings bk ON bsg.BookingId = bk.BookingId
            WHERE bsg.ScheduleId = @ScheduleId 
            AND bks.BookingSegmentId = bsg.BookingSegmentId
            AND bk.Status IN (1, 2) -- Pending or Confirmed
            AND bk.IsActive = 1
        )
    WHERE bs.ScheduleId = @ScheduleId
        AND bs.TravelDate = @TravelDate
        AND bs.Status = 1 -- Scheduled
        AND bs.IsActive = 1
        AND b.Status = 1 -- Active
        AND b.IsActive = 1
        AND sld.SeatNumber IN (SELECT value FROM STRING_SPLIT(@SeatNumbers, ','))
    ORDER BY sld.RowNumber, sld.ColumnNumber;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetVendorRecentBookings]
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 10 
        b.BookingId,
        CONCAT(c.FirstName, ' ', c.LastName) as CustomerName,
        CONCAT(r.Source, ' to ', r.Destination) as Route,
        b.BookedAt as BookingDate,
        CASE b.Status 
            WHEN 0 THEN 'Pending'
            WHEN 1 THEN 'Confirmed' 
            WHEN 2 THEN 'Cancelled'
            WHEN 3 THEN 'Completed'
            ELSE 'Unknown'
        END as Status
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    INNER JOIN Routes r ON sch.RouteId = r.RouteId
    WHERE bus.VendorId = @VendorId
    ORDER BY b.BookedAt DESC;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetAllBookings
AS
BEGIN
    SELECT 
        b.BookingId,
        b.PNRNo,
        b.TotalSeats,
        b.TotalAmount,
        b.TravelDate,
        b.Status,
        b.BookedAt,
        CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
        u.Email AS CustomerEmail,
        u.Phone AS CustomerPhone,
        bus.BusName,
        CONCAT(r.Source, ' - ', r.Destination) AS Route,
        CASE 
            WHEN b.Status = 3 AND rf.RefundStatus = 2 THEN 4  -- Cancelled and Refunded (add new enum value)
            WHEN b.Status = 3 AND rf.RefundStatus = 1 THEN 1  -- Cancelled but refund pending
            WHEN b.Status = 3 AND (rf.RefundStatus IS NULL OR rf.RefundStatus = 3) THEN 3  -- Cancelled, refund failed
            WHEN b.Status = 1 THEN 1  -- Pending booking, payment pending
            ELSE ISNULL(p.PaymentStatus, 1)  -- Use actual payment status or default to pending
        END AS PaymentStatus
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId
    INNER JOIN Users u ON c.UserId = u.UserId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    INNER JOIN Routes r ON sch.RouteId = r.RouteId
    LEFT JOIN Payments p ON b.BookingId = p.BookingId
    LEFT JOIN Cancellations can ON b.BookingId = can.BookingId
    LEFT JOIN Refunds rf ON p.PaymentId = rf.PaymentId
    ORDER BY b.BookedAt DESC
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_FilterBusesForAdmin]
    @SearchName NVARCHAR(100) = NULL,
    @Status INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BusId,
        b.BusName,
        b.BusType,
        b.TotalSeats,
        b.RegistrationNo,
        b.Status,
        b.Amenities,
        b.DriverName,
        b.DriverContact,
        b.IsActive,
        b.CreatedOn,
        b.CreatedBy,
        b.UpdatedBy,
        b.SeatLayoutTemplateId,
        v.AgencyName AS VendorName,
        v.VendorId
    FROM Buses b
    INNER JOIN Vendors v ON b.VendorId = v.VendorId
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE 
        (@SearchName IS NULL OR b.BusName LIKE '%' + @SearchName + '%' OR v.AgencyName LIKE '%' + @SearchName + '%')
        AND (@Status IS NULL OR b.Status = @Status)
        AND (@IsActive IS NULL OR b.IsActive = @IsActive)
    ORDER BY b.CreatedOn DESC;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllBusesForAdmin]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BusId,
        b.BusName,
        b.BusType,
        b.TotalSeats,
        b.RegistrationNo,
        b.Status,
        b.Amenities,
        b.DriverName,
        b.DriverContact,
        b.IsActive,
        b.CreatedOn,
        b.CreatedBy,
        b.UpdatedBy,
        b.SeatLayoutTemplateId,
        v.AgencyName AS VendorName,
        v.VendorId
    FROM Buses b
    INNER JOIN Vendors v ON b.VendorId = v.VendorId
    INNER JOIN Users u ON v.UserId = u.UserId
    ORDER BY b.CreatedOn DESC;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetBusesByStatus]
    @Status INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BusId,
        b.BusName,
        b.BusType,
        b.TotalSeats,
        b.RegistrationNo,
        b.Status,
        b.Amenities,
        b.DriverName,
        b.DriverContact,
        b.IsActive,
        b.CreatedOn,
        b.CreatedBy,
        b.UpdatedBy,
        b.SeatLayoutTemplateId,
        v.AgencyName AS VendorName,
        v.VendorId
    FROM Buses b
    INNER JOIN Vendors v ON b.VendorId = v.VendorId
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE b.Status = @Status
    ORDER BY b.CreatedOn DESC;
END
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllCustomersWithSummary]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerId,
        CONCAT(c.FirstName, ' ', c.LastName) AS FullName,
        c.Gender,
        DATEDIFF(YEAR, c.DateOfBirth, GETDATE()) AS Age,
        c.IsActive,
        c.CreatedOn,
        u.Email,
        u.Phone,
        u.UserId,
        ISNULL(booking_stats.TotalBookings, 0) AS TotalBookings,
        ISNULL(booking_stats.TotalSpent, 0) AS TotalSpent,
        booking_stats.LastBookingDate
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    LEFT JOIN (
        SELECT 
            b.CustomerId,
            COUNT(*) AS TotalBookings,
            SUM(b.TotalAmount) AS TotalSpent,
            MAX(b.BookedAt) AS LastBookingDate
        FROM Bookings b
        WHERE b.Status IN (1, 2) -- Pending or Confirmed bookings
        GROUP BY b.CustomerId
    ) booking_stats ON c.CustomerId = booking_stats.CustomerId
    ORDER BY c.CreatedOn DESC;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_FilterCustomersWithSummary]
    @SearchName NVARCHAR(100) = NULL,
    @IsActive BIT = NULL,
    @MinAge INT = NULL,
    @MaxAge INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerId,
        CONCAT(c.FirstName, ' ', c.LastName) AS FullName,
        c.Gender,
        DATEDIFF(YEAR, c.DateOfBirth, GETDATE()) AS Age,
        c.IsActive,
        c.CreatedOn,
        u.Email,
        u.Phone,
        u.UserId,
        ISNULL(booking_stats.TotalBookings, 0) AS TotalBookings,
        ISNULL(booking_stats.TotalSpent, 0) AS TotalSpent,
        booking_stats.LastBookingDate
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    LEFT JOIN (
        SELECT 
            b.CustomerId,
            COUNT(*) AS TotalBookings,
            SUM(b.TotalAmount) AS TotalSpent,
            MAX(b.BookedAt) AS LastBookingDate
        FROM Bookings b
        WHERE b.Status IN (1, 2) -- Pending or Confirmed bookings
        GROUP BY b.CustomerId
    ) booking_stats ON c.CustomerId = booking_stats.CustomerId
    WHERE 
        (@SearchName IS NULL OR CONCAT(c.FirstName, ' ', c.LastName) LIKE '%' + @SearchName + '%' OR u.Email LIKE '%' + @SearchName + '%')
        AND (@IsActive IS NULL OR c.IsActive = @IsActive)
        AND (@MinAge IS NULL OR DATEDIFF(YEAR, c.DateOfBirth, GETDATE()) >= @MinAge)
        AND (@MaxAge IS NULL OR DATEDIFF(YEAR, c.DateOfBirth, GETDATE()) <= @MaxAge)
    ORDER BY c.CreatedOn DESC;
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetAllStops
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
        
        SELECT 
            StopId,
            Name,
            Landmark,
            IsActive,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Stops 
        WHERE IsActive = 1
        ORDER BY CreatedOn DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetStopById
    @StopId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            StopId,
            Name,
            Landmark,
            IsActive,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Stops 
        WHERE StopId = @StopId AND IsActive = 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ValidateSeatsAndStops]
    @ScheduleId INT,
    @TravelDate DATE,
    @SeatNumbers NVARCHAR(MAX), -- Comma-separated seat numbers
    @BoardingStopId INT,
    @DroppingStopId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate stops exist in route and order is correct
    DECLARE @BoardingOrder INT, @DroppingOrder INT, @RouteId INT;
    
    -- Get the RouteId for this schedule
    SELECT @RouteId = RouteId
    FROM BusSchedules 
    WHERE ScheduleId = @ScheduleId;
    
    -- Check if schedule exists
    IF @RouteId IS NULL
    BEGIN
        SELECT 'SCHEDULE_NOT_FOUND' AS ValidationResult;
        RETURN;
    END
    
    -- Get boarding stop order using RouteId instead of ScheduleId
    SELECT @BoardingOrder = rs.OrderNumber
    FROM RouteStops rs
    WHERE rs.RouteId = @RouteId AND rs.StopId = @BoardingStopId;
    
    -- Get dropping stop order using RouteId instead of ScheduleId  
    SELECT @DroppingOrder = rs.OrderNumber
    FROM RouteStops rs
    WHERE rs.RouteId = @RouteId AND rs.StopId = @DroppingStopId;
    
    -- Check if stops are valid
    IF @BoardingOrder IS NULL OR @DroppingOrder IS NULL
    BEGIN
        SELECT 'INVALID_STOPS' AS ValidationResult;
        RETURN;
    END
    
    -- Check if dropping stop is after boarding stop
    IF @DroppingOrder <= @BoardingOrder
    BEGIN
        SELECT 'INVALID_STOP_ORDER' AS ValidationResult;
        RETURN;
    END
    
    -- Validate seat availability (reuse existing logic)
    SELECT 
        sld.SeatNumber,
        sld.SeatType,
        sld.SeatPosition,
        sld.PriceTier,
        r.BasePrice,
        b.BusType,
        b.Amenities,
        CASE WHEN bks.BookedSeatId IS NOT NULL THEN 1 ELSE 0 END AS IsBooked
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    INNER JOIN Routes r ON bs.RouteId = r.RouteId
    INNER JOIN SeatLayoutTemplates slt ON b.SeatLayoutTemplateId = slt.SeatLayoutTemplateId
    INNER JOIN SeatLayoutDetails sld ON slt.SeatLayoutTemplateId = sld.SeatLayoutTemplateId
    LEFT JOIN BookedSeats bks ON bks.SeatNumber = sld.SeatNumber 
        AND bks.TravelDate = @TravelDate 
        AND EXISTS (
            SELECT 1 FROM BookingSegments bsg 
            INNER JOIN Bookings bk ON bsg.BookingId = bk.BookingId
            WHERE bsg.ScheduleId = @ScheduleId 
            AND bks.BookingSegmentId = bsg.BookingSegmentId
            AND bk.Status IN (1, 2) -- Pending or Confirmed
            AND bk.IsActive = 1
        )
    WHERE bs.ScheduleId = @ScheduleId
        AND bs.TravelDate = @TravelDate
        AND bs.Status = 1 -- Scheduled
        AND bs.IsActive = 1
        AND b.Status = 1 -- Active
        AND b.IsActive = 1
        AND sld.SeatNumber IN (SELECT value FROM STRING_SPLIT(@SeatNumbers, ','))
    ORDER BY sld.RowNumber, sld.ColumnNumber;
END
------------------------------------------------------