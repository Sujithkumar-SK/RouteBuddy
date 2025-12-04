CREATE PROCEDURE sp_GetSeatLayoutTemplatesByBusType
    @BusType INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        SeatLayoutTemplateId,
        TemplateName,
        TotalSeats,
        BusType,
        Description,
        IsActive,
        CreatedOn
    FROM SeatLayoutTemplates
    WHERE BusType = @BusType AND IsActive = 1
    ORDER BY CreatedOn DESC;
END