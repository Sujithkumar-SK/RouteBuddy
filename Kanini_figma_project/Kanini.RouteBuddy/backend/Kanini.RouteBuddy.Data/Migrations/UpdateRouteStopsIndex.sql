-- Migration: Update RouteStops unique index to include ScheduleId
-- This allows same (RouteId, OrderNumber) for different schedules

-- Drop the old unique index
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RouteStops_RouteId_OrderNumber')
BEGIN
    DROP INDEX IX_RouteStops_RouteId_OrderNumber ON RouteStops;
    PRINT 'Dropped old index IX_RouteStops_RouteId_OrderNumber';
END

-- Create new unique index including ScheduleId
CREATE UNIQUE INDEX IX_RouteStops_RouteId_OrderNumber_ScheduleId 
ON RouteStops(RouteId, OrderNumber, ScheduleId);

PRINT 'Created new index IX_RouteStops_RouteId_OrderNumber_ScheduleId';

-- This allows:
-- RouteId=1, OrderNumber=1, ScheduleId=NULL (route template)
-- RouteId=1, OrderNumber=1, ScheduleId=101 (schedule-specific)
-- RouteId=1, OrderNumber=1, ScheduleId=102 (different schedule-specific)