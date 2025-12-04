CREATE PROCEDURE sp_GetStopById
    @StopId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
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
        WHERE StopId = @StopId AND IsActive = 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END