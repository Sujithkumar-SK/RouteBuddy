CREATE PROCEDURE sp_CheckBusPhotoExists
    @BusPhotoId INT
AS
BEGIN
    SELECT COUNT(1) 
    FROM BusPhotos 
    WHERE BusPhotoId = @BusPhotoId
END