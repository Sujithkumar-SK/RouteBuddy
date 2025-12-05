CREATE PROCEDURE [dbo].[sp_CheckUserExistsByPhone]
    @Phone NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1)
    FROM Users 
    WHERE Phone = @Phone AND IsActive = 1;
END