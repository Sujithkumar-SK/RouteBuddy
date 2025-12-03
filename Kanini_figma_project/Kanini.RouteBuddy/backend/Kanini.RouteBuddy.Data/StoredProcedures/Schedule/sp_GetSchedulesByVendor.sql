CREATE PROCEDURE [dbo].[sp_GetSchedulesByVendor]
    @VendorId INT,
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input parameters
        IF @VendorId <= 0 OR @PageNumber <= 0 OR @PageSize <= 0
        BEGIN
            SELECT 'ERROR' AS Result, 'INVALID_PARAMETERS' AS ErrorMessage;
            RETURN;
        END
        
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
        
        -- Get paginated schedules with bus and route information
        SELECT s.ScheduleId, s.BusId, b.BusName, s.RouteId, r.Source, r.Destination,
               s.TravelDate, s.DepartureTime, s.ArrivalTime, s.AvailableSeats, 
               s.Status, s.IsActive
        FROM BusSchedules s
        INNER JOIN Buses b ON s.BusId = b.BusId
        INNER JOIN Routes r ON s.RouteId = r.RouteId
        WHERE b.VendorId = @VendorId AND s.IsActive = 1 
        ORDER BY s.CreatedOn DESC 
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        
        SELECT 'SUCCESS' AS Result;
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END