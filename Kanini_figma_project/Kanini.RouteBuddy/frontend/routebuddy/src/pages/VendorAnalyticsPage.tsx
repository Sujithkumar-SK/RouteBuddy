import { useEffect } from 'react';
import { Container, Typography, Grid, Box, CircularProgress, Alert } from '@mui/material';
import Layout from '../components/layout/Layout';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchVendorAnalytics } from '../features/vendor/vendorSlice';
import RevenueAnalyticsCard from '../features/vendor/components/RevenueAnalyticsCard';
import FleetStatusCard from '../features/vendor/components/FleetStatusCard';
import QuickStatsCard from '../features/vendor/components/QuickStatsCard';
import RecentBookingsCard from '../features/vendor/components/RecentBookingsCard';
import PerformanceMetricsCard from '../features/vendor/components/PerformanceMetricsCard';
import NotificationsCard from '../features/vendor/components/NotificationsCard';
import AlertsCard from '../features/vendor/components/AlertsCard';

const VendorAnalyticsPage = () => {
  const dispatch = useAppDispatch();
  const {
    revenueAnalytics,
    performanceMetrics,
    fleetStatus,
    quickStats,
    recentBookings,
    notifications,
    alerts,
    loading,
    error,
  } = useAppSelector((state) => state.vendor);

  useEffect(() => {
    dispatch(fetchVendorAnalytics());
  }, [dispatch]);

  if (loading.analytics) {
    return (
      <Layout>
        <Container maxWidth="xl" sx={{ py: 4 }}>
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
        <Container maxWidth="xl" sx={{ py: 4 }}>
          <Alert severity="error">{error}</Alert>
        </Container>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="analytics-container">
        <Container maxWidth="xl" sx={{ py: 4 }}>
          <div className="analytics-header">
            <h1 className="analytics-title">Analytics & Reports</h1>
          </div>

        <Grid container spacing={3}>
          {/* Revenue Analytics */}
          {revenueAnalytics && (
            <Grid item xs={12} lg={8}>
              <RevenueAnalyticsCard
                totalRevenue={revenueAnalytics.totalRevenue}
                monthlyRevenue={revenueAnalytics.monthlyRevenue}
                weeklyRevenue={revenueAnalytics.weeklyRevenue}
              />
            </Grid>
          )}

          {/* Quick Stats */}
          {quickStats && (
            <Grid item xs={12} lg={4}>
              <QuickStatsCard
                totalBookings={quickStats.totalBookings}
                activeRoutes={quickStats.activeRoutes}
                totalRevenue={quickStats.totalRevenue}
              />
            </Grid>
          )}

          {/* Performance Metrics */}
          {performanceMetrics && (
            <Grid item xs={12} md={6}>
              <PerformanceMetricsCard
                monthlyBookings={performanceMetrics.monthlyBookings}
                onTimePerformance={performanceMetrics.onTimePerformance}
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
          <Grid item xs={12} lg={8}>
            <RecentBookingsCard bookings={recentBookings} />
          </Grid>

          {/* Notifications & Alerts */}
          <Grid item xs={12} lg={4}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <NotificationsCard notifications={notifications} />
              </Grid>
              <Grid item xs={12}>
                <AlertsCard alerts={alerts} />
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </Container>
      </div>
    </Layout>
  );
};

export default VendorAnalyticsPage;