CREATE PROCEDURE [dbo].[sp_GetAllBusesForAdmin]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BusId,
        b.BusName,
        b.BusType,
        b.TotalSeats,
        b.RegistrationNo,
        b.Status,
        b.Amenities,
        b.DriverName,
        b.DriverContact,
        b.IsActive,
        b.CreatedOn,
        b.CreatedBy,
        b.UpdatedBy,
        b.SeatLayoutTemplateId,
        v.AgencyName AS VendorName,
        v.VendorId
    FROM Buses b
    INNER JOIN Vendors v ON b.VendorId = v.VendorId
    INNER JOIN Users u ON v.UserId = u.UserId
    ORDER BY b.CreatedOn DESC;
END