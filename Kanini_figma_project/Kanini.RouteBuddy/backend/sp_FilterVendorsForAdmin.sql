CREATE PROCEDURE [dbo].[sp_FilterVendorsForAdmin]
    @SearchName NVARCHAR(100) = NULL,
    @IsActive BIT = NULL,
    @Status INT = NULL
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
    WHERE 
        (@SearchName IS NULL OR v.AgencyName LIKE '%' + @SearchName + '%' OR v.OwnerName LIKE '%' + @SearchName + '%')
        AND (@IsActive IS NULL OR v.IsActive = @IsActive)
        AND (@Status IS NULL OR v.Status = @Status)
    ORDER BY v.CreatedOn DESC;
END