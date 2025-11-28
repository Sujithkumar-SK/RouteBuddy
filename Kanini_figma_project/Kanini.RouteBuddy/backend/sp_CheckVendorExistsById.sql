-- Stored Procedure: Check if Vendor Exists by ID
CREATE OR ALTER PROCEDURE sp_CheckVendorExistsById
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM Vendors
    WHERE VendorId = @VendorId
    AND IsActive = 1;
END