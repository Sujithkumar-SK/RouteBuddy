CREATE PROCEDURE sp_CheckScheduleExists
    @BusId INT,
    @RouteId INT,
    @TravelDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF @BusId IS NULL OR @BusId <= 0
        BEGIN
            RAISERROR('Invalid BusId parameter', 16, 1);
            RETURN;
        END
        
        IF @RouteId IS NULL OR @RouteId <= 0
        BEGIN
            RAISERROR('Invalid RouteId parameter', 16, 1);
            RETURN;
        END
        
        IF @TravelDate IS NULL
        BEGIN
            RAISERROR('Invalid TravelDate parameter', 16, 1);
            RETURN;
        END
        
        SELECT COUNT(*)
        FROM BusSchedules
        WHERE BusId = @BusId 
        AND RouteId = @RouteId 
        AND CAST(TravelDate AS DATE) = @TravelDate;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END