CREATE PROCEDURE sp_ValidatePlaceExists
    @PlaceName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if place exists in Routes table (either as source or destination)
    SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END as PlaceExists
    FROM Routes r
    WHERE r.IsActive = 1
        AND (LOWER(TRIM(r.Source)) = LOWER(TRIM(@PlaceName))
             OR LOWER(TRIM(r.Destination)) = LOWER(TRIM(@PlaceName)));
END