CREATE PROCEDURE [dbo].[sp_GetScheduleById]
    @ScheduleId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input
        IF @ScheduleId <= 0
        BEGIN
            SELECT 'ERROR' AS Result, 'INVALID_SCHEDULE_ID' AS ErrorMessage;
            RETURN;
        END
        
        -- Get schedule data
        SELECT ScheduleId, BusId, RouteId, TravelDate, DepartureTime, ArrivalTime, 
               AvailableSeats, IsActive
        FROM BusSchedules 
        WHERE ScheduleId = @ScheduleId AND IsActive = 1;
        
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'ERROR' AS Result, 'SCHEDULE_NOT_FOUND' AS ErrorMessage;
        END
        ELSE
        BEGIN
            SELECT 'SUCCESS' AS Result;
        END
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END