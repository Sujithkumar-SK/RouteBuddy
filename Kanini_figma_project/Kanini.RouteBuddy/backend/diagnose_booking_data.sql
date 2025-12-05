-- Diagnostic queries to check booking data integrity

-- 1. Check the booking and customer relationship
SELECT 
    'Booking-Customer Relationship' AS CheckType,
    b.BookingId,
    b.CustomerId,
    b.PNRNo,
    b.Status,
    b.IsActive AS BookingActive,
    c.CustomerId AS CustomerIdFromTable,
    c.FirstName,
    c.LastName,
    c.UserId,
    c.IsActive AS CustomerActive,
    u.UserId AS UserIdFromTable,
    u.Email,
    u.Phone,
    u.IsActive AS UserActive
FROM Bookings b
LEFT JOIN Customers c ON b.CustomerId = c.CustomerId
LEFT JOIN Users u ON c.UserId = u.UserId
WHERE b.BookingId = 3006;

-- 2. Check booking segments
SELECT 
    'Booking Segments' AS CheckType,
    bs.BookingSegmentId,
    bs.BookingId,
    bs.ScheduleId,
    bs.SeatsBooked,
    bs.SegmentAmount,
    sch.IsActive AS ScheduleActive,
    bus.IsActive AS BusActive,
    r.IsActive AS RouteActive
FROM BookingSegments bs
LEFT JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
LEFT JOIN Buses bus ON sch.BusId = bus.BusId
LEFT JOIN Routes r ON sch.RouteId = r.RouteId
WHERE bs.BookingId = 3006;

-- 3. Check booked seats
SELECT 
    'Booked Seats' AS CheckType,
    bst.BookedSeatId,
    bst.SeatNumber,
    bst.PassengerName,
    bst.PassengerAge,
    bst.PassengerGender,
    bst.BookingSegmentId
FROM BookedSeats bst
INNER JOIN BookingSegments bs ON bst.BookingSegmentId = bs.BookingSegmentId
WHERE bs.BookingId = 3006;

-- 4. Check payment
SELECT 
    'Payment Details' AS CheckType,
    p.PaymentId,
    p.BookingId,
    p.Amount,
    p.PaymentMethod,
    p.PaymentStatus,
    p.TransactionId,
    p.IsActive AS PaymentActive
FROM Payments p
WHERE p.BookingId = 3006;

-- 5. Check if customer 3001 has any bookings at all
SELECT 
    'All Customer Bookings' AS CheckType,
    COUNT(*) AS TotalBookings,
    COUNT(CASE WHEN b.Status = 1 THEN 1 END) AS PendingBookings,
    COUNT(CASE WHEN b.Status = 2 THEN 1 END) AS ConfirmedBookings,
    COUNT(CASE WHEN b.Status = 3 THEN 1 END) AS CancelledBookings
FROM Bookings b
WHERE b.CustomerId = 3001 AND b.IsActive = 1;

-- 6. List all bookings for customer 3001
SELECT 
    'Customer 3001 Bookings List' AS CheckType,
    b.BookingId,
    b.PNRNo,
    b.Status,
    b.TotalAmount,
    b.TravelDate,
    b.BookedAt,
    b.IsActive
FROM Bookings b
WHERE b.CustomerId = 3001
ORDER BY b.BookedAt DESC;