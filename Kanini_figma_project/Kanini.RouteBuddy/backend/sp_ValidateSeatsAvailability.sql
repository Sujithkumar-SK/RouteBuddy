CREATE PROCEDURE [dbo].[sp_ValidateSeatsAvailability]
    @ScheduleId INT,
    @TravelDate DATE,
    @SeatNumbers NVARCHAR(MAX) -- Comma-separated seat numbers
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Split seat numbers and validate availability
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