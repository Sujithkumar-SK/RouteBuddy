CREATE PROCEDURE [dbo].[sp_GetAllSeatLayoutTemplates]
AS
BEGIN
    SET NOCOUNT ON;
    
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
        slt.UpdatedOn,
        COUNT(sld.SeatLayoutDetailId) AS SeatDetailsCount
    FROM SeatLayoutTemplates slt
    LEFT JOIN SeatLayoutDetails sld ON slt.SeatLayoutTemplateId = sld.SeatLayoutTemplateId
    WHERE slt.IsActive = 1
    GROUP BY 
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
    ORDER BY slt.CreatedOn DESC; -- Recent records first
END