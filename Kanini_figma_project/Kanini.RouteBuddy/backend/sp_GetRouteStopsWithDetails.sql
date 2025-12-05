-- Stored Procedure: Get Route Stops with Detailed Information
CREATE OR ALTER PROCEDURE sp_GetRouteStopsWithDetails
    @RouteId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate route exists
    IF NOT EXISTS (SELECT 1 FROM Routes WHERE RouteId = @RouteId AND IsActive = 1)
    BEGIN
        RAISERROR('Route not found or inactive', 16, 1);
        RETURN;
    END
    
    SELECT 
        rs.RouteStopId,
        rs.StopId,
        s.Name as StopName,
        s.Landmark,
        rs.OrderNumber,
        rs.ArrivalTime,
        rs.DepartureTime
    FROM RouteStops rs
    INNER JOIN Stops s ON rs.StopId = s.StopId
    WHERE rs.RouteId = @RouteId
        AND s.IsActive = 1
    ORDER BY rs.OrderNumber ASC;
END