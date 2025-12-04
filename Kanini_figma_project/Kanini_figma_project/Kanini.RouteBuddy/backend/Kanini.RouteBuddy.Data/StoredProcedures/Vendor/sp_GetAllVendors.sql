CREATE PROCEDURE sp_GetAllVendors
    @Offset INT,
    @PageSize INT
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
        v.IsActive, 
        v.Status,
        v.CreatedOn,
        u.Email, 
        u.Phone
    FROM Vendors v 
    INNER JOIN Users u ON v.UserId = u.UserId 
    ORDER BY v.CreatedOn DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END