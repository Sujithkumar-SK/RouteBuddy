CREATE PROCEDURE sp_ApplyTemplateToLayout
    @BusId INT,
    @TemplateId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Update bus with template ID
        UPDATE Buses 
        SET SeatLayoutTemplateId = @TemplateId,
            UpdatedOn = GETUTCDATE(),
            UpdatedBy = 'System'
        WHERE BusId = @BusId;
        
        COMMIT TRANSACTION;
        
        SELECT 1 as Success;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END