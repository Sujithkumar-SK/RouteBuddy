CREATE PROCEDURE [dbo].[sp_GetBusDetailsForAdmin]
    @BusId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BusId,
        b.BusName,
        b.BusType,
        b.TotalSeats,
        b.RegistrationNo,
        b.RegistrationPath,
        b.Status,
        b.Amenities,
        b.DriverName,
        b.DriverContact,
        b.IsActive,
        b.CreatedOn,
        b.CreatedBy,
        b.UpdatedBy,
        b.UpdatedOn,
        b.SeatLayoutTemplateId,
        v.AgencyName AS VendorName,
        v.VendorId,
        v.OwnerName,
        u.Email AS VendorEmail,
        u.Phone AS VendorPhone
    FROM Buses b
    INNER JOIN Vendors v ON b.VendorId = v.VendorId
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE b.BusId = @BusId;
END