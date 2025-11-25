CREATE PROCEDURE sp_GetVendorTotalCount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS TotalCount
    FROM Vendors 
    WHERE IsActive = 1;
END