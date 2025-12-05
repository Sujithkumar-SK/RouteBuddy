-- Stored Procedure: Check if Vendor Exists by Email
CREATE OR ALTER PROCEDURE sp_CheckVendorExistsByEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE u.Email = @Email
    AND v.IsActive = 1;
END