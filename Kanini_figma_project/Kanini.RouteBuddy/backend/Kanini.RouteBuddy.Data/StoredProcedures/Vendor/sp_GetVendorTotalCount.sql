CREATE PROCEDURE sp_GetVendorTotalCount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS TotalCount
    FROM Vendors;
END