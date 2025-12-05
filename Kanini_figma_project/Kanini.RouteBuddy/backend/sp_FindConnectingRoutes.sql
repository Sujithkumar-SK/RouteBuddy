CREATE PROCEDURE [dbo].[sp_FindConnectingRoutes]
    @Source NVARCHAR(100),
    @Destination NVARCHAR(100),
    @TravelDate DATE,
    @Toggle NVARCHAR(20) = 'cheapest'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Find connecting routes with 1-hour buffer
    WITH ConnectingRoutes AS (
        -- First leg routes
        SELECT 
            bs1.ScheduleId AS FirstScheduleId,
            b1.BusName AS FirstBusName,
            r1.Source AS FirstSource,
            r1.Destination AS FirstDestination,
            bs1.TravelDate AS FirstTravelDate,
            bs1.DepartureTime AS FirstDepartureTime,
            bs1.ArrivalTime AS FirstArrivalTime,
            r1.BasePrice AS FirstPrice,
            v1.AgencyName AS FirstVendorName,
            bs1.AvailableSeats AS FirstAvailableSeats,
            
            -- Second leg routes
            bs2.ScheduleId AS SecondScheduleId,
            b2.BusName AS SecondBusName,
            r2.Source AS SecondSource,
            r2.Destination AS SecondDestination,
            bs2.TravelDate AS SecondTravelDate,
            bs2.DepartureTime AS SecondDepartureTime,
            bs2.ArrivalTime AS SecondArrivalTime,
            r2.BasePrice AS SecondPrice,
            v2.AgencyName AS SecondVendorName,
            bs2.AvailableSeats AS SecondAvailableSeats,
            
            -- Calculated fields
            (r1.BasePrice + r2.BasePrice) AS TotalPrice,
            DATEDIFF(MINUTE, bs1.DepartureTime, bs2.ArrivalTime) AS TotalDurationMinutes,
            CASE WHEN bs1.AvailableSeats < bs2.AvailableSeats THEN bs1.AvailableSeats ELSE bs2.AvailableSeats END AS MinAvailableSeats
            
        FROM BusSchedules bs1
        INNER JOIN Buses b1 ON bs1.BusId = b1.BusId
        INNER JOIN Routes r1 ON bs1.RouteId = r1.RouteId
        INNER JOIN Vendors v1 ON b1.VendorId = v1.VendorId
        
        INNER JOIN BusSchedules bs2 ON bs1.TravelDate = bs2.TravelDate
        INNER JOIN Buses b2 ON bs2.BusId = b2.BusId
        INNER JOIN Routes r2 ON bs2.RouteId = r2.RouteId AND r1.Destination = r2.Source
        INNER JOIN Vendors v2 ON b2.VendorId = v2.VendorId
        
        WHERE r1.Source = @Source
            AND r2.Destination = @Destination
            AND bs1.TravelDate = @TravelDate
            AND bs2.TravelDate = @TravelDate
            AND bs1.Status = 1 AND bs1.IsActive = 1 AND bs1.AvailableSeats > 0
            AND bs2.Status = 1 AND bs2.IsActive = 1 AND bs2.AvailableSeats > 0
            AND b1.Status = 1 AND b1.IsActive = 1
            AND b2.Status = 1 AND b2.IsActive = 1
            AND v1.Status = 1 AND v1.IsActive = 1
            AND v2.Status = 1 AND v2.IsActive = 1
            -- 1-hour buffer between arrival and departure
            AND DATEDIFF(MINUTE, bs1.ArrivalTime, bs2.DepartureTime) >= 60
    )
    
    SELECT 
        FirstScheduleId, FirstBusName, FirstSource, FirstDestination,
        FirstTravelDate, FirstDepartureTime, FirstArrivalTime, FirstPrice,
        FirstVendorName, FirstAvailableSeats,
        SecondScheduleId, SecondBusName, SecondSource, SecondDestination,
        SecondTravelDate, SecondDepartureTime, SecondArrivalTime, SecondPrice,
        SecondVendorName, SecondAvailableSeats,
        TotalPrice, TotalDurationMinutes, MinAvailableSeats,
        CONCAT(FirstSource, ' → ', FirstDestination, ' → ', SecondDestination) AS RouteDescription
    FROM ConnectingRoutes
    ORDER BY 
        CASE 
            WHEN @Toggle = 'cheapest' THEN TotalPrice
            WHEN @Toggle = 'fastest' THEN TotalDurationMinutes
            ELSE TotalPrice
        END ASC;
END