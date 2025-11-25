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
        v.IsActive, 
        v.Status, 
        u.Email, 
        u.Phone
    FROM Vendors v 
    INNER JOIN Users u ON v.UserId = u.UserId 
    WHERE v.IsActive = 1
    ORDER BY v.VendorId 
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END