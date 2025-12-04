CREATE PROCEDURE [dbo].[sp_UpdateCustomerProfilePicture]
    @CustomerId INT,
    @ProfilePicture VARBINARY(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE Customers 
        SET 
            ProfilePicture = @ProfilePicture,
            UpdatedOn = GETUTCDATE()
        WHERE CustomerId = @CustomerId AND IsActive = 1;
        
        IF @@ROWCOUNT = 0
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 0 AS Success;
            RETURN;
        END
        
        COMMIT TRANSACTION;
        SELECT 1 AS Success;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success;
    END CATCH
END