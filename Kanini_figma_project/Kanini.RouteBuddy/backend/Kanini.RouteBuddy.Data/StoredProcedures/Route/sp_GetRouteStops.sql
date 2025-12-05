CREATE PROCEDURE [dbo].[sp_GetRouteStops]
    @RouteId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input
        IF @RouteId <= 0
        BEGIN
            SELECT 'ERROR' AS Result, 'INVALID_ROUTE_ID' AS ErrorMessage;
            RETURN;
        END
        
        -- Check if route exists
        IF NOT EXISTS (SELECT 1 FROM Routes WHERE RouteId = @RouteId AND IsActive = 1)
        BEGIN
            SELECT 'ERROR' AS Result, 'ROUTE_NOT_FOUND' AS ErrorMessage;
            RETURN;
        END
        
        -- Get route stops
        SELECT rs.RouteStopId, rs.StopId, rs.OrderNumber, rs.ArrivalTime, rs.DepartureTime,
               s.Name AS StopName, s.Landmark
        FROM RouteStops rs
        INNER JOIN Stops s ON rs.StopId = s.StopId
        WHERE rs.RouteId = @RouteId
        ORDER BY rs.OrderNumber;
        
        SELECT 'SUCCESS' AS Result;
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END