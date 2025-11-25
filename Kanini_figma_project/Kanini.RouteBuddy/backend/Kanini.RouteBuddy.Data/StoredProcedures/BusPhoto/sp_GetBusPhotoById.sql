CREATE PROCEDURE sp_GetBusPhotoById
    @BusPhotoId INT
AS
BEGIN
    SELECT BusPhotoId, BusId, ImagePath, Caption, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn 
    FROM BusPhotos 
    WHERE BusPhotoId = @BusPhotoId
END