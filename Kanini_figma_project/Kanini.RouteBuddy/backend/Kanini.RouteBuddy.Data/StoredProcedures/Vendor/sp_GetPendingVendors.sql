CREATE PROCEDURE sp_GetPendingVendors
    @Offset INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        VendorId, 
        AgencyName, 
        OwnerName, 
        Status
    FROM Vendors 
    WHERE Status = 0 -- PendingApproval
    ORDER BY VendorId 
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END