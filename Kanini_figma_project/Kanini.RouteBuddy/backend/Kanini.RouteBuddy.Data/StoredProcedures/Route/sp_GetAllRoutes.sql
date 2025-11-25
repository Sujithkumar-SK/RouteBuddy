CREATE PROCEDURE [dbo].[sp_GetAllRoutes]
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input parameters
        IF @PageNumber <= 0 OR @PageSize <= 0
        BEGIN
            SELECT 'ERROR' AS Result, 'INVALID_PAGINATION_PARAMETERS' AS ErrorMessage;
            RETURN;
        END
        
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
        
        -- Get paginated routes
        SELECT RouteId, Source, Destination, Distance, Duration, BasePrice, IsActive 
        FROM Routes 
        WHERE IsActive = 1 
        ORDER BY CreatedOn DESC 
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        
        -- Get total count for pagination
        SELECT COUNT(*) AS TotalCount FROM Routes WHERE IsActive = 1;
        
        SELECT 'SUCCESS' AS Result;
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END