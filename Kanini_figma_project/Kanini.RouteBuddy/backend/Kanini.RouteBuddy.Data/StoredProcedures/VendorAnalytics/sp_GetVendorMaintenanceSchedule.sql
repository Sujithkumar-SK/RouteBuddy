ALTER PROCEDURE [dbo].[sp_GetVendorMaintenanceSchedule]
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Only return buses that are actually under maintenance (Status = 3)
    SELECT 
        b.RegistrationNo as BusNumber,
        'Under Maintenance' as Type,
        b.UpdatedOn as DueDate,
        'High' as Priority
    FROM Buses b
    WHERE b.VendorId = @VendorId 
    AND b.Status = 3
    ORDER BY b.UpdatedOn DESC;
END