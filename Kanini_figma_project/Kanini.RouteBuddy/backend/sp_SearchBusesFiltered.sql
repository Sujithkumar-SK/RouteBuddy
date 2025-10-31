CREATE PROCEDURE [dbo].[sp_SearchBusesFiltered]
    @Source NVARCHAR(100),
    @Destination NVARCHAR(100),
    @TravelDate DATE,
    @BusTypes NVARCHAR(MAX) = NULL,
    @Amenities NVARCHAR(MAX) = NULL,
    @DepartureTimeFrom TIME = NULL,
    @DepartureTimeTo TIME = NULL,
    @MinPrice DECIMAL(18,2) = NULL,
    @MaxPrice DECIMAL(18,2) = NULL,
    @SortBy NVARCHAR(50) = 'time_asc'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Base query with filters
    SELECT 
        bs.ScheduleId,
        b.BusId,
        b.BusName,
        b.BusType,
        b.TotalSeats,
        bs.AvailableSeats,
        r.Source,
        r.Destination,
        bs.TravelDate,
        bs.DepartureTime,
        bs.ArrivalTime,
        r.BasePrice,
        b.Amenities,
        v.AgencyName AS VendorName,
        DATEDIFF(MINUTE, bs.DepartureTime, bs.ArrivalTime) AS DurationMinutes
    FROM BusSchedules bs
    INNER JOIN Buses b ON bs.BusId = b.BusId
    INNER JOIN Routes r ON bs.RouteId = r.RouteId
    INNER JOIN Vendors v ON b.VendorId = v.VendorId
    WHERE r.Source = @Source
        AND r.Destination = @Destination
        AND bs.TravelDate = @TravelDate
        AND bs.Status = 1 -- Scheduled
        AND bs.IsActive = 1
        AND b.Status = 1 -- Active
        AND b.IsActive = 1
        AND v.Status = 1 -- Active
        AND v.IsActive = 1
        AND bs.AvailableSeats > 0 -- Only show buses with available seats
        -- Bus Type Filter
        AND (@BusTypes IS NULL OR b.BusType IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(@BusTypes, ',')))
        -- Amenities Filter (bitwise AND for flags)
        AND (@Amenities IS NULL OR EXISTS (
            SELECT 1 FROM STRING_SPLIT(@Amenities, ',') 
            WHERE (b.Amenities & CAST(value AS INT)) = CAST(value AS INT)
        ))
        -- Time Range Filter
        AND (@DepartureTimeFrom IS NULL OR bs.DepartureTime >= @DepartureTimeFrom)
        AND (@DepartureTimeTo IS NULL OR bs.DepartureTime <= @DepartureTimeTo)
        -- Price Range Filter
        AND (@MinPrice IS NULL OR r.BasePrice >= @MinPrice)
        AND (@MaxPrice IS NULL OR r.BasePrice <= @MaxPrice)
    ORDER BY 
        CASE @SortBy
            WHEN 'price_asc' THEN r.BasePrice
            WHEN 'price_desc' THEN r.BasePrice
            WHEN 'time_asc' THEN DATEPART(HOUR, bs.DepartureTime) * 60 + DATEPART(MINUTE, bs.DepartureTime)
            WHEN 'duration_asc' THEN DATEDIFF(MINUTE, bs.DepartureTime, bs.ArrivalTime)
            ELSE bs.ScheduleId
        END ASC,
        CASE @SortBy
            WHEN 'price_desc' THEN r.BasePrice
        END DESC,
        bs.CreatedOn DESC; -- Recently added first as default
END