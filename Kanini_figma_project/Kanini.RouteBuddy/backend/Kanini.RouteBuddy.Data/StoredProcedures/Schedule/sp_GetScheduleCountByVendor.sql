CREATE PROCEDURE sp_GetScheduleCountByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF @VendorId IS NULL OR @VendorId <= 0
        BEGIN
            RAISERROR('Invalid VendorId parameter', 16, 1);
            RETURN;
        END
        
        SELECT COUNT(*)
        FROM BusSchedules bs
        INNER JOIN Buses b ON bs.BusId = b.BusId
        WHERE b.VendorId = @VendorId 
        AND bs.IsActive = 1;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END