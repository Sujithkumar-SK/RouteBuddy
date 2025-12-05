-- =============================================
-- EXECUTE THESE STORED PROCEDURES IN SQL SERVER
-- =============================================

-- 1. Get All Stops (Required for Stop Management)
CREATE PROCEDURE sp_GetAllStops
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
        
        SELECT 
            StopId,
            Name,
            Landmark,
            IsActive,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Stops 
        WHERE IsActive = 1
        ORDER BY CreatedOn DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- 2. Get Stop By ID (Required for Stop Details)
CREATE PROCEDURE sp_GetStopById
    @StopId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            StopId,
            Name,
            Landmark,
            IsActive,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Stops 
        WHERE StopId = @StopId AND IsActive = 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- 3. Get All Active Stops (Required for Vendor Route Creation)
CREATE PROCEDURE sp_GetAllActiveStops
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            StopId,
            Name,
            Landmark,
            IsActive,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Stops 
        WHERE IsActive = 1
        ORDER BY Name ASC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- 4. Place Autocomplete (Already exists - check if created)
-- This should already exist from previous implementations
-- If not, create it:
CREATE PROCEDURE sp_GetPlaceAutocomplete
    @Query NVARCHAR(100) = '',
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF @Query = ''
        BEGIN
            SELECT TOP (@Limit)
                StopId,
                Name,
                Landmark
            FROM Stops 
            WHERE IsActive = 1
            ORDER BY Name ASC;
        END
        ELSE
        BEGIN
            SELECT TOP (@Limit)
                StopId,
                Name,
                Landmark
            FROM Stops 
            WHERE IsActive = 1 
                AND (Name LIKE '%' + @Query + '%' 
                     OR Landmark LIKE '%' + @Query + '%')
            ORDER BY 
                CASE WHEN Name LIKE @Query + '%' THEN 1 ELSE 2 END,
                Name ASC;
        END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- 5. Validate Place Exists (Already exists - check if created)
CREATE PROCEDURE sp_ValidatePlaceExists
    @PlaceName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT COUNT(*)
        FROM Stops 
        WHERE IsActive = 1 
            AND (Name = @PlaceName OR Name LIKE '%' + @PlaceName + '%');
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO