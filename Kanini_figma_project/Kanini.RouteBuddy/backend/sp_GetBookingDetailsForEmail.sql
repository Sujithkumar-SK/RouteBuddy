CREATE PROCEDURE [dbo].[sp_GetBookingDetailsForEmail]
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