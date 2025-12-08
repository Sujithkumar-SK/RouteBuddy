CREATE PROCEDURE sp_GetAllActiveRoutes
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            r.RouteId,
            r.Source,
            r.Destination,
            r.Distance,
            r.Duration,
            r.BasePrice,
            COUNT(rs.RouteStopId) as TotalStops,
            r.IsActive
        FROM Routes r
        LEFT JOIN RouteStops rs ON r.RouteId = rs.RouteId
        WHERE r.IsActive = 1
        GROUP BY r.RouteId, r.Source, r.Destination, r.Distance, r.Duration, r.BasePrice, r.IsActive, r.CreatedOn
        ORDER BY r.CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END