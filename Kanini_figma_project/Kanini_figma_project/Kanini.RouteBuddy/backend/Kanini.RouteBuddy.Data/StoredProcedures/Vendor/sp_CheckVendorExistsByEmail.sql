CREATE PROCEDURE sp_CheckVendorExistsByEmail
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS EmailExists
    FROM Vendors v 
    INNER JOIN Users u ON v.UserId = u.UserId 
    WHERE u.Email = @Email;
END