CREATE PROCEDURE [dbo].[sp_CheckUserExistsByEmail]
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1)
    FROM Users 
    WHERE Email = @Email AND IsActive = 1;
END