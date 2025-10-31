CREATE PROCEDURE [dbo].[sp_SearchBuses]
    @Source NVARCHAR(100),
    @Destination NVARCHAR(100),
    @TravelDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all results ordered by CreatedOn DESC (recently added first)
    SELECT 
        bs.ScheduleId,
        b.BusId,
        b.BusName,
        b.BusType,
        b.TotalSeats,
        bs.AvailableSeats,
        r.Source,
        r.Destination,
        bs.TravelDate,
        bs.DepartureTime,
        bs.ArrivalTime,
        r.BasePrice,
        b.Amenities,
        v.AgencyName AS VendorName
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    INNER JOIN Routes r ON bs.RouteId = r.RouteId
    INNER JOIN Vendors v ON b.VendorId = v.VendorId
    WHERE r.Source = @Source
        AND r.Destination = @Destination
        AND bs.TravelDate = @TravelDate
        AND bs.Status = 1 -- Scheduled
        AND bs.IsActive = 1
        AND b.Status = 1 -- Active
        AND b.IsActive = 1
        AND v.Status = 1 -- Active
        AND v.IsActive = 1
        AND bs.AvailableSeats > 0 -- Only show buses with available seats
    ORDER BY bs.CreatedOn DESC;
END