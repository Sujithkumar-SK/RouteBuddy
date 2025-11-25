CREATE PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId, 
        Email, 
        PasswordHash, 
        Phone, 
        Role, 
        IsEmailVerified, 
        IsActive, 
        LastLogin, 
        FailedLoginAttempts, 
        LockoutEnd, 
        CreatedBy, 
        CreatedOn, 
        UpdatedBy, 
        UpdatedOn
    FROM Users
    WHERE Email = @Email;
END
