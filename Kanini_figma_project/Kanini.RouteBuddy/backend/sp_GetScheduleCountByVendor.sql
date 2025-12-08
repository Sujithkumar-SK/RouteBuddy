CREATE PROCEDURE [dbo].[sp_GetScheduleCountByVendor]
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM BusSchedules s
    INNER JOIN Buses b ON s.BusId = b.BusId
    WHERE b.VendorId = @VendorId 
        AND s.IsActive = 1
        AND b.IsActive = 1;
END