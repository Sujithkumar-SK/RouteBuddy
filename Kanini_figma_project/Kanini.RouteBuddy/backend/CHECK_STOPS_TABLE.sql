-- Check if Stops table exists and has data
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Stops';

-- If table exists, check its structure
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Stops';

-- Check if there's any data
SELECT COUNT(*) as TotalStops FROM Stops;

-- Check existing stored procedures
SELECT name FROM sys.procedures WHERE name LIKE '%Stop%' OR name LIKE '%Place%';

-- Sample data check
SELECT TOP 5 * FROM Stops;