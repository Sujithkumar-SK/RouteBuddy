CREATE PROCEDURE [dbo].[sp_UpdateCustomerProfile]
    @CustomerId INT,
    @FirstName NVARCHAR(50),
    @MiddleName NVARCHAR(50) = NULL,
    @LastName NVARCHAR(50),
    @DateOfBirth DATE,
    @Gender INT,
    @Phone NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Update Customer table
        UPDATE Customers 
        SET 
            FirstName = @FirstName,
            MiddleName = @MiddleName,
            LastName = @LastName,
            DateOfBirth = @DateOfBirth,
            Gender = @Gender,
            UpdatedOn = GETUTCDATE()
        WHERE CustomerId = @CustomerId AND IsActive = 1;
        
        -- Update User phone
        UPDATE Users 
        SET 
            Phone = @Phone,
            UpdatedOn = GETUTCDATE()
        WHERE UserId = (SELECT UserId FROM Customers WHERE CustomerId = @CustomerId)
            AND IsActive = 1;
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success;
    END CATCH
END