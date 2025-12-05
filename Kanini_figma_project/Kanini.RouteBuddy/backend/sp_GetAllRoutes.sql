CREATE PROCEDURE [dbo].[sp_GetAllRoutes]
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        RouteId,
        Source,
        Destination,
        Distance,
        Duration,
        BasePrice,
        IsActive,
        CreatedOn,
        CreatedBy,
        UpdatedOn,
        UpdatedBy
    FROM Routes
    WHERE IsActive = 1
    ORDER BY CreatedOn DESC, RouteId DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END