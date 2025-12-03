CREATE PROCEDURE sp_GetSeatLayoutTemplatesByBusType
    @BusType INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            SeatLayoutTemplateId,
            TemplateName,
            TotalSeats,
            BusType,
            Description,
            IsActive,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn,
            (SELECT COUNT(*) FROM SeatLayoutDetails WHERE SeatLayoutTemplateId = SLT.SeatLayoutTemplateId) as SeatDetailsCount
        FROM SeatLayoutTemplates SLT
        WHERE IsActive = 1 
        AND (@BusType IS NULL OR BusType = @BusType)
        ORDER BY CreatedOn DESC; -- Rule 9: Recent records first
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END