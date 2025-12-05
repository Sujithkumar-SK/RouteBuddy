CREATE PROCEDURE [dbo].[sp_GetAllCustomersWithSummary]
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