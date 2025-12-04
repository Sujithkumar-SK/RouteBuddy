import { Container, Typography, Box, Card, CardContent, Grid } from '@mui/material';
import { useEffect } from 'react';
import Layout from '../components/layout/Layout';
import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchAllVendors, fetchPendingVendors } from '../features/admin/adminSlice';
import { fetchAllBuses } from '../features/admin/adminBusSlice';
import { fetchAllBookings } from '../features/admin/adminBookingSlice';

const AdminDashboard = () => {
  const dispatch = useAppDispatch();
  const { vendors, pendingVendors } = useAppSelector((state) => state.admin);
  const { buses } = useAppSelector((state) => state.adminBus);
  const { bookings, statusSummary } = useAppSelector((state) => state.adminBooking);

  useEffect(() => {
    dispatch(fetchAllVendors({ pageNumber: 1, pageSize: 1000 }));
    dispatch(fetchPendingVendors({ pageNumber: 1, pageSize: 1000 }));
    dispatch(fetchAllBuses());
    dispatch(fetchAllBookings());
  }, [dispatch]);

  return (
    <Layout>
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Typography variant="h4" sx={{ mb: 3, fontWeight: 600 }}>
          🛡️ Admin Dashboard
        </Typography>

        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Card>
              <CardContent>
                <Typography variant="h6" color="primary.main">👥 Vendors</Typography>
                <Typography variant="h4">{vendors.length.toLocaleString()}</Typography>
                <Typography variant="body2" color="success.main">▲ +15% MoM</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Card>
              <CardContent>
                <Typography variant="h6" color="primary.main">🚌 Buses</Typography>
                <Typography variant="h4">{buses.length.toLocaleString()}</Typography>
                <Typography variant="body2" color="success.main">▲ +8% MoM</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Card>
              <CardContent>
                <Typography variant="h6" color="primary.main">📋 Bookings</Typography>
                <Typography variant="h4">{bookings.length.toLocaleString()}</Typography>
                <Typography variant="body2" color="success.main">▲ +12% MoM</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Card>
              <CardContent>
                <Typography variant="h6" color="primary.main">💰 Revenue</Typography>
                <Typography variant="h4">₹{statusSummary?.totalRevenue ? (statusSummary.totalRevenue / 100000).toFixed(1) + 'L' : '0'}</Typography>
                <Typography variant="body2" color="success.main"> +18% MoM</Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>

        <Grid container spacing={3} sx={{ mt: 2 }}>
          <Grid size={{ xs: 12, md: 6 }}>
            <Card>
              <CardContent>
                <Typography variant="h6" sx={{ mb: 2 }}>📊 Monthly Booking Trends</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <BarChart data={(() => {
                    const monthlyData: { [key: string]: number } = {};
                    bookings.forEach(booking => {
                      const month = new Date(booking.bookingDate).toLocaleDateString('en-US', { month: 'short', year: '2-digit' });
                      monthlyData[month] = (monthlyData[month] || 0) + 1;
                    });
                    return Object.entries(monthlyData).map(([month, count]) => ({ name: month, value: count })).slice(-6);
                  })()}>
                    <XAxis dataKey="name" />
                    <YAxis allowDecimals={false} />
                    <Bar dataKey="value" fill="#d84e55" />
                  </BarChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ xs: 12, md: 6 }}>
            <Card>
              <CardContent>
                <Typography variant="h6" sx={{ mb: 2 }}>🔔 Action Items</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <PieChart>
                    <Pie
                      data={[
                        { name: 'Vendor Apps', value: pendingVendors.length, fill: '#ff9800' },
                        { name: 'Bus Registrations', value: buses.filter(b => b.status === 0).length, fill: '#2196f3' },
                        { name: 'Complaints', value: 12, fill: '#f44336' },
                        { name: 'Payment Disputes', value: 3, fill: '#9c27b0' }
                      ]}
                      cx="50%"
                      cy="50%"
                      outerRadius={80}
                      dataKey="value"
                      label={({ name, value }) => `${name}: ${value}`}
                    />
                  </PieChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </Container>
    </Layout>
  );
};

export default AdminDashboard;