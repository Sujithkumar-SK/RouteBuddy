CREATE PROCEDURE [dbo].[sp_GetCustomerProfileByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerId,
        c.FirstName,
        c.MiddleName,
        c.LastName,
        c.DateOfBirth,
        c.Gender,
        c.ProfilePicture,
        c.IsActive,
        c.CreatedOn,
        c.UpdatedOn,
        u.Email,
        u.Phone,
        u.IsActive AS UserIsActive
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    WHERE u.UserId = @UserId 
        AND c.IsActive = 1
        AND u.IsActive = 1
    ORDER BY c.UpdatedOn DESC;
END