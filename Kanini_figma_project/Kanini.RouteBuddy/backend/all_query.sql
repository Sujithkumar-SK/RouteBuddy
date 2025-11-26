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