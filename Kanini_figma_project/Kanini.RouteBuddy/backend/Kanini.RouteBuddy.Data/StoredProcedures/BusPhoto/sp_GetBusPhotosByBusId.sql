CREATE PROCEDURE sp_GetBusPhotosByBusId
    @BusId INT
AS
BEGIN
    SELECT BusPhotoId, BusId, ImagePath, Caption, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn 
    FROM BusPhotos 
    WHERE BusId = @BusId 
    ORDER BY CreatedOn
END