-- Test Stored Procedure: Verify table structure
CREATE OR ALTER PROCEDURE sp_TestVendorTables
    @VendorId INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Test Buses table
    SELECT 'Buses Table Test' AS TestName, COUNT(*) AS RecordCount
    FROM Buses 
    WHERE VendorId = @VendorId;
    
    -- Test BusSchedules table
    SELECT 'BusSchedules Table Test' AS TestName, COUNT(*) AS RecordCount
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    WHERE b.VendorId = @VendorId;
    
    -- Test Bookings table
    SELECT 'Bookings Table Test' AS TestName, COUNT(*) AS RecordCount
    FROM Bookings bk
    INNER JOIN BookingSegments bs ON bk.BookingId = bs.BookingId
    INNER JOIN BusSchedules sch ON bs.ScheduleId = sch.ScheduleId
    INNER JOIN Buses b ON sch.BusId = b.BusId
    WHERE b.VendorId = @VendorId;
    
    -- Test Vendors table
    SELECT 'Vendors Table Test' AS TestName, COUNT(*) AS RecordCount
    FROM Vendors 
    WHERE VendorId = @VendorId;
END