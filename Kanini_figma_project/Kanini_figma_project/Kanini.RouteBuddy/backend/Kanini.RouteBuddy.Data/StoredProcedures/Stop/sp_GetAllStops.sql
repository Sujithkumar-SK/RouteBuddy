CREATE PROCEDURE sp_GetAllStops
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
        
        SELECT 
            StopId,
            Name,
            Landmark,
            IsActive,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Stops 
        WHERE IsActive = 1
        ORDER BY CreatedOn DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END