CREATE PROCEDURE [dbo].[sp_GetRouteById]
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
        
        -- Get route data
        SELECT RouteId, Source, Destination, Distance, Duration, BasePrice, IsActive 
        FROM Routes 
        WHERE RouteId = @RouteId AND IsActive = 1;
        
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'ERROR' AS Result, 'ROUTE_NOT_FOUND' AS ErrorMessage;
        END
        ELSE
        BEGIN
            SELECT 'SUCCESS' AS Result;
        END
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END