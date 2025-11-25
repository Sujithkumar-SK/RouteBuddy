CREATE PROCEDURE sp_GetBusPhotoCountByBusId
    @BusId INT
AS
BEGIN
    SELECT COUNT(*) 
    FROM BusPhotos 
    WHERE BusId = @BusId
END