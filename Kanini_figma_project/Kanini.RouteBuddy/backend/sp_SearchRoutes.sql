-- Stored Procedure: Search Routes with Real-time Filtering
CREATE OR ALTER PROCEDURE sp_SearchRoutes
    @Source NVARCHAR(100) = NULL,
    @Destination NVARCHAR(100) = NULL,
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate parameters
    IF @Limit <= 0 OR @Limit > 50
        SET @Limit = 10;
    
    SELECT TOP (@Limit)
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
        AND (@Source IS NULL OR r.Source LIKE '%' + @Source + '%')
        AND (@Destination IS NULL OR r.Destination LIKE '%' + @Destination + '%')
    GROUP BY r.RouteId, r.Source, r.Destination, r.Distance, r.Duration, r.BasePrice, r.IsActive, r.CreatedOn
    ORDER BY r.CreatedOn DESC;
END