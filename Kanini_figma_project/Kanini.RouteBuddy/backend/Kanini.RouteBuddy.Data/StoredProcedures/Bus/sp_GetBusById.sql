CREATE PROCEDURE [dbo].[sp_GetBusById]
    @BusId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input
        IF @BusId <= 0
        BEGIN
            SELECT 'ERROR' AS Result, 'INVALID_BUS_ID' AS ErrorMessage;
            RETURN;
        END
        
        -- Get bus data
        SELECT b.BusId, b.VendorId, b.BusName, b.RegistrationNo, b.BusType, b.TotalSeats, 
               b.Amenities, b.Status, b.IsActive, b.DriverName, b.DriverContact, 
               b.RegistrationPath, b.SeatLayoutTemplateId, b.CreatedBy, b.UpdatedBy, 
               v.AgencyName, b.CreatedOn
        FROM Buses b
        INNER JOIN Vendors v ON b.VendorId = v.VendorId
        WHERE b.BusId = @BusId AND b.IsActive = 1;
        
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'ERROR' AS Result, 'BUS_NOT_FOUND' AS ErrorMessage;
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