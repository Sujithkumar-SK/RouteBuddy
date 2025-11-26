CREATE PROCEDURE sp_GetPendingVendorsCount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS PendingCount
    FROM Vendors 
    WHERE Status = 0; -- PendingApproval
END