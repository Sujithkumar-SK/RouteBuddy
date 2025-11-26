CREATE PROCEDURE sp_CheckVendorExistsById
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS VendorExists
    FROM Vendors 
    WHERE VendorId = @VendorId AND IsActive = 1;
END