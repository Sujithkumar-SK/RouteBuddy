CREATE PROCEDURE [dbo].[sp_ValidateSeatsAndStops]
    @ScheduleId INT,
    @TravelDate DATE,
    @SeatNumbers NVARCHAR(MAX), -- Comma-separated seat numbers
    @BoardingStopId INT,
    @DroppingStopId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate stops exist in route and order is correct
    DECLARE @BoardingOrder INT, @DroppingOrder INT;
    
    SELECT @BoardingOrder = rs.OrderNumber
    FROM BusSchedules bs
    INNER JOIN RouteStops rs ON bs.ScheduleId = rs.ScheduleId
    WHERE bs.ScheduleId = @ScheduleId AND rs.RouteStopId = @BoardingStopId;
    
    SELECT @DroppingOrder = rs.OrderNumber
    FROM BusSchedules bs
    INNER JOIN RouteStops rs ON bs.ScheduleId = rs.ScheduleId
    WHERE bs.ScheduleId = @ScheduleId AND rs.RouteStopId = @DroppingStopId;
    
    -- Check if stops are valid
    IF @BoardingOrder IS NULL OR @DroppingOrder IS NULL
    BEGIN
        SELECT 'INVALID_STOPS' AS ValidationResult;
        RETURN;
    END
    
    -- Check if dropping stop is after boarding stop
    IF @DroppingOrder <= @BoardingOrder
    BEGIN
        SELECT 'INVALID_STOP_ORDER' AS ValidationResult;
        RETURN;
    END
    
    -- Validate seat availability (reuse existing logic)
    SELECT 
        sld.SeatNumber,
        sld.SeatType,
        sld.SeatPosition,
        sld.PriceTier,
        r.BasePrice,
        b.BusType,
        b.Amenities,
        CASE WHEN bks.BookedSeatId IS NOT NULL THEN 1 ELSE 0 END AS IsBooked
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    INNER JOIN Routes r ON bs.RouteId = r.RouteId
    INNER JOIN SeatLayoutTemplates slt ON b.SeatLayoutTemplateId = slt.SeatLayoutTemplateId
    INNER JOIN SeatLayoutDetails sld ON slt.SeatLayoutTemplateId = sld.SeatLayoutTemplateId
    LEFT JOIN BookedSeats bks ON bks.SeatNumber = sld.SeatNumber 
        AND bks.TravelDate = @TravelDate 
        AND EXISTS (
            SELECT 1 FROM BookingSegments bsg 
            INNER JOIN Bookings bk ON bsg.BookingId = bk.BookingId
            WHERE bsg.ScheduleId = @ScheduleId 
            AND bks.BookingSegmentId = bsg.BookingSegmentId
            AND bk.Status IN (1, 2) -- Pending or Confirmed
            AND bk.IsActive = 1
        )
    WHERE bs.ScheduleId = @ScheduleId
        AND bs.TravelDate = @TravelDate
        AND bs.Status = 1 -- Scheduled
        AND bs.IsActive = 1
        AND b.Status = 1 -- Active
        AND b.IsActive = 1
        AND sld.SeatNumber IN (SELECT value FROM STRING_SPLIT(@SeatNumbers, ','))
    ORDER BY sld.RowNumber, sld.ColumnNumber;
END