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
import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, PieChart, Pie, Tooltip, AreaChart, Area } from 'recharts';
import Layout from '../components/layout/Layout';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchAllBuses, fetchPendingBuses } from '../features/admin/adminBusSlice';

const BusStatus = {
  PendingApproval: 0,
  Active: 1,
  Inactive: 2,
  Maintenance: 3,
  Rejected: 4
} as const;

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

const AdminBusesPage = () => {
  const dispatch = useAppDispatch();
  const { buses,  loading } = useAppSelector((state) => state.adminBus);
  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    dispatch(fetchAllBuses());
    dispatch(fetchPendingBuses());
  }, [dispatch]);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const getBusTypeName = (type: number) => {
    const types: { [key: number]: string } = { 1: 'AC', 2: 'Non-AC', 3: 'Sleeper', 4: 'Semi-Sleeper', 5: 'Volvo', 6: 'Luxury' };
    return types[type] || 'Unknown';
  };

  const getStatusChip = (status: number) => {
    const statusMap: { [key: number]: React.ReactElement } = {
      [BusStatus.PendingApproval]: <Chip label="Pending Approval" color="warning" size="small" />,
      [BusStatus.Active]: <Chip label="Active" color="success" size="small" />,
      [BusStatus.Inactive]: <Chip label="Inactive" color="default" size="small" />,
      [BusStatus.Maintenance]: <Chip label="Maintenance" color="error" size="small" />,
      [BusStatus.Rejected]: <Chip label="Rejected" color="error" size="small" />
    };
    return statusMap[status] || <Chip label="Unknown" color="default" size="small" />;
  };

  return (
    <Layout>
      <div className="bus-management-container">
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <div className="bus-management-header">
          <h1 className="bus-management-title">🚌 Bus Management</h1>
        </div>

        <div className="admin-card">
          <div className="admin-tabs-container">
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label="Overview" />
              <Tab label={`All Buses (${buses.length})`} />
            </Tabs>
          </div>

          <TabPanel value={tabValue} index={0}>
            <Typography variant="h6" sx={{ mb: 3 }}>
              Bus Analytics Overview
            </Typography>
            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3, mb: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>Bus Status Distribution</Typography>
              <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))', gap: 2 }}>
                <Box sx={{ textAlign: 'center', p: 2, bgcolor: '#e8f5e8', borderRadius: 2, border: '1px solid #4caf50' }}>
                  <Typography variant="h5" sx={{ fontWeight: 700, color: '#2e7d32', mb: 1 }}>
                    {buses.filter(b => b.status === BusStatus.Active).length}
                  </Typography>
                  <Typography variant="body2" sx={{ color: '#2e7d32', fontWeight: 500 }}>Active</Typography>
                  <Typography variant="caption" sx={{ color: '#666' }}>
                    {buses.length > 0 ? ((buses.filter(b => b.status === BusStatus.Active).length / buses.length) * 100).toFixed(0) : 0}%
                  </Typography>
                </Box>
                <Box sx={{ textAlign: 'center', p: 2, bgcolor: '#fff3e0', borderRadius: 2, border: '1px solid #ff9800' }}>
                  <Typography variant="h5" sx={{ fontWeight: 700, color: '#f57c00', mb: 1 }}>
                    {buses.filter(b => b.status === BusStatus.PendingApproval).length}
                  </Typography>
                  <Typography variant="body2" sx={{ color: '#f57c00', fontWeight: 500 }}>Pending</Typography>
                  <Typography variant="caption" sx={{ color: '#666' }}>
                    {buses.length > 0 ? ((buses.filter(b => b.status === BusStatus.PendingApproval).length / buses.length) * 100).toFixed(0) : 0}%
                  </Typography>
                </Box>
                <Box sx={{ textAlign: 'center', p: 2, bgcolor: '#f5f5f5', borderRadius: 2, border: '1px solid #9e9e9e' }}>
                  <Typography variant="h5" sx={{ fontWeight: 700, color: '#616161', mb: 1 }}>
                    {buses.filter(b => b.status === BusStatus.Inactive).length}
                  </Typography>
                  <Typography variant="body2" sx={{ color: '#616161', fontWeight: 500 }}>Inactive</Typography>
                  <Typography variant="caption" sx={{ color: '#666' }}>
                    {buses.length > 0 ? ((buses.filter(b => b.status === BusStatus.Inactive).length / buses.length) * 100).toFixed(0) : 0}%
                  </Typography>
                </Box>
                <Box sx={{ textAlign: 'center', p: 2, bgcolor: '#ffebee', borderRadius: 2, border: '1px solid #f44336' }}>
                  <Typography variant="h5" sx={{ fontWeight: 700, color: '#d32f2f', mb: 1 }}>
                    {buses.filter(b => b.status === BusStatus.Maintenance).length}
                  </Typography>
                  <Typography variant="body2" sx={{ color: '#d32f2f', fontWeight: 500 }}>Maintenance</Typography>
                  <Typography variant="caption" sx={{ color: '#666' }}>
                    {buses.length > 0 ? ((buses.filter(b => b.status === BusStatus.Maintenance).length / buses.length) * 100).toFixed(0) : 0}%
                  </Typography>
                </Box>
                <Box sx={{ textAlign: 'center', p: 2, bgcolor: '#fce4ec', borderRadius: 2, border: '1px solid #e91e63' }}>
                  <Typography variant="h5" sx={{ fontWeight: 700, color: '#c2185b', mb: 1 }}>
                    {buses.filter(b => b.status === BusStatus.Rejected).length}
                  </Typography>
                  <Typography variant="body2" sx={{ color: '#c2185b', fontWeight: 500 }}>Rejected</Typography>
                  <Typography variant="caption" sx={{ color: '#666' }}>
                    {buses.length > 0 ? ((buses.filter(b => b.status === BusStatus.Rejected).length / buses.length) * 100).toFixed(0) : 0}%
                  </Typography>
                </Box>
              </Box>
            </Card>
            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3, mb: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>Bus Type Distribution</Typography>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={[
                  { name: 'AC', count: buses.filter(b => b.busType === 1).length },
                  { name: 'Non-AC', count: buses.filter(b => b.busType === 2).length },
                  { name: 'Sleeper', count: buses.filter(b => b.busType === 3).length },
                  { name: 'Semi-Sleeper', count: buses.filter(b => b.busType === 4).length },
                  { name: 'Volvo', count: buses.filter(b => b.busType === 5).length },
                  { name: 'Luxury', count: buses.filter(b => b.busType === 6).length }
                ]}>
                  <XAxis dataKey="name" />
                  <YAxis />
                  <Bar dataKey="count" fill="#2196f3" />
                </BarChart>
              </ResponsiveContainer>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 3 }}>
                {[
                  { label: 'AC', buses: buses.filter(b => b.busType === 1) },
                  { label: 'Non-AC', buses: buses.filter(b => b.busType === 2) },
                  { label: 'Sleeper', buses: buses.filter(b => b.busType === 3) },
                  { label: 'Semi-Sleeper', buses: buses.filter(b => b.busType === 4) },
                  { label: 'Volvo', buses: buses.filter(b => b.busType === 5) },
                  { label: 'Luxury', buses: buses.filter(b => b.busType === 6) }
                ].map(({ label, buses: typeBuses }) => (
                  <Box key={label}>
                    <Typography variant="body2" sx={{ fontWeight: 600, color: '#333', mb: 1 }}>{label}</Typography>
                    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                      {typeBuses.length > 0 ? typeBuses.map(bus => (
                        <Typography key={bus.busId} variant="caption" sx={{ 
                          bgcolor: '#f5f5f5', 
                          px: 1, 
                          py: 0.5, 
                          borderRadius: 1, 
                          fontSize: '11px' 
                        }}>
                          {bus.busName} ({bus.vendorName})
                        </Typography>
                      )) : (
                        <Typography variant="caption" sx={{ color: '#999', fontStyle: 'italic' }}>No buses</Typography>
                      )}
                    </Box>
                  </Box>
                ))}
              </Box>
            </Card>
            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3, mb: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>Buses per Vendor</Typography>
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie
                    data={(() => {
                      const vendorCounts: { [key: string]: number } = {};
                      buses.forEach(bus => {
                        vendorCounts[bus.vendorName] = (vendorCounts[bus.vendorName] || 0) + 1;
                      });
                      const colors = ['#ff9800', '#2196f3', '#4caf50', '#f44336', '#9c27b0', '#ff5722'];
                      return Object.entries(vendorCounts).map(([vendor, count], index) => ({ 
                        name: vendor, 
                        value: count, 
                        fill: colors[index % colors.length] 
                      }));
                    })()}
                    cx="50%"
                    cy="50%"
                    outerRadius={80}
                    dataKey="value"
                    label={({ name, value }) => `${name}: ${value}`}
                  />
                </PieChart>
              </ResponsiveContainer>
            </Card>
            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3, mb: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>Bus Layout & Capacity</Typography>
              <ResponsiveContainer width="100%" height={300}>
                <AreaChart data={[
                  { layout: '1x1 (<20)', count: buses.filter(b => b.totalSeats < 20).length },
                  { layout: '2x1 (20-29)', count: buses.filter(b => b.totalSeats >= 20 && b.totalSeats < 30).length },
                  { layout: '2x2 (30-45)', count: buses.filter(b => b.totalSeats >= 30 && b.totalSeats <= 45).length },
                  { layout: '2x3 (45+)', count: buses.filter(b => b.totalSeats > 45).length }
                ]}>
                  <XAxis dataKey="layout" />
                  <YAxis domain={[0, 'dataMax']} allowDecimals={false} />
                  <Tooltip formatter={(value) => [`${value} buses`, 'Bus Count']} />
                  <Area type="monotone" dataKey="count" stroke="#4caf50" fill="#4caf50" fillOpacity={0.6} />
                </AreaChart>
              </ResponsiveContainer>
            </Card>
            

            
            <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
              <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>Bus Amenities Distribution</Typography>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={[
                  { name: 'AC', count: buses.filter(b => b.amenities & 1).length },
                  { name: 'WiFi', count: buses.filter(b => b.amenities & 2).length },
                  { name: 'Charging', count: buses.filter(b => b.amenities & 4).length },
                  { name: 'Blanket', count: buses.filter(b => b.amenities & 8).length },
                  { name: 'Pillow', count: buses.filter(b => b.amenities & 16).length },
                  { name: 'Meals', count: buses.filter(b => b.amenities & 32).length },
                  { name: 'Washroom', count: buses.filter(b => b.amenities & 64).length },
                  { name: 'USB', count: buses.filter(b => b.amenities & 128).length },
                  { name: 'Reading Light', count: buses.filter(b => b.amenities & 256).length },
                  { name: 'Entertainment', count: buses.filter(b => b.amenities & 512).length },
                  { name: 'Reclining Seats', count: buses.filter(b => b.amenities & 1024).length }
                ]}>
                  <XAxis dataKey="name" angle={-45} textAnchor="end" height={80} />
                  <YAxis />
                  <Bar dataKey="count" fill="#9c27b0" />
                </BarChart>
              </ResponsiveContainer>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 3 }}>
                {[
                  { label: 'AC', buses: buses.filter(b => b.amenities & 1) },
                  { label: 'WiFi', buses: buses.filter(b => b.amenities & 2) },
                  { label: 'Charging', buses: buses.filter(b => b.amenities & 4) },
                  { label: 'Blanket', buses: buses.filter(b => b.amenities & 8) },
                  { label: 'Pillow', buses: buses.filter(b => b.amenities & 16) },
                  { label: 'Meals', buses: buses.filter(b => b.amenities & 32) },
                  { label: 'Washroom', buses: buses.filter(b => b.amenities & 64) },
                  { label: 'USB', buses: buses.filter(b => b.amenities & 128) },
                  { label: 'Reading Light', buses: buses.filter(b => b.amenities & 256) },
                  { label: 'Entertainment', buses: buses.filter(b => b.amenities & 512) },
                  { label: 'Reclining Seats', buses: buses.filter(b => b.amenities & 1024) }
                ].map(({ label, buses: amenityBuses }) => (
                  <Box key={label}>
                    <Typography variant="body2" sx={{ fontWeight: 600, color: '#333', mb: 1 }}>{label}</Typography>
                    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                      {amenityBuses.length > 0 ? amenityBuses.map(bus => (
                        <Typography key={bus.busId} variant="caption" sx={{ 
                          bgcolor: '#f5f5f5', 
                          px: 1, 
                          py: 0.5, 
                          borderRadius: 1, 
                          fontSize: '11px' 
                        }}>
                          {bus.busName} ({bus.vendorName})
                        </Typography>
                      )) : (
                        <Typography variant="caption" sx={{ color: '#999', fontStyle: 'italic' }}>No buses</Typography>
                      )}
                    </Box>
                  </Box>
                ))}
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
                      <TableCell>Bus Name</TableCell>
                      <TableCell>Registration No</TableCell>
                      <TableCell>Type</TableCell>
                      <TableCell>Seats</TableCell>
                      <TableCell>Vendor</TableCell>
                      <TableCell>Driver</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Active</TableCell>
                      <TableCell>Created</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {buses.map((bus) => (
                      <TableRow key={bus.busId}>
                        <TableCell>{bus.busName}</TableCell>
                        <TableCell>{bus.registrationNo}</TableCell>
                        <TableCell>{getBusTypeName(bus.busType)}</TableCell>
                        <TableCell>{bus.totalSeats}</TableCell>
                        <TableCell>{bus.vendorName}</TableCell>
                        <TableCell>
                          {bus.driverName ? (
                            <Box>
                              <Typography variant="body2">{bus.driverName}</Typography>
                              <Typography variant="caption" color="text.secondary">
                                {bus.driverContact}
                              </Typography>
                            </Box>
                          ) : (
                            <Typography variant="body2" color="text.secondary">-</Typography>
                          )}
                        </TableCell>
                        <TableCell>{getStatusChip(bus.status)}</TableCell>
                        <TableCell>
                          <Chip 
                            label={bus.isActive ? 'Active' : 'Inactive'} 
                            color={bus.isActive ? 'success' : 'default'} 
                            size="small" 
                          />
                        </TableCell>
                        <TableCell>
                          <Typography variant="body2">
                            {new Date(bus.createdAt).toLocaleDateString()}
                          </Typography>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </TabPanel>
        </div>
      </Container>
      </div>
    </Layout>
  );
};

export default AdminBusesPage;