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
import { LineChart, Line, XAxis, YAxis, ResponsiveContainer, AreaChart, Area, PieChart, Pie, Cell, BarChart, Bar, Tooltip } from 'recharts';
import Layout from '../components/layout/Layout';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchAllBookings, fetchBookingStatusSummary } from '../features/admin/adminBookingSlice';
import { fetchAllBuses } from '../features/admin/adminBusSlice';

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

enum BookingStatus {
  Pending = 1,
  Confirmed = 2,
  Cancelled = 3
}

const AdminBookingsPage = () => {
  const dispatch = useAppDispatch();
  const { bookings, statusSummary, loading } = useAppSelector((state) => state.adminBooking);
  const { buses } = useAppSelector((state) => state.adminBus);
  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    dispatch(fetchAllBookings());
    dispatch(fetchBookingStatusSummary());
    dispatch(fetchAllBuses());
  }, [dispatch]);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const getStatusChip = (status: number) => {
    const statusMap: { [key: number]: JSX.Element } = {
      [BookingStatus.Pending]: <Chip label="Pending" color="warning" size="small" />,
      [BookingStatus.Confirmed]: <Chip label="Confirmed" color="success" size="small" />,
      [BookingStatus.Cancelled]: <Chip label="Cancelled" color="error" size="small" />
    };
    return statusMap[status] || <Chip label="Unknown" color="default" size="small" />;
  };

  return (
    <Layout>
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Typography variant="h4" sx={{ mb: 3, fontWeight: 600 }}>
          📋 Booking Management
        </Typography>

        <Card>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label="Overview" />
              <Tab label={`All Bookings (${bookings.length})`} />
              <Tab label="📋 Latest Bookings" />
              <Tab label="👥 Top Customers" />
              <Tab label="❌ Cancellations" />
            </Tabs>
          </Box>

          <TabPanel value={tabValue} index={0}>
            <Typography variant="h6" sx={{ mb: 3 }}>
              Booking Analytics Overview
            </Typography>
            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3, mb: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>Booking Status Distribution</Typography>
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie
                    data={[
                      { name: 'Confirmed', value: statusSummary?.confirmedBookings || 0, fill: '#4caf50' },
                      { name: 'Pending', value: statusSummary?.pendingBookings || 0, fill: '#ff9800' },
                      { name: 'Cancelled', value: statusSummary?.cancelledBookings || 0, fill: '#f44336' }
                    ]}
                    cx="50%"
                    cy="50%"
                    outerRadius={80}
                    dataKey="value"
                    label={({ name, value }) => value > 0 ? `${name}: ${value}` : ''}
                  />
                </PieChart>
              </ResponsiveContainer>
              <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: 2, mt: 2 }}>
                <Box sx={{ textAlign: 'center', p: 2, bgcolor: '#e8f5e8', borderRadius: 2 }}>
                  <Typography variant="h5" sx={{ fontWeight: 700, color: '#2e7d32', mb: 1 }}>
                    {statusSummary?.totalBookings || 0}
                  </Typography>
                  <Typography variant="body2" sx={{ color: '#2e7d32', fontWeight: 500 }}>Total Bookings</Typography>
                </Box>
                <Box sx={{ textAlign: 'center', p: 2, bgcolor: '#fff3e0', borderRadius: 2 }}>
                  <Typography variant="h5" sx={{ fontWeight: 700, color: '#f57c00', mb: 1 }}>
                    ₹{statusSummary?.totalRevenue?.toLocaleString() || 0}
                  </Typography>
                  <Typography variant="body2" sx={{ color: '#f57c00', fontWeight: 500 }}>Total Revenue</Typography>
                </Box>
              </Box>
            </Card>
            

            

            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3, mb: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>Yearly Booking Trends</Typography>
              <ResponsiveContainer width="100%" height={300}>
                <LineChart data={(() => {
                  const currentYear = new Date().getFullYear();
                  const last5Years = Array.from({ length: 5 }, (_, i) => currentYear - 4 + i);
                  const yearlyData: { [key: string]: number } = {};
                  
                  const uniqueBookings = bookings.filter((booking, index, arr) => 
                    arr.findIndex(b => b.bookingId === booking.bookingId) === index
                  );
                  
                  uniqueBookings.forEach(booking => {
                    const bookingDate = new Date(booking.bookedAt);
                    if (isNaN(bookingDate.getTime())) return;
                    const year = bookingDate.getFullYear();
                    if (last5Years.includes(year)) {
                      yearlyData[year.toString()] = (yearlyData[year.toString()] || 0) + 1;
                    }
                  });
                  
                  return last5Years.map(year => ({
                    year: year.toString(),
                    bookings: yearlyData[year.toString()] || 0
                  }));
                })()}>
                  <XAxis dataKey="year" />
                  <YAxis allowDecimals={false} domain={[0, 'dataMax + 10']} />
                  <Tooltip />
                  <Line type="monotone" dataKey="bookings" stroke="#2196f3" strokeWidth={2} name="Bookings" />
                </LineChart>
              </ResponsiveContainer>
            </Card>
            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>💰 Revenue per Vendor</Typography>
              <ResponsiveContainer width="100%" height={350}>
                <BarChart data={(() => {
                  const vendorRevenue: { [key: string]: number } = {};
                  const uniqueConfirmedBookings = bookings.filter(b => b.status === 2)
                    .filter((booking, index, arr) => 
                      arr.findIndex(b => b.bookingId === booking.bookingId) === index
                    );
                  
                  uniqueConfirmedBookings.forEach(booking => {
                    const vendorName = booking.busName || 'Unknown';
                    vendorRevenue[vendorName] = (vendorRevenue[vendorName] || 0) + (booking.totalAmount || 0);
                  });
                  
                  return Object.entries(vendorRevenue)
                    .sort(([,a], [,b]) => b - a)
                    .map(([vendor, revenue]) => ({ name: vendor, revenue: Math.round(revenue) }));
                })()}
                margin={{ top: 20, right: 30, left: 40, bottom: 60 }}>
                  <XAxis 
                    dataKey="name" 
                    angle={-45} 
                    textAnchor="end" 
                    height={80}
                    tick={{ fontSize: 12 }}
                  />
                  <YAxis 
                    tickFormatter={(value) => `₹${(value/1000).toFixed(0)}K`}
                    tick={{ fontSize: 12 }}
                  />
                  <Tooltip 
                    formatter={(value) => [`₹${Number(value).toLocaleString('en-IN')}`, 'Revenue']}
                    labelStyle={{ color: '#333', fontWeight: 600 }}
                    contentStyle={{ 
                      backgroundColor: '#fff', 
                      border: '1px solid #ddd', 
                      borderRadius: '8px',
                      boxShadow: '0 4px 12px rgba(0,0,0,0.1)'
                    }}
                  />
                  <Bar 
                    dataKey="revenue" 
                    fill="#4caf50"
                    radius={[4, 4, 0, 0]}
                  />
                </BarChart>
              </ResponsiveContainer>
              <Box sx={{ mt: 2, p: 2, bgcolor: '#f8f9fa', borderRadius: 1 }}>
                <Typography variant="body2" sx={{ color: '#666', textAlign: 'center' }}>
                  Total Revenue: ₹{(() => {
                    const uniqueBookings = bookings.filter(b => b.status === 2)
                      .filter((booking, index, arr) => 
                        arr.findIndex(b => b.bookingId === booking.bookingId) === index
                      );
                    const total = uniqueBookings.reduce((sum, b) => sum + (b.totalAmount || 0), 0);
                    return total.toLocaleString('en-IN');
                  })()} from confirmed bookings
                </Typography>
              </Box>
            </Card>
            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>🔁 Customer Repeat Bookings</Typography>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                {(() => {
                  const customerCounts: { [key: string]: number } = {};
                  const uniqueBookings = bookings.filter((booking, index, arr) => 
                    arr.findIndex(b => b.bookingId === booking.bookingId) === index
                  );
                  
                  uniqueBookings.forEach(booking => {
                    customerCounts[booking.customerName] = (customerCounts[booking.customerName] || 0) + 1;
                  });
                  
                  return Object.entries(customerCounts)
                    .sort(([,a], [,b]) => b - a)
                    .slice(0, 5)
                    .map(([customer, count]) => (
                      <Card key={customer} sx={{ 
                        p: 2.5, 
                        border: '1px solid #e0e0e0', 
                        borderRadius: 2,
                        bgcolor: '#fafafa',
                        '&:hover': { bgcolor: '#f5f5f5', transform: 'translateY(-1px)' },
                        transition: 'all 0.2s ease'
                      }}>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                          <Box>
                            <Typography variant="body1" sx={{ fontWeight: 600, color: '#333', mb: 0.5 }}>
                              {customer}
                            </Typography>
                            <Typography variant="caption" sx={{ color: '#666' }}>
                              Loyal Customer
                            </Typography>
                          </Box>
                          <Box sx={{ textAlign: 'right' }}>
                            <Typography variant="h4" sx={{ 
                              fontWeight: 700, 
                              color: count >= 10 ? '#2e7d32' : count >= 5 ? '#f57c00' : '#666',
                              mb: 0.5
                            }}>
                              {count >= 15 ? '15+' : count}
                            </Typography>
                            <Typography variant="caption" sx={{ color: '#666' }}>
                              Bookings
                            </Typography>
                          </Box>
                        </Box>
                      </Card>
                    ));
                })()}
              </Box>
            </Card>
            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>📊 Seat Utilization per Bus</Typography>
              <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: 3 }}>
                {(() => {
                  const uniqueBookings = bookings.filter((booking, index, arr) => 
                    arr.findIndex(b => b.bookingId === booking.bookingId) === index
                  );
                  
                  return buses.slice(0, 4).map(bus => {
                    const busBookings = uniqueBookings.filter(booking => booking.busName === bus.busName);
                    const bookedSeats = busBookings.reduce((sum, booking) => sum + (booking.totalSeats || 0), 0);
                    const totalSeats = bus.totalSeats || 40;
                    const utilization = totalSeats > 0 ? Math.round((bookedSeats / totalSeats) * 100) : 0;
                    
                    return (
                      <Card key={bus.busId} sx={{ p: 3, border: '1px solid #e0e0e0', borderRadius: 2 }}>
                        <Typography variant="h6" sx={{ mb: 2, fontWeight: 600, color: '#333' }}>
                          {bus.busName}
                        </Typography>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 3 }}>
                          <Box sx={{ position: 'relative', width: 120, height: 120 }}>
                            <svg width="120" height="120" viewBox="0 0 120 120">
                              <circle
                                cx="60"
                                cy="60"
                                r="45"
                                fill="none"
                                stroke="#e0e0e0"
                                strokeWidth="12"
                              />
                              <circle
                                cx="60"
                                cy="60"
                                r="45"
                                fill="none"
                                stroke={utilization >= 80 ? '#4caf50' : utilization >= 60 ? '#ff9800' : '#f44336'}
                                strokeWidth="12"
                                strokeDasharray={`${(utilization / 100) * 282.74} 282.74`}
                                strokeDashoffset="70.69"
                                transform="rotate(-90 60 60)"
                                strokeLinecap="round"
                              />
                            </svg>
                            <Box sx={{ 
                              position: 'absolute', 
                              top: '50%', 
                              left: '50%', 
                              transform: 'translate(-50%, -50%)',
                              textAlign: 'center'
                            }}>
                              <Typography variant="h5" sx={{ fontWeight: 700, color: '#333' }}>
                                {utilization}%
                              </Typography>
                              <Typography variant="caption" sx={{ color: '#666' }}>
                                Utilization
                              </Typography>
                            </Box>
                          </Box>
                          <Box sx={{ flex: 1 }}>
                            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                              <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                                <Typography variant="body2" sx={{ color: '#666' }}>Total Seats</Typography>
                                <Typography variant="h6" sx={{ fontWeight: 600 }}>{totalSeats}</Typography>
                              </Box>
                              <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                                <Typography variant="body2" sx={{ color: '#666' }}>Booked Seats</Typography>
                                <Typography variant="h6" sx={{ fontWeight: 600 }}>{bookedSeats}</Typography>
                              </Box>
                            </Box>
                          </Box>
                        </Box>
                      </Card>
                    );
                  });
                })()}
              </Box>
            </Card>
          </TabPanel>

          <TabPanel value={tabValue} index={1}>
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
                      <TableCell>Bus</TableCell>
                      <TableCell>Route</TableCell>
                      <TableCell>Travel Date</TableCell>
                      <TableCell>Seats</TableCell>
                      <TableCell>Amount</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Payment</TableCell>
                      <TableCell>Booked On</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {bookings.map((booking, index) => (
                      <TableRow key={`${booking.bookingId}-${index}`}>
                        <TableCell>{booking.pnrNo}</TableCell>
                        <TableCell>{booking.customerName}</TableCell>
                        <TableCell>{booking.busName}</TableCell>
                        <TableCell>{booking.route}</TableCell>
                        <TableCell>{new Date(booking.travelDate).toLocaleDateString()}</TableCell>
                        <TableCell>{booking.totalSeats}</TableCell>
                        <TableCell>₹{booking.totalAmount.toLocaleString()}</TableCell>
                        <TableCell>{getStatusChip(booking.status)}</TableCell>
                        <TableCell>
                          <Chip 
                            label={booking.paymentStatus === 2 ? 'Completed' : booking.paymentStatus === 1 ? 'Pending' : 'Failed'} 
                            color={booking.paymentStatus === 2 ? 'success' : booking.paymentStatus === 1 ? 'warning' : 'error'} 
                            size="small" 
                          />
                        </TableCell>
                        <TableCell>
                          <Typography variant="body2">
                            {new Date(booking.bookedAt).toLocaleDateString()}
                          </Typography>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </TabPanel>

          <TabPanel value={tabValue} index={2}>
            <Typography variant="h6" sx={{ mb: 3 }}>Latest Bookings</Typography>
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
                    {(() => {
                      const uniqueBookings = bookings.filter((booking, index, arr) => 
                        arr.findIndex(b => b.bookingId === booking.bookingId) === index
                      );
                      return uniqueBookings
                        .sort((a, b) => new Date(b.bookedAt).getTime() - new Date(a.bookedAt).getTime())
                        .slice(0, 20)
                        .map((booking) => (
                          <TableRow key={booking.bookingId}>
                            <TableCell>{booking.pnrNo}</TableCell>
                            <TableCell>{booking.customerName}</TableCell>
                            <TableCell>{booking.totalSeats}</TableCell>
                            <TableCell>₹{booking.totalAmount.toLocaleString()}</TableCell>
                            <TableCell>
                              <Chip 
                                label={booking.paymentStatus === 2 ? 'Completed' : booking.paymentStatus === 1 ? 'Pending' : 'Failed'} 
                                color={booking.paymentStatus === 2 ? 'success' : booking.paymentStatus === 1 ? 'warning' : 'error'} 
                                size="small" 
                              />
                            </TableCell>
                            <TableCell>{getStatusChip(booking.status)}</TableCell>
                            <TableCell>{new Date(booking.bookedAt).toLocaleDateString()}</TableCell>
                          </TableRow>
                        ));
                    })()}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </TabPanel>

          <TabPanel value={tabValue} index={3}>
            <Typography variant="h6" sx={{ mb: 3 }}>Top Customers</Typography>
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
                    const uniqueBookings = bookings.filter((booking, index, arr) => 
                      arr.findIndex(b => b.bookingId === booking.bookingId) === index
                    );
                    
                    uniqueBookings.forEach(booking => {
                      if (!customerStats[booking.customerName]) {
                        customerStats[booking.customerName] = { count: 0, revenue: 0 };
                      }
                      customerStats[booking.customerName].count += 1;
                      if (booking.status === 2) {
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

          <TabPanel value={tabValue} index={4}>
            <Typography variant="h6" sx={{ mb: 3 }}>Cancelled Bookings</Typography>
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
                  {(() => {
                    const uniqueBookings = bookings.filter((booking, index, arr) => 
                      arr.findIndex(b => b.bookingId === booking.bookingId) === index
                    );
                    return uniqueBookings
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
                      ));
                  })()}
                </TableBody>
              </Table>
            </TableContainer>
          </TabPanel>
        </Card>
      </Container>
    </Layout>
  );
};

export default AdminBookingsPage;