ALTER PROCEDURE [dbo].[sp_GetVendorRevenueAnalytics]
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            COALESCE(SUM(p.Amount), 0) AS TotalRevenue,
            COALESCE(SUM(CASE WHEN p.PaymentDate >= DATEADD(MONTH, -1, GETDATE()) THEN p.Amount ELSE 0 END), 0) AS MonthlyRevenue,
            COALESCE(SUM(CASE WHEN p.PaymentDate >= DATEADD(WEEK, -1, GETDATE()) THEN p.Amount ELSE 0 END), 0) AS WeeklyRevenue
        FROM Payments p
        INNER JOIN Bookings b ON p.BookingId = b.BookingId
        INNER JOIN BookingSegments bseg ON b.BookingId = bseg.BookingId
        INNER JOIN BusSchedules bs ON bseg.ScheduleId = bs.ScheduleId
        INNER JOIN Buses bus ON bs.BusId = bus.BusId
        WHERE bus.VendorId = @VendorId AND p.PaymentStatus = 2;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END