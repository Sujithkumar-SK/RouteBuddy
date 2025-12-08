CREATE PROCEDURE [dbo].[sp_GetBusSeatLayout]
    @ScheduleId INT,
    @TravelDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return bus info first
    SELECT 
        bs.ScheduleId,
        b.BusName,
        b.BusType,
        b.Amenities AS BusAmenities,
        b.TotalSeats,
        bs.AvailableSeats,
        (b.TotalSeats - bs.AvailableSeats) AS BookedSeats,
        r.BasePrice
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    INNER JOIN Routes r ON bs.RouteId = r.RouteId
    WHERE bs.ScheduleId = @ScheduleId
        AND bs.TravelDate = @TravelDate
        AND bs.Status = 1 -- Scheduled
        AND bs.IsActive = 1
        AND b.Status = 1 -- Active
        AND b.IsActive = 1;
    
    -- Return seat layout with booking status
    SELECT 
        sld.SeatNumber,
        sld.SeatType,
        sld.SeatPosition,
        sld.RowNumber,
        sld.ColumnNumber,
        sld.PriceTier,
        CASE WHEN bks.BookedSeatId IS NOT NULL THEN 1 ELSE 0 END AS IsBooked
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
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
    ORDER BY sld.RowNumber, sld.ColumnNumber;
END