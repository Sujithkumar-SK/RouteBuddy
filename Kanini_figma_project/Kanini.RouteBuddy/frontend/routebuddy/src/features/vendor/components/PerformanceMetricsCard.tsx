import { Card, CardContent, Typography, Box, Grid, LinearProgress } from '@mui/material';
import { Speed, BookOnline } from '@mui/icons-material';

interface PerformanceMetricsCardProps {
  monthlyBookings: number;
  onTimePerformance: number;
}

const PerformanceMetricsCard = ({ monthlyBookings, onTimePerformance }: PerformanceMetricsCardProps) => {
  const getPerformanceColor = (performance: number) => {
    if (performance >= 90) return 'success';
    if (performance >= 75) return 'warning';
    return 'error';
  };

  return (
    <Card>
      <CardContent>
        <Box display="flex" alignItems="center" mb={2}>
          <Speed sx={{ mr: 1, color: 'primary.main' }} />
          <Typography variant="h6">Performance Metrics</Typography>
        </Box>

        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <Box textAlign="center" p={2} bgcolor="primary.light" borderRadius={1}>
              <BookOnline sx={{ fontSize: 32, color: 'primary.main', mb: 1 }} />
              <Typography variant="h4" color="primary.main">
                {monthlyBookings}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Monthly Bookings
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={12} md={6}>
            <Box p={2}>
              <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                <Typography variant="body2" color="text.secondary">
                  On-Time Performance
                </Typography>
                <Typography variant="h6" color={`${getPerformanceColor(onTimePerformance)}.main`}>
                  {onTimePerformance.toFixed(1)}%
                </Typography>
              </Box>
              <LinearProgress
                variant="determinate"
                value={onTimePerformance}
                color={getPerformanceColor(onTimePerformance)}
                sx={{ height: 8, borderRadius: 4 }}
              />
              <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block' }}>
                {onTimePerformance >= 90 ? 'Excellent' : onTimePerformance >= 75 ? 'Good' : 'Needs Improvement'}
              </Typography>
            </Box>
          </Grid>
        </Grid>
      </CardContent>
    </Card>
  );
};

export default PerformanceMetricsCard;