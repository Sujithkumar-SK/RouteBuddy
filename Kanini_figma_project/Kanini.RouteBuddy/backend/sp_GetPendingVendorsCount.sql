CREATE PROCEDURE sp_GetPendingVendorsCount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) 
    FROM Vendors 
    WHERE Status = 0; -- Pending status
END