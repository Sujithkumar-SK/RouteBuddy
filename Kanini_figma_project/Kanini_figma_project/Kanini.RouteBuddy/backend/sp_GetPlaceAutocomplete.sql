CREATE PROCEDURE sp_GetPlaceAutocomplete
    @Query NVARCHAR(50),
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all unique cities from Routes table when query is empty, otherwise filter
    IF @Query = ''
    BEGIN
        SELECT 
            ROW_NUMBER() OVER (ORDER BY CityName) as StopId,
            CityName as Name,
            NULL as Landmark
        FROM (
            SELECT DISTINCT r.Source as CityName FROM Routes r WHERE r.IsActive = 1
            UNION
            SELECT DISTINCT r.Destination as CityName FROM Routes r WHERE r.IsActive = 1
        ) Cities
        ORDER BY CityName ASC;
    END
    ELSE
    BEGIN
        SELECT TOP (@Limit)
            ROW_NUMBER() OVER (ORDER BY Priority, CityName) as StopId,
            CityName as Name,
            NULL as Landmark
        FROM (
            SELECT DISTINCT 
                r.Source as CityName,
                CASE WHEN r.Source LIKE @Query + '%' THEN 1 ELSE 2 END as Priority
            FROM Routes r 
            WHERE r.IsActive = 1 AND r.Source LIKE '%' + @Query + '%'
            UNION
            SELECT DISTINCT 
                r.Destination as CityName,
                CASE WHEN r.Destination LIKE @Query + '%' THEN 1 ELSE 2 END as Priority
            FROM Routes r 
            WHERE r.IsActive = 1 AND r.Destination LIKE '%' + @Query + '%'
        ) Cities
        ORDER BY Priority, CityName ASC;
    END
END