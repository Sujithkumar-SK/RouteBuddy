CREATE PROCEDURE [dbo].[sp_FilterCustomersWithSummary]
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