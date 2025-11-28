-- Update stored procedures for admin functionality

-- 1. Update sp_GetPendingVendors
DROP PROCEDURE IF EXISTS sp_GetPendingVendors;
GO
CREATE PROCEDURE sp_GetPendingVendors
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
        v.Status,
        v.IsActive,
        v.CreatedOn,
        u.Email,
        u.Phone
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.Status = 0 -- PendingApproval
    ORDER BY v.CreatedOn DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- 2. Update sp_GetAllVendors
DROP PROCEDURE IF EXISTS sp_GetAllVendors;
GO
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
GO

-- 3. Update sp_GetVendorTotalCount
DROP PROCEDURE IF EXISTS sp_GetVendorTotalCount;
GO
CREATE PROCEDURE sp_GetVendorTotalCount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS TotalCount
    FROM Vendors;
END
GO

-- Test the procedures
EXEC sp_GetPendingVendors @Offset = 0, @PageSize = 10;
EXEC sp_GetAllVendors @Offset = 0, @PageSize = 10;
EXEC sp_GetVendorTotalCount;