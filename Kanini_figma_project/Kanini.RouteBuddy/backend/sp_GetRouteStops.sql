CREATE PROCEDURE [dbo].[sp_GetRouteStops]
    @ScheduleId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        rs.RouteStopId,
        rs.StopId,
        s.Name AS StopName,
        s.Landmark,
        rs.OrderNumber,
        rs.ArrivalTime,
        rs.DepartureTime
    FROM BusSchedules bs
    INNER JOIN RouteStops rs ON bs.ScheduleId = rs.ScheduleId
    INNER JOIN Stops s ON rs.StopId = s.StopId
    WHERE bs.ScheduleId = @ScheduleId
        AND bs.Status = 1 -- Scheduled
        AND bs.IsActive = 1
        AND s.IsActive = 1
    ORDER BY rs.OrderNumber ASC;
END