CREATE PROCEDURE [dbo].[sp_GetSeatLayoutTemplateById]
    @SeatLayoutTemplateId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return template basic info
    SELECT 
        slt.SeatLayoutTemplateId,
        slt.TemplateName,
        slt.TotalSeats,
        slt.BusType,
        slt.Description,
        slt.IsActive,
        slt.CreatedBy,
        slt.CreatedOn,
        slt.UpdatedBy,
        slt.UpdatedOn
    FROM SeatLayoutTemplates slt
    WHERE slt.SeatLayoutTemplateId = @SeatLayoutTemplateId
        AND slt.IsActive = 1;
    
    -- Return seat layout details
    SELECT 
        sld.SeatLayoutDetailId,
        sld.SeatLayoutTemplateId,
        sld.SeatNumber,
        sld.SeatType,
        sld.SeatPosition,
        sld.RowNumber,
        sld.ColumnNumber,
        sld.PriceTier,
        sld.CreatedBy,
        sld.CreatedOn,
        sld.UpdatedBy,
        sld.UpdatedOn
    FROM SeatLayoutDetails sld
    WHERE sld.SeatLayoutTemplateId = @SeatLayoutTemplateId
    ORDER BY sld.RowNumber ASC, sld.ColumnNumber ASC;
END