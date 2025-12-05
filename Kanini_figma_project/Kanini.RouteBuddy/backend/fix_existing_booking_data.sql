-- Fix existing booking data for amber user
-- This script corrects the CustomerId in the existing booking

-- First, let's check the current state
SELECT 'Before Fix - Booking Data' AS CheckType,
    b.BookingId, b.CustomerId, b.PNRNo, 
    c.FirstName, c.LastName, u.Email
FROM Bookings b
LEFT JOIN Customers c ON b.CustomerId = c.CustomerId
LEFT JOIN Users u ON c.UserId = u.UserId
WHERE b.BookingId = 3006;

-- Update the booking to use the correct CustomerId (3001 for amber)
UPDATE Bookings 
SET CustomerId = 3001,
    UpdatedBy = 'DataFix',
    UpdatedOn = GETUTCDATE()
WHERE BookingId = 3006;

-- Verify the fix
SELECT 'After Fix - Booking Data' AS CheckType,
    b.BookingId, b.CustomerId, b.PNRNo, 
    c.FirstName, c.LastName, u.Email
FROM Bookings b
LEFT JOIN Customers c ON b.CustomerId = c.CustomerId
LEFT JOIN Users u ON c.UserId = u.UserId
WHERE b.BookingId = 3006;

-- Test the stored procedures after fix
PRINT 'Testing Email Procedure after fix...';
EXEC sp_GetBookingDetailsForEmail @BookingId = 3006;

PRINT 'Testing Customer Bookings after fix...';
EXEC sp_GetCustomerBookings @CustomerId = 3001;

PRINT 'Data fix completed successfully!';