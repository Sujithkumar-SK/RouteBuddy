CREATE PROCEDURE sp_GetVendorById
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.VendorId, 
        v.UserId, 
        v.AgencyName, 
        v.OwnerName, 
        v.BusinessLicenseNumber,
        v.OfficeAddress, 
        v.FleetSize, 
        v.TaxRegistrationNumber, 
        v.Status, 
        v.IsActive,
        u.Email, 
        u.Phone
    FROM Vendors v 
    INNER JOIN Users u ON v.UserId = u.UserId 
    WHERE v.VendorId = @VendorId;
END