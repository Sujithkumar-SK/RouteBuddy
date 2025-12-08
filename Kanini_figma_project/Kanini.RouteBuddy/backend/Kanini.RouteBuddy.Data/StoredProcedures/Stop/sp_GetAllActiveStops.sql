CREATE PROCEDURE [dbo].[sp_GetAllActiveStops]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            StopId,
            Name,
            Landmark,
            IsActive,
            CreatedOn
        FROM Stops 
        WHERE IsActive = 1
        ORDER BY CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END