CREATE PROCEDURE sp_GetUserByRefreshToken
    @RefreshToken NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.UserId, 
        u.Email, 
        u.PasswordHash, 
        u.Phone, 
        u.Role, 
        u.IsEmailVerified, 
        u.IsActive, 
        u.LastLogin, 
        u.FailedLoginAttempts, 
        u.LockoutEnd,
        u.CreatedBy, 
        u.CreatedOn, 
        u.UpdatedBy, 
        u.UpdatedOn
    FROM Users u
    INNER JOIN RefreshTokens rt ON u.UserId = rt.UserId
    WHERE rt.Token = @RefreshToken 
      AND rt.ExpiresAt > GETUTCDATE()
      AND rt.IsRevoked = 0
      AND u.IsActive = 1;
END
