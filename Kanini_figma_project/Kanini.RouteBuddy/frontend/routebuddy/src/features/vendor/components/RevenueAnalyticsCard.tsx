import { Card, CardContent, Typography, Box, Grid } from '@mui/material';
import { TrendingUp, AttachMoney, CalendarToday, DateRange } from '@mui/icons-material';
import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, Tooltip } from 'recharts';

interface RevenueAnalyticsCardProps {
  totalRevenue: number;
  monthlyRevenue: number;
  weeklyRevenue: number;
}

const RevenueAnalyticsCard = ({ totalRevenue, monthlyRevenue, weeklyRevenue }: RevenueAnalyticsCardProps) => {
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
          <TrendingUp sx={{ mr: 1, color: 'success.main' }} />
          <Typography variant="h6">Revenue Analytics</Typography>
        </Box>

        <Grid container spacing={2}>
          <Grid item xs={12} md={4}>
            <Box textAlign="center" p={2} bgcolor="success.light" borderRadius={1}>
              <AttachMoney sx={{ fontSize: 32, color: 'success.main', mb: 1 }} />
              <Typography variant="h5" color="success.main">
                {formatCurrency(totalRevenue)}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Total Revenue
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={12} md={4}>
            <Box textAlign="center" p={2} bgcolor="info.light" borderRadius={1}>
              <CalendarToday sx={{ fontSize: 32, color: 'info.main', mb: 1 }} />
              <Typography variant="h5" color="info.main">
                {formatCurrency(monthlyRevenue)}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                This Month
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={12} md={4}>
            <Box textAlign="center" p={2} bgcolor="warning.light" borderRadius={1}>
              <DateRange sx={{ fontSize: 32, color: 'warning.main', mb: 1 }} />
              <Typography variant="h5" color="warning.main">
                {formatCurrency(weeklyRevenue)}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Last 7 Days
              </Typography>
            </Box>
          </Grid>
        </Grid>

        <Box mt={3}>
          <Typography variant="h6" gutterBottom>
            Revenue Breakdown
          </Typography>
          <Box height={250}>
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={[
                { period: 'Weekly', amount: weeklyRevenue },
                { period: 'Monthly', amount: monthlyRevenue },
                { period: 'Total', amount: totalRevenue }
              ]}>
                <defs>
                  <linearGradient id="revenueGradient" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#1976d2" stopOpacity={0.8}/>
                    <stop offset="95%" stopColor="#1976d2" stopOpacity={0.3}/>
                  </linearGradient>
                </defs>
                <XAxis dataKey="period" />
                <YAxis tickFormatter={(value) => `₹${(value / 1000).toFixed(0)}K`} />
                <Tooltip formatter={(value) => [formatCurrency(Number(value)), 'Revenue']} />
                <Bar dataKey="amount" fill="url(#revenueGradient)" />
              </BarChart>
            </ResponsiveContainer>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
};

export default RevenueAnalyticsCard;