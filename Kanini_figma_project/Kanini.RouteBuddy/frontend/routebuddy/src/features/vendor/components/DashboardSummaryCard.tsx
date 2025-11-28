import { Card, CardContent, Typography, Box, Chip } from '@mui/material';
import { DirectionsBus, Route, Schedule, CheckCircle } from '@mui/icons-material';

interface DashboardSummaryCardProps {
  totalBuses: number;
  activeBuses: number;
  pendingBuses: number;
  totalRoutes: number;
  totalSchedules: number;
  upcomingSchedules: number;
  vendorStatus: string;
}

const DashboardSummaryCard = ({
  totalBuses,
  activeBuses,
  pendingBuses,
  totalRoutes,
  totalSchedules,
  upcomingSchedules,
  vendorStatus,
}: DashboardSummaryCardProps) => {
  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Active': return 'success';
      case 'PendingApproval': return 'warning';
      case 'Suspended': return 'error';
      default: return 'default';
    }
  };

  return (
    <Card>
      <CardContent>
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
          <Typography variant="h6">Dashboard Overview</Typography>
          <Chip 
            label={vendorStatus} 
            color={getStatusColor(vendorStatus)} 
            size="small" 
          />
        </Box>

        <Box display="grid" gridTemplateColumns="repeat(auto-fit, minmax(200px, 1fr))" gap={2}>
          <Box textAlign="center" p={2} bgcolor="primary.light" borderRadius={1}>
            <DirectionsBus sx={{ fontSize: 40, color: 'primary.main', mb: 1 }} />
            <Typography variant="h4" color="primary.main">{totalBuses}</Typography>
            <Typography variant="body2" color="text.secondary">Total Buses</Typography>
            <Typography variant="caption" color="success.main">
              {activeBuses} Active • {pendingBuses} Pending
            </Typography>
          </Box>

          <Box textAlign="center" p={2} bgcolor="secondary.light" borderRadius={1}>
            <Route sx={{ fontSize: 40, color: 'secondary.main', mb: 1 }} />
            <Typography variant="h4" color="secondary.main">{totalRoutes}</Typography>
            <Typography variant="body2" color="text.secondary">Active Routes</Typography>
          </Box>

          <Box textAlign="center" p={2} bgcolor="success.light" borderRadius={1}>
            <Schedule sx={{ fontSize: 40, color: 'success.main', mb: 1 }} />
            <Typography variant="h4" color="success.main">{totalSchedules}</Typography>
            <Typography variant="body2" color="text.secondary">Total Schedules</Typography>
            <Typography variant="caption" color="info.main">
              {upcomingSchedules} Upcoming
            </Typography>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
};

export default DashboardSummaryCard;