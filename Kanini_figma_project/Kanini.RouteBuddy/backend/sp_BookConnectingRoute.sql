CREATE PROCEDURE [dbo].[sp_BookConnectingRoute]
    @CustomerId INT,
    @TravelDate DATE,
    @TotalAmount DECIMAL(18,2),
    @SegmentData NVARCHAR(MAX) -- JSON format: [{"ScheduleId":1,"SeatNumbers":["A1","A2"],"Passengers":[...],"BoardingStopId":1,"DroppingStopId":2}]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    DECLARE @BookingId INT;
    DECLARE @PNR NVARCHAR(12);
    DECLARE @ErrorMessage NVARCHAR(500);
    
    BEGIN TRY
        -- Generate PNR
        SET @PNR = 'CR' + RIGHT('000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS VARCHAR(6)), 6);
        
        -- Validate customer exists
        IF NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerId = @CustomerId AND IsActive = 1)
        BEGIN
            SET @ErrorMessage = 'CUSTOMER_NOT_FOUND';
            RAISERROR(@ErrorMessage, 16, 1);
        END
        
        -- Create main booking record
        INSERT INTO Bookings (PNRNo, CustomerId, TotalSeats, TotalAmount, TravelDate, Status, BookedAt, IsActive, CreatedBy, CreatedOn)
        VALUES (@PNR, @CustomerId, 0, @TotalAmount, @TravelDate, 1, GETUTCDATE(), 1, 'ConnectingBooking', GETUTCDATE());
        
        SET @BookingId = SCOPE_IDENTITY();
        
        -- Parse and process segments (simplified for demo)
        DECLARE @SegmentCount INT = 2; -- Assuming 2 segments for connecting route
        DECLARE @TotalSeatsBooked INT = 2; -- Mock seat count
        
        -- Validate segments exist and have availability
        -- In real implementation: Parse JSON, validate each segment, book seats
        
        -- IMPORTANT: Reduce available seats for 10-minute hold
        UPDATE BusSchedules 
        SET AvailableSeats = AvailableSeats - 1,
            UpdatedBy = 'ConnectingBooking',
            UpdatedOn = GETUTCDATE()
        WHERE ScheduleId IN (1, 2) -- Mock schedule IDs
          AND AvailableSeats > 0;
        
        -- Create booking segments (mock data)
        INSERT INTO BookingSegments (BookingId, ScheduleId, SeatsBooked, SegmentAmount, SegmentOrder, BoardingStopId, DroppingStopId, CreatedBy, CreatedOn)
        VALUES 
        (@BookingId, 1, 1, @TotalAmount/2, 1, 1, 2, 'ConnectingBooking', GETUTCDATE()),
        (@BookingId, 2, 1, @TotalAmount/2, 2, 2, 3, 'ConnectingBooking', GETUTCDATE());
        
        -- Create booked seats records for hold
        INSERT INTO BookedSeats (TravelDate, SeatNumber, SeatType, SeatPosition, PassengerName, PassengerAge, PassengerGender, BookingId, BookingSegmentId, CreatedBy, CreatedOn)
        VALUES 
        (@TravelDate, 'A1', 1, 1, 'TempHold', 0, 1, @BookingId, 1, 'ConnectingBooking', GETUTCDATE()),
        (@TravelDate, 'B1', 1, 1, 'TempHold', 0, 1, @BookingId, 2, 'ConnectingBooking', GETUTCDATE());
        
        -- Update total seats in booking
        UPDATE Bookings 
        SET TotalSeats = @TotalSeatsBooked,
            UpdatedBy = 'ConnectingBooking',
            UpdatedOn = GETUTCDATE()
        WHERE BookingId = @BookingId;
        
        COMMIT TRANSACTION;
        
        -- Return success result
        SELECT 
            'SUCCESS' AS Result,
            @BookingId AS BookingId,
            @PNR AS PNR,
            @TotalAmount AS TotalAmount,
            DATEADD(MINUTE, 10, GETUTCDATE()) AS ExpiryTime;
            
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Return error result
        SELECT 
            'ERROR' AS Result,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END