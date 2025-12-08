-- Migration: Add ScheduleId to RouteStops table
-- Run this after adding the column via ALTER TABLE

-- Update existing RouteStops to be route-level templates (ScheduleId = NULL)
-- They are already NULL by default, so no update needed

-- Add index for better performance
CREATE INDEX IX_RouteStops_ScheduleId ON RouteStops(ScheduleId);
CREATE INDEX IX_RouteStops_RouteId_ScheduleId ON RouteStops(RouteId, ScheduleId);

PRINT 'Migration completed: Added ScheduleId to RouteStops with indexes';