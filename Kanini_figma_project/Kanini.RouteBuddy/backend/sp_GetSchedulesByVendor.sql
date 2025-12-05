CREATE PROCEDURE [dbo].[sp_GetSchedulesByVendor]
    @VendorId INT,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        s.ScheduleId,
        s.BusId,
        s.RouteId,
        s.TravelDate,
        s.DepartureTime,
        s.ArrivalTime,
        s.AvailableSeats,
        s.Status,
        s.IsActive,
        b.BusName,
        r.Source,
        r.Destination
    FROM BusSchedules s
    INNER JOIN Buses b ON s.BusId = b.BusId
    INNER JOIN Routes r ON s.RouteId = r.RouteId
    WHERE b.VendorId = @VendorId 
        AND s.IsActive = 1
        AND b.IsActive = 1
        AND r.IsActive = 1
    ORDER BY s.CreatedOn DESC -- Recently added records first
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END