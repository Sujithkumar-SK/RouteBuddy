import { useEffect } from 'react';
import { Container, Typography, Grid, Box, CircularProgress, Alert } from '@mui/material';
import Layout from '../components/layout/Layout';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchVendorDashboard } from '../features/vendor/vendorSlice';
import DashboardSummaryCard from '../features/vendor/components/DashboardSummaryCard';
import RevenueAnalyticsCard from '../features/vendor/components/RevenueAnalyticsCard';
import FleetStatusCard from '../features/vendor/components/FleetStatusCard';
import QuickStatsCard from '../features/vendor/components/QuickStatsCard';
import RecentBookingsCard from '../features/vendor/components/RecentBookingsCard';

const VendorDashboard = () => {
  const dispatch = useAppDispatch();
  const {
    dashboard,
    revenueAnalytics,
    fleetStatus,
    quickStats,
    recentBookings,
    loading,
    error,
  } = useAppSelector((state) => state.vendor);

  useEffect(() => {
    dispatch(fetchVendorDashboard());
  }, [dispatch]);

  if (loading.dashboard) {
    return (
      <Layout>
        <Container maxWidth="lg" sx={{ py: 4 }}>
          <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
            <CircularProgress />
          </Box>
        </Container>
      </Layout>
    );
  }

  if (error) {
    return (
      <Layout>
        <Container maxWidth="lg" sx={{ py: 4 }}>
          <Alert severity="error">{error}</Alert>
        </Container>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="vendor-dashboard-container">
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <div className="vendor-dashboard-header">
          <h1 className="vendor-dashboard-title">Vendor Dashboard</h1>
        </div>

        <Grid container spacing={3}>
          {/* Dashboard Summary */}
          {dashboard && (
            <Grid item xs={12}>
              <DashboardSummaryCard
                totalBuses={dashboard.totalBuses}
                activeBuses={dashboard.activeBuses}
                pendingBuses={dashboard.pendingBuses}
                totalRoutes={dashboard.totalRoutes}
                totalSchedules={dashboard.totalSchedules}
                upcomingSchedules={dashboard.upcomingSchedules}
                vendorStatus={dashboard.vendorStatus}
              />
            </Grid>
          )}

          {/* Revenue Analytics */}
          {revenueAnalytics && (
            <Grid item xs={12} md={8}>
              <RevenueAnalyticsCard
                totalRevenue={revenueAnalytics.totalRevenue}
                monthlyRevenue={revenueAnalytics.monthlyRevenue}
                weeklyRevenue={revenueAnalytics.weeklyRevenue}
              />
            </Grid>
          )}

          {/* Quick Stats */}
          {quickStats && (
            <Grid item xs={12} md={4}>
              <QuickStatsCard
                totalBookings={quickStats.totalBookings}
                activeRoutes={quickStats.activeRoutes}
                totalRevenue={quickStats.totalRevenue}
              />
            </Grid>
          )}

          {/* Fleet Status */}
          {fleetStatus && (
            <Grid item xs={12} md={6}>
              <FleetStatusCard
                totalBuses={fleetStatus.totalBuses}
                activeBuses={fleetStatus.activeBuses}
                maintenanceBuses={fleetStatus.maintenanceBuses}
                idleBuses={fleetStatus.idleBuses}
              />
            </Grid>
          )}

          {/* Recent Bookings */}
          <Grid item xs={12} md={6}>
            <RecentBookingsCard bookings={recentBookings} />
          </Grid>
        </Grid>
      </Container>
      </div>
    </Layout>
  );
};

export default VendorDashboard;
