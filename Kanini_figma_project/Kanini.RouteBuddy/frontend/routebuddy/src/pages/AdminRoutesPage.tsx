import { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Card,
  CircularProgress,
  Tabs,
  Tab,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
} from '@mui/material';
import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, Tooltip } from 'recharts';
import Layout from '../components/layout/Layout';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchAllBookings } from '../features/admin/adminBookingSlice';
import RouteManagement from '../features/admin/RouteManagement';
import StopManagement from '../features/admin/StopManagement';
import api from '../services/api';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;
  return (
    <div role="tabpanel" hidden={value !== index} {...other}>
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

const AdminRoutesPage = () => {
  const dispatch = useAppDispatch();
  const { bookings, loading } = useAppSelector((state) => state.adminBooking);
  const [tabValue, setTabValue] = useState(0);
  const [routes, setRoutes] = useState<any[]>([]);

  useEffect(() => {
    dispatch(fetchAllBookings());
    fetchRoutes();
  }, [dispatch]);

  const fetchRoutes = async () => {
    try {
      const response = await api.get('/route?pageNumber=1&pageSize=100');
      setRoutes(response.data.data || response.data || []);
    } catch (error) {
      console.error('Failed to fetch routes:', error);
      setRoutes([]);
    }
  };

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  // Extract route data from bookings
  const uniqueBookings = bookings.filter((booking, index, arr) => 
    arr.findIndex(b => b.bookingId === booking.bookingId) === index
  );
  
  const routeData = (() => {
    const routeStats: { [key: string]: { totalBookings: number, confirmedBookings: number, cancelledBookings: number, totalRevenue: number } } = {};
    
    uniqueBookings.forEach(booking => {
      if (!routeStats[booking.route]) {
        routeStats[booking.route] = { totalBookings: 0, confirmedBookings: 0, cancelledBookings: 0, totalRevenue: 0 };
      }
      routeStats[booking.route].totalBookings += 1;
      if (booking.status === 2) {
        routeStats[booking.route].confirmedBookings += 1;
        routeStats[booking.route].totalRevenue += booking.totalAmount;
      } else if (booking.status === 3) {
        routeStats[booking.route].cancelledBookings += 1;
      }
    });
    
    return Object.entries(routeStats).map(([route, stats]) => {
      const [source, destination] = route.split(' to ');
      return {
        routeId: Math.random(),
        source: source || route,
        destination: destination || '',
        totalBookings: stats.totalBookings,
        confirmedBookings: stats.confirmedBookings,
        cancelledBookings: stats.cancelledBookings,
        totalRevenue: stats.totalRevenue,
        averageBookingValue: stats.confirmedBookings > 0 ? Math.round(stats.totalRevenue / stats.confirmedBookings) : 0,
        successRate: stats.totalBookings > 0 ? Math.round((stats.confirmedBookings / stats.totalBookings) * 100) : 0,
        isActive: true
      };
    });
  })();

  return (
    <Layout>
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Typography variant="h4" sx={{ mb: 3, fontWeight: 600 }}>
          🛣️ Route Management
        </Typography>

        <Card>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label="Analytics" />
              <Tab label="Route Management" />
              <Tab label="Stop Management" />
            </Tabs>
          </Box>

          <TabPanel value={tabValue} index={0}>
            <Typography variant="h6" sx={{ mb: 3 }}>
              Route Analytics Overview
            </Typography>
            
            {/* Route Performance Overview */}
            <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: 2, mb: 3 }}>
              <Card sx={{ p: 2, textAlign: 'center', bgcolor: '#e8f5e8', border: '1px solid #4caf50' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: '#2e7d32' }}>
                  {routes.filter(r => r.isActive).length}
                </Typography>
                <Typography variant="body2" sx={{ color: '#2e7d32', fontWeight: 500 }}>Active Routes</Typography>
              </Card>
              <Card sx={{ p: 2, textAlign: 'center', bgcolor: '#ffebee', border: '1px solid #f44336' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: '#d32f2f' }}>
                  {routes.filter(r => !r.isActive).length}
                </Typography>
                <Typography variant="body2" sx={{ color: '#d32f2f', fontWeight: 500 }}>Inactive Routes</Typography>
              </Card>
              <Card sx={{ p: 2, textAlign: 'center', bgcolor: '#e3f2fd', border: '1px solid #2196f3' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: '#1976d2' }}>
                  {routes.length}
                </Typography>
                <Typography variant="body2" sx={{ color: '#1976d2', fontWeight: 500 }}>Total Routes</Typography>
              </Card>
            </Box>

            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3, mb: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>📊 Route Performance</Typography>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={routeData
                  .sort((a, b) => b.totalBookings - a.totalBookings)
                  .slice(0, 8)
                  .map(route => ({
                    name: `${route.source}-${route.destination}`.length > 15 
                      ? `${route.source}-${route.destination}`.substring(0, 15) + '...' 
                      : `${route.source}-${route.destination}`,
                    bookings: route.totalBookings,
                    revenue: Math.round(route.totalRevenue / 1000),
                    successRate: route.successRate
                  }))}>
                  <XAxis dataKey="name" angle={-45} textAnchor="end" height={80} tick={{ fontSize: 11 }} />
                  <YAxis yAxisId="left" orientation="left" tick={{ fontSize: 11 }} />
                  <YAxis yAxisId="right" orientation="right" tick={{ fontSize: 11 }} />
                  <Tooltip 
                    formatter={(value, name) => [
                      name === 'Revenue (₹K)' ? `₹${Number(value) * 1000}` : value, 
                      name
                    ]}
                    labelStyle={{ color: '#333', fontWeight: 600 }}
                  />
                  <Bar yAxisId="left" dataKey="bookings" fill="#2196f3" name="Bookings" />
                  <Bar yAxisId="right" dataKey="revenue" fill="#4caf50" name="Revenue (₹K)" />
                </BarChart>
              </ResponsiveContainer>
            </Card>

            <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 3, mb: 3 }}>
              <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>💰 Revenue Distribution</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <BarChart data={routeData
                    .filter(r => r.totalRevenue > 0)
                    .sort((a, b) => b.totalRevenue - a.totalRevenue)
                    .slice(0, 6)
                    .map(route => ({
                      name: `${route.source} → ${route.destination}`.length > 12 
                        ? `${route.source} → ${route.destination}`.substring(0, 12) + '...' 
                        : `${route.source} → ${route.destination}`,
                      revenue: Math.round(route.totalRevenue / 1000)
                    }))}>
                    <XAxis dataKey="name" angle={-45} textAnchor="end" height={60} tick={{ fontSize: 10 }} />
                    <YAxis tick={{ fontSize: 11 }} />
                    <Tooltip formatter={(value) => [`₹${Number(value) * 1000}`, 'Revenue']} />
                    <Bar dataKey="revenue" fill="#4caf50" name="Revenue (₹K)" />
                  </BarChart>
                </ResponsiveContainer>
              </Card>
              
              <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>🔥 Route Demand Trends</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <BarChart data={routeData
                    .sort((a, b) => b.successRate - a.successRate)
                    .slice(0, 6)
                    .map(route => ({
                      name: `${route.source}-${route.destination}`.length > 12 
                        ? `${route.source}-${route.destination}`.substring(0, 12) + '...' 
                        : `${route.source}-${route.destination}`,
                      demand: route.totalBookings,
                      success: route.successRate
                    }))}>
                    <XAxis dataKey="name" angle={-45} textAnchor="end" height={60} tick={{ fontSize: 10 }} />
                    <YAxis tick={{ fontSize: 11 }} />
                    <Tooltip />
                    <Bar dataKey="demand" fill="#ff9800" name="Total Bookings" />
                  </BarChart>
                </ResponsiveContainer>
              </Card>
            </Box>

            <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 3, mb: 3 }}>
              <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>⏱️ Route Duration Comparison</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <BarChart data={[
                    { name: 'Chennai → Bangalore', duration: 6 },
                    { name: 'Mumbai → Pune', duration: 3 },
                    { name: 'Chennai → Salem', duration: 4 },
                    { name: 'Salem → Bangalore', duration: 3 }
                  ]}>
                    <XAxis dataKey="name" angle={-45} textAnchor="end" height={60} tick={{ fontSize: 10 }} />
                    <YAxis tick={{ fontSize: 11 }} label={{ value: 'Hours', angle: -90, position: 'insideLeft' }} />
                    <Tooltip formatter={(value) => [`${value} hrs`, 'Duration']} />
                    <Bar dataKey="duration" fill="#2196f3" name="Duration (hrs)" />
                  </BarChart>
                </ResponsiveContainer>
              </Card>
              
              <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>💰 Base Price Comparison</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <BarChart data={[
                    { name: 'Chennai → Bangalore', price: 500 },
                    { name: 'Mumbai → Pune', price: 300 },
                    { name: 'Chennai → Salem', price: 300 },
                    { name: 'Salem → Bangalore', price: 250 }
                  ]}>
                    <XAxis dataKey="name" angle={-45} textAnchor="end" height={60} tick={{ fontSize: 10 }} />
                    <YAxis tick={{ fontSize: 11 }} label={{ value: '₹', angle: -90, position: 'insideLeft' }} />
                    <Tooltip formatter={(value) => [`₹${value}`, 'Base Price']} />
                    <Bar dataKey="price" fill="#4caf50" name="Base Price (₹)" />
                  </BarChart>
                </ResponsiveContainer>
              </Card>
            </Box>


          </TabPanel>

          <TabPanel value={tabValue} index={1}>
            <RouteManagement />
          </TabPanel>

          <TabPanel value={tabValue} index={2}>
            <StopManagement />
          </TabPanel>
        </Card>
      </Container>
    </Layout>
  );
};

export default AdminRoutesPage;