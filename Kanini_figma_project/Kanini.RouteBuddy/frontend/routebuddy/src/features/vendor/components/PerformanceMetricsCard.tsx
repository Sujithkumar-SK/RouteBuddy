import { Card, CardContent, Typography, Box, Grid, LinearProgress } from '@mui/material';
import { Speed, BookOnline } from '@mui/icons-material';
import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, Tooltip } from 'recharts';

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

        <Box mt={3}>
          <Typography variant="h6" gutterBottom>
            Performance Overview
          </Typography>
          <Box height={200}>
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={[
                { metric: 'Bookings', value: monthlyBookings },
                { metric: 'On-Time %', value: onTimePerformance }
              ]}>
                <defs>
                  <linearGradient id="performanceGradient" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#1976d2" stopOpacity={0.8}/>
                    <stop offset="95%" stopColor="#42a5f5" stopOpacity={0.3}/>
                  </linearGradient>
                </defs>
                <XAxis dataKey="metric" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="value" fill="url(#performanceGradient)" />
              </BarChart>
            </ResponsiveContainer>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
};

export default PerformanceMetricsCard;