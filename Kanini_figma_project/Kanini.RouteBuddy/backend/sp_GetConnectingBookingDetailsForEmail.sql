CREATE PROCEDURE [dbo].[sp_GetConnectingBookingDetailsForEmail]
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get booking and customer details
    SELECT 
        b.PNRNo,
        b.TravelDate,
        b.TotalAmount,
        c.FirstName + ' ' + c.LastName AS CustomerName,
        u.Email AS CustomerEmail,
        COUNT(bs.BookingSegmentId) AS TotalSegments
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId
    INNER JOIN Users u ON c.UserId = u.UserId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    WHERE b.BookingId = @BookingId
    GROUP BY b.PNRNo, b.TravelDate, b.TotalAmount, c.FirstName, c.LastName, u.Email;
    
    -- Get segment details with passengers (Recently added first - ORDER BY bs.CreatedOn DESC)
    SELECT 
        bs.SegmentOrder,
        bus.BusName,
        r.Source,
        r.Destination,
        sch.DepartureTime,
        sch.ArrivalTime,
        boarding.Name AS BoardingStop,
        dropping.Name AS DroppingStop,
        seat.PassengerName,
        seat.SeatNumber,
        seat.PassengerAge,
        CASE seat.PassengerGender 
            WHEN 1 THEN 'Male' 
            WHEN 2 THEN 'Female' 
            ELSE 'Other' 
        END AS PassengerGender
    FROM BookingSegments bs
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses bus ON sch.BusId = bus.BusId
    INNER JOIN Routes r ON sch.RouteId = r.RouteId
    INNER JOIN Stops boarding ON bs.BoardingStopId = boarding.StopId
    INNER JOIN Stops dropping ON bs.DroppingStopId = dropping.StopId
    INNER JOIN BookedSeats seat ON bs.BookingSegmentId = seat.BookingSegmentId
    WHERE bs.BookingId = @BookingId
    ORDER BY bs.SegmentOrder, seat.CreatedOn DESC;
END