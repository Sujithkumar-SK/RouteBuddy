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
import Layout from '../components/layout/Layout';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchAllBookings } from '../features/admin/adminBookingSlice';

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

const AdminReportsPage = () => {
  const dispatch = useAppDispatch();
  const { bookings, loading } = useAppSelector((state) => state.adminBooking);
  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    dispatch(fetchAllBookings());
  }, [dispatch]);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const getPaymentStatusChip = (status: number) => {
    const statusMap: { [key: number]: React.ReactElement } = {
      1: <Chip label="Pending" color="warning" size="small" />,
      2: <Chip label="Completed" color="success" size="small" />,
      3: <Chip label="Failed" color="error" size="small" />
    };
    return statusMap[status] || <Chip label="Unknown" color="default" size="small" />;
  };

  const getBookingStatusChip = (status: number) => {
    const statusMap: { [key: number]: React.ReactElement } = {
      1: <Chip label="Pending" color="warning" size="small" />,
      2: <Chip label="Confirmed" color="success" size="small" />,
      3: <Chip label="Cancelled" color="error" size="small" />
    };
    return statusMap[status] || <Chip label="Unknown" color="default" size="small" />;
  };

  const uniqueBookings = bookings.filter((booking, index, arr) => 
    arr.findIndex(b => b.bookingId === booking.bookingId) === index
  );

  return (
    <Layout>
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Typography variant="h4" sx={{ mb: 3, fontWeight: 600 }}>
          📊 Booking Reports
        </Typography>

        <Card>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label="📋 Latest Bookings" />
              <Tab label="👥 Top Customers" />
              <Tab label="❌ Cancellations" />
            </Tabs>
          </Box>

          <TabPanel value={tabValue} index={0}>
            <Typography variant="h6" sx={{ mb: 3 }}>
              Latest Bookings
            </Typography>
            {loading ? (
              <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
                <CircularProgress />
              </Box>
            ) : (
              <TableContainer component={Paper}>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>PNR</TableCell>
                      <TableCell>Customer</TableCell>
                      <TableCell>Seats</TableCell>
                      <TableCell>Amount</TableCell>
                      <TableCell>Payment Status</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Booked At</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {uniqueBookings
                      .sort((a, b) => new Date(b.bookedAt).getTime() - new Date(a.bookedAt).getTime())
                      .slice(0, 20)
                      .map((booking) => (
                        <TableRow key={booking.bookingId}>
                          <TableCell>{booking.pnrNo}</TableCell>
                          <TableCell>{booking.customerName}</TableCell>
                          <TableCell>{booking.totalSeats}</TableCell>
                          <TableCell>₹{booking.totalAmount.toLocaleString()}</TableCell>
                          <TableCell>{getPaymentStatusChip(booking.paymentStatus)}</TableCell>
                          <TableCell>{getBookingStatusChip(booking.status)}</TableCell>
                          <TableCell>{new Date(booking.bookedAt).toLocaleDateString()}</TableCell>
                        </TableRow>
                      ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </TabPanel>

          <TabPanel value={tabValue} index={1}>
            <Typography variant="h6" sx={{ mb: 3 }}>
              Top Customers
            </Typography>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Customer Name</TableCell>
                    <TableCell>Bookings Count</TableCell>
                    <TableCell>Revenue</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {(() => {
                    const customerStats: { [key: string]: { count: number, revenue: number } } = {};
                    
                    uniqueBookings.forEach(booking => {
                      if (!customerStats[booking.customerName]) {
                        customerStats[booking.customerName] = { count: 0, revenue: 0 };
                      }
                      customerStats[booking.customerName].count += 1;
                      if (booking.status === 2) { // Confirmed bookings only
                        customerStats[booking.customerName].revenue += booking.totalAmount;
                      }
                    });
                    
                    return Object.entries(customerStats)
                      .sort(([,a], [,b]) => b.count - a.count)
                      .slice(0, 10)
                      .map(([customer, stats]) => (
                        <TableRow key={customer}>
                          <TableCell>{customer}</TableCell>
                          <TableCell>
                            <Chip 
                              label={stats.count >= 15 ? '15+' : stats.count.toString()} 
                              color={stats.count >= 10 ? 'success' : stats.count >= 5 ? 'warning' : 'default'} 
                              size="small" 
                            />
                          </TableCell>
                          <TableCell>₹{stats.revenue.toLocaleString()}</TableCell>
                        </TableRow>
                      ));
                  })()}
                </TableBody>
              </Table>
            </TableContainer>
          </TabPanel>

          <TabPanel value={tabValue} index={2}>
            <Typography variant="h6" sx={{ mb: 3 }}>
              Cancelled Bookings
            </Typography>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>PNR</TableCell>
                    <TableCell>Customer</TableCell>
                    <TableCell>Bus</TableCell>
                    <TableCell>Route</TableCell>
                    <TableCell>Seats</TableCell>
                    <TableCell>Amount</TableCell>
                    <TableCell>Travel Date</TableCell>
                    <TableCell>Booked At</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {uniqueBookings
                    .filter(booking => booking.status === 3)
                    .sort((a, b) => new Date(b.bookedAt).getTime() - new Date(a.bookedAt).getTime())
                    .map((booking) => (
                      <TableRow key={booking.bookingId}>
                        <TableCell>{booking.pnrNo}</TableCell>
                        <TableCell>{booking.customerName}</TableCell>
                        <TableCell>{booking.busName}</TableCell>
                        <TableCell>{booking.route}</TableCell>
                        <TableCell>{booking.totalSeats}</TableCell>
                        <TableCell>₹{booking.totalAmount.toLocaleString()}</TableCell>
                        <TableCell>{new Date(booking.travelDate).toLocaleDateString()}</TableCell>
                        <TableCell>{new Date(booking.bookedAt).toLocaleDateString()}</TableCell>
                      </TableRow>
                    ))}
                </TableBody>
              </Table>
            </TableContainer>
          </TabPanel>
        </Card>
      </Container>
    </Layout>
  );
};

export default AdminReportsPage;