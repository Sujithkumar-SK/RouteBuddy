import { Card, CardContent, Typography, Box, Grid } from '@mui/material';
import { Assessment, BookOnline, Route, AttachMoney } from '@mui/icons-material';
import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, Tooltip } from 'recharts';

interface QuickStatsCardProps {
  totalBookings: number;
  activeRoutes: number;
  totalRevenue: number;
}

const QuickStatsCard = ({ totalBookings, activeRoutes, totalRevenue }: QuickStatsCardProps) => {
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 0,
    }).format(amount);
  };

  return (
    <Card>
      <CardContent>
        <Box display="flex" alignItems="center" mb={2}>
          <Assessment sx={{ mr: 1, color: 'primary.main' }} />
          <Typography variant="h6">Quick Stats</Typography>
        </Box>

        <Grid container spacing={2}>
          <Grid item xs={4}>
            <Box textAlign="center" p={2} bgcolor="primary.light" borderRadius={1}>
              <BookOnline sx={{ fontSize: 32, color: 'primary.main', mb: 1 }} />
              <Typography variant="h5" color="primary.main">
                {totalBookings}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Total Bookings
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={4}>
            <Box textAlign="center" p={2} bgcolor="secondary.light" borderRadius={1}>
              <Route sx={{ fontSize: 32, color: 'secondary.main', mb: 1 }} />
              <Typography variant="h5" color="secondary.main">
                {activeRoutes}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Active Routes
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={4}>
            <Box textAlign="center" p={2} bgcolor="success.light" borderRadius={1}>
              <AttachMoney sx={{ fontSize: 32, color: 'success.main', mb: 1 }} />
              <Typography variant="h6" color="success.main">
                {formatCurrency(totalRevenue)}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Total Revenue
              </Typography>
            </Box>
          </Grid>
        </Grid>

        <Box mt={3}>
          <Typography variant="h6" gutterBottom>
            Quick Stats Overview
          </Typography>
          <Box height={180}>
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={[
                { stat: 'Bookings', value: totalBookings },
                { stat: 'Routes', value: activeRoutes },
                { stat: 'Revenue (K)', value: totalRevenue / 1000 }
              ]}>
                <defs>
                  <linearGradient id="quickStatsGradient" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#1976d2" stopOpacity={0.8}/>
                    <stop offset="95%" stopColor="#64b5f6" stopOpacity={0.3}/>
                  </linearGradient>
                </defs>
                <XAxis dataKey="stat" />
                <YAxis />
                <Tooltip formatter={(value, name) => [
                  name === 'Revenue (K)' ? formatCurrency(Number(value) * 1000) : value,
                  name === 'Revenue (K)' ? 'Revenue' : name
                ]} />
                <Bar dataKey="value" fill="url(#quickStatsGradient)" />
              </BarChart>
            </ResponsiveContainer>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
};

export default QuickStatsCard;