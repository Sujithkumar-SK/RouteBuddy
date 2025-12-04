CREATE PROCEDURE [dbo].[sp_GetBusesByVendor]
    @VendorId INT,
    @PageNumber INT,
    @PageSize INT,
    @Status INT = NULL,
    @BusType INT = NULL,
    @Search NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input
        IF @VendorId <= 0 OR @PageNumber <= 0 OR @PageSize <= 0
        BEGIN
            SELECT 'ERROR' AS Result, 'INVALID_PARAMETERS' AS ErrorMessage;
            RETURN;
        END
        
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
        DECLARE @WhereClause NVARCHAR(500) = 'WHERE b.VendorId = @VendorId';
        
        -- Build dynamic where clause
        IF @Status IS NOT NULL
            SET @WhereClause = @WhereClause + ' AND b.Status = @Status';
        IF @BusType IS NOT NULL
            SET @WhereClause = @WhereClause + ' AND b.BusType = @BusType';
        IF @Search IS NOT NULL
            SET @WhereClause = @WhereClause + ' AND (b.BusName LIKE ''%' + @Search + '%'' OR b.RegistrationNo LIKE ''%' + @Search + '%'')';
        
        -- Get buses
        DECLARE @SQL NVARCHAR(MAX) = '
        SELECT b.BusId, b.VendorId, b.BusName, b.RegistrationNo, b.BusType, b.TotalSeats, 
               b.Amenities, b.Status, b.IsActive, b.DriverName, b.DriverContact, 
               b.SeatLayoutTemplateId, b.CreatedBy, b.UpdatedBy, v.AgencyName, b.CreatedOn
        FROM Buses b
        INNER JOIN Vendors v ON b.VendorId = v.VendorId ' +
        @WhereClause + '
        ORDER BY b.CreatedOn DESC 
        OFFSET ' + CAST(@Offset AS NVARCHAR(10)) + ' ROWS FETCH NEXT ' + CAST(@PageSize AS NVARCHAR(10)) + ' ROWS ONLY';
        
        EXEC sp_executesql @SQL, N'@VendorId INT, @Status INT, @BusType INT', @VendorId, @Status, @BusType;
        
        SELECT 'SUCCESS' AS Result;
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' AS Result, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END