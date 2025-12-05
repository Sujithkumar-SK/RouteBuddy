CREATE PROCEDURE [dbo].[sp_GetConnectingBookingDetailsForEmail]
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        -- Booking Information
        b.BookingId,
        b.PNRNo,
        b.TotalAmount,
        b.TravelDate,
        b.BookedAt,
        
        -- Customer Information
        c.FirstName,
        c.LastName,
        u.Email AS CustomerEmail,
        u.Phone AS CustomerPhone,
        
        -- Overall Journey Information
        (SELECT TOP 1 r.Source 
         FROM BookingSegments bs1 
         INNER JOIN BusSchedules bsch1 ON bs1.ScheduleId = bsch1.ScheduleId
         INNER JOIN Routes r ON bsch1.RouteId = r.RouteId
         WHERE bs1.BookingId = b.BookingId 
         ORDER BY bs1.SegmentOrder ASC) AS OverallSource,
        
        (SELECT TOP 1 r.Destination 
         FROM BookingSegments bs2 
         INNER JOIN BusSchedules bsch2 ON bs2.ScheduleId = bsch2.ScheduleId
         INNER JOIN Routes r ON bsch2.RouteId = r.RouteId
         WHERE bs2.BookingId = b.BookingId 
         ORDER BY bs2.SegmentOrder DESC) AS OverallDestination,
        
        -- Payment Information
        p.PaymentMethod,
        p.TransactionId,
        p.PaymentDate,
        
        -- Segment Information
        bs.SegmentOrder,
        bus.BusName,
        bus.RegistrationNo,
        r.Source,
        r.Destination,
        bsch.DepartureTime,
        bsch.ArrivalTime,
        v.AgencyName AS VendorName,
        bs.SegmentAmount,
        
        -- Stop Information
        boarding_stop.Name AS BoardingStopName,
        boarding_stop.Landmark AS BoardingStopLandmark,
        dropping_stop.Name AS DroppingStopName,
        dropping_stop.Landmark AS DroppingStopLandmark,
        
        -- Passenger and Seat Information
        bseat.SeatNumber,
        bseat.PassengerName,
        bseat.PassengerAge,
        bseat.PassengerGender,
        bseat.SeatType,
        bseat.SeatPosition
        
    FROM Bookings b
    INNER JOIN Customers c ON b.CustomerId = c.CustomerId
    INNER JOIN Users u ON c.UserId = u.UserId
    INNER JOIN Payments p ON b.BookingId = p.BookingId
    INNER JOIN BookingSegments bs ON b.BookingId = bs.BookingId
    INNER JOIN BusSchedules bsch ON bs.ScheduleId = bsch.ScheduleId
    INNER JOIN Buses bus ON bsch.BusId = bus.BusId
    INNER JOIN Routes r ON bsch.RouteId = r.RouteId
    INNER JOIN Vendors v ON bus.VendorId = v.VendorId
    INNER JOIN Stops boarding_stop ON bs.BoardingStopId = boarding_stop.StopId
    INNER JOIN Stops dropping_stop ON bs.DroppingStopId = dropping_stop.StopId
    INNER JOIN BookedSeats bseat ON bs.BookingSegmentId = bseat.BookingSegmentId
    
    WHERE b.BookingId = @BookingId
        AND b.Status = 2 -- Confirmed
        AND b.IsActive = 1
        AND p.PaymentStatus = 1 -- Success
        AND p.IsActive = 1
    
    ORDER BY bs.SegmentOrder ASC, bseat.SeatNumber ASC;
END