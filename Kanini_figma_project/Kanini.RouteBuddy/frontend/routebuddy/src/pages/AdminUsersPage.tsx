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
  Avatar,
} from '@mui/material';
import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, PieChart, Pie, Tooltip, LineChart, Line } from 'recharts';
import PersonIcon from '@mui/icons-material/Person';
import Layout from '../components/layout/Layout';
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

const AdminUsersPage = () => {
  const [tabValue, setTabValue] = useState(0);
  const [users, setUsers] = useState<any[]>([]);
  const [customers, setCustomers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);


  useEffect(() => {
    fetchUsers();
    fetchCustomers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await api.get('/user?pageNumber=1&pageSize=1000');
      setUsers(response.data.data || response.data || []);
    } catch (error) {
      console.error('Failed to fetch users:', error);
      // Provide sample data for demonstration
      setUsers([
        { userId: 1, email: 'admin@routebuddy.com', phone: '9876543210', role: 3, isActive: true, isEmailVerified: true, lastLogin: new Date(), createdAt: new Date() },
        { userId: 2, email: 'vendor@routebuddy.com', phone: '9876543211', role: 2, isActive: true, isEmailVerified: true, lastLogin: new Date(), createdAt: new Date() },
        { userId: 3, email: 'customer@routebuddy.com', phone: '9876543212', role: 1, isActive: true, isEmailVerified: false, lastLogin: null, createdAt: new Date() }
      ]);
    }
  };

  const fetchCustomers = async () => {
    try {
      const response = await api.get('/customer?pageNumber=1&pageSize=1000');
      setCustomers(response.data.data || response.data || []);
      setLoading(false);
    } catch (error) {
      console.error('Failed to fetch customers:', error);
      // Provide sample data for demonstration
      setCustomers([
        { customerId: 1, firstName: 'John', middleName: '', lastName: 'Doe', dateOfBirth: '1990-01-01', gender: 1, isActive: true, userId: 3 },
        { customerId: 2, firstName: 'Jane', middleName: 'M', lastName: 'Smith', dateOfBirth: '1985-05-15', gender: 2, isActive: true, userId: 4 },
        { customerId: 3, firstName: 'Alex', middleName: '', lastName: 'Johnson', dateOfBirth: '1995-12-20', gender: 3, isActive: false, userId: 5 }
      ]);
      setLoading(false);
    }
  };

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };



  // User analytics data
  const userStats = {
    totalUsers: users.length,
    activeUsers: users.filter(u => u.isActive).length,
    verifiedUsers: users.filter(u => u.isEmailVerified).length,
    customerUsers: users.filter(u => u.role === 1).length,
    vendorUsers: users.filter(u => u.role === 2).length,
    adminUsers: users.filter(u => u.role === 3).length,
  };

  // Registration trends (last 6 months)
  const registrationTrends = (() => {
    const monthlyData: { [key: string]: number } = {};
    users.forEach(user => {
      const month = new Date(user.createdAt).toLocaleDateString('en-US', { month: 'short', year: '2-digit' });
      monthlyData[month] = (monthlyData[month] || 0) + 1;
    });
    return Object.entries(monthlyData)
      .map(([month, count]) => ({ name: month, users: count }))
      .slice(-6);
  })();

  // Age distribution from customers
  const ageDistribution = (() => {
    const ageGroups = { '18-25': 0, '26-35': 0, '36-45': 0, '46-55': 0, '55+': 0 };
    customers.forEach(customer => {
      const age = new Date().getFullYear() - new Date(customer.dateOfBirth).getFullYear();
      if (age <= 25) ageGroups['18-25']++;
      else if (age <= 35) ageGroups['26-35']++;
      else if (age <= 45) ageGroups['36-45']++;
      else if (age <= 55) ageGroups['46-55']++;
      else ageGroups['55+']++;
    });
    return Object.entries(ageGroups).map(([range, count]) => ({ name: range, value: count }));
  })();

  // Gender distribution
  const genderDistribution = (() => {
    const genders = { Male: 0, Female: 0, Other: 0 };
    customers.forEach(customer => {
      if (customer.gender === 1) genders.Male++;
      else if (customer.gender === 2) genders.Female++;
      else genders.Other++;
    });
    return Object.entries(genders).map(([gender, count]) => ({ name: gender, value: count }));
  })();

  const getRoleName = (role: number) => {
    switch (role) {
      case 1: return 'Customer';
      case 2: return 'Vendor';
      case 3: return 'Admin';
      default: return 'Unknown';
    }
  };

  const getGenderName = (gender: number) => {
    switch (gender) {
      case 1: return 'Male';
      case 2: return 'Female';
      case 3: return 'Other';
      default: return 'Not specified';
    }
  };

  return (
    <Layout>
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Typography variant="h4" sx={{ mb: 3, fontWeight: 600 }}>
          👥 User Management
        </Typography>

        <Card>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label="Overview" />
              <Tab label={`All Users (${users.length})`} />
              <Tab label={`Customers (${customers.length})`} />
            </Tabs>
          </Box>

          <TabPanel value={tabValue} index={0}>
            <Typography variant="h6" sx={{ mb: 3 }}>
              User Analytics Overview
            </Typography>
            
            {/* User Statistics Overview */}
            <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: 2, mb: 3 }}>
              <Card sx={{ p: 2, textAlign: 'center', bgcolor: '#e3f2fd', border: '1px solid #2196f3' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: '#1976d2' }}>
                  {userStats.totalUsers}
                </Typography>
                <Typography variant="body2" sx={{ color: '#1976d2', fontWeight: 500 }}>Total Users</Typography>
              </Card>
              <Card sx={{ p: 2, textAlign: 'center', bgcolor: '#e8f5e8', border: '1px solid #4caf50' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: '#2e7d32' }}>
                  {userStats.activeUsers}
                </Typography>
                <Typography variant="body2" sx={{ color: '#2e7d32', fontWeight: 500 }}>Active Users</Typography>
              </Card>
              <Card sx={{ p: 2, textAlign: 'center', bgcolor: '#fff3e0', border: '1px solid #ff9800' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: '#f57c00' }}>
                  {userStats.verifiedUsers}
                </Typography>
                <Typography variant="body2" sx={{ color: '#f57c00', fontWeight: 500 }}>Verified Users</Typography>
              </Card>
              <Card sx={{ p: 2, textAlign: 'center', bgcolor: '#fce4ec', border: '1px solid #e91e63' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: '#c2185b' }}>
                  {userStats.customerUsers}
                </Typography>
                <Typography variant="body2" sx={{ color: '#c2185b', fontWeight: 500 }}>Customers</Typography>
              </Card>
            </Box>

            <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 3, mb: 3 }}>
              <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>📈 User Registration Trends</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <LineChart data={registrationTrends}>
                    <XAxis dataKey="name" tick={{ fontSize: 11 }} />
                    <YAxis tick={{ fontSize: 11 }} />
                    <Tooltip />
                    <Line type="monotone" dataKey="users" stroke="#2196f3" strokeWidth={2} />
                  </LineChart>
                </ResponsiveContainer>
              </Card>
              
              <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>👤 User Roles Distribution</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <PieChart>
                    <Pie
                      data={[
                        { name: 'Customers', value: userStats.customerUsers, fill: '#4caf50' },
                        { name: 'Vendors', value: userStats.vendorUsers, fill: '#ff9800' },
                        { name: 'Admins', value: userStats.adminUsers, fill: '#f44336' }
                      ]}
                      cx="50%"
                      cy="50%"
                      outerRadius={70}
                      dataKey="value"
                      label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                    />
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
              </Card>
            </Box>

            <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 3, mb: 3 }}>
              <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>🎂 Age Distribution</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <BarChart data={ageDistribution}>
                    <XAxis dataKey="name" tick={{ fontSize: 11 }} />
                    <YAxis tick={{ fontSize: 11 }} />
                    <Tooltip />
                    <Bar dataKey="value" fill="#9c27b0" />
                  </BarChart>
                </ResponsiveContainer>
              </Card>
              
              <Card sx={{ bgcolor: 'white', border: '1px solid #f0f0f0', borderRadius: 2, p: 3 }}>
                <Typography variant="h6" sx={{ mb: 3, fontWeight: 600, color: '#333' }}>⚧ Gender Distribution</Typography>
                <ResponsiveContainer width="100%" height={250}>
                  <PieChart>
                    <Pie
                      data={genderDistribution}
                      cx="50%"
                      cy="50%"
                      outerRadius={70}
                      dataKey="value"
                      label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                    />
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
              </Card>
            </Box>
          </TabPanel>

          <TabPanel value={tabValue} index={1}>
            <Typography variant="h6" sx={{ mb: 3 }}>
              All Users
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
                      <TableCell>User</TableCell>
                      <TableCell>Email</TableCell>
                      <TableCell>Phone</TableCell>
                      <TableCell>Role</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Last Login</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {users.map((user) => (
                      <TableRow key={user.userId}>
                        <TableCell>
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                            <Avatar sx={{ bgcolor: '#2196f3' }}>
                              <PersonIcon />
                            </Avatar>
                            <Typography variant="body2" sx={{ fontWeight: 500 }}>
                              User #{user.userId}
                            </Typography>
                          </Box>
                        </TableCell>
                        <TableCell>{user.email}</TableCell>
                        <TableCell>{user.phone}</TableCell>
                        <TableCell>
                          <Chip 
                            label={getRoleName(user.role)} 
                            color={user.role === 3 ? 'error' : user.role === 2 ? 'warning' : 'primary'}
                            size="small"
                          />
                        </TableCell>
                        <TableCell>
                          <Box sx={{ display: 'flex', gap: 1 }}>
                            <Chip 
                              label={user.isActive ? 'Active' : 'Inactive'} 
                              color={user.isActive ? 'success' : 'default'}
                              size="small"
                            />
                            {user.isEmailVerified && (
                              <Chip label="Verified" color="info" size="small" />
                            )}
                          </Box>
                        </TableCell>
                        <TableCell>
                          {user.lastLogin ? new Date(user.lastLogin).toLocaleDateString() : 'Never'}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </TabPanel>

          <TabPanel value={tabValue} index={2}>
            <Typography variant="h6" sx={{ mb: 3 }}>
              Customer Details
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
                      <TableCell>Customer</TableCell>
                      <TableCell>Full Name</TableCell>
                      <TableCell>Age</TableCell>
                      <TableCell>Gender</TableCell>
                      <TableCell>Status</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {customers.map((customer) => (
                      <TableRow key={customer.customerId}>
                        <TableCell>
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                            <Avatar sx={{ bgcolor: '#4caf50' }}>
                              <PersonIcon />
                            </Avatar>
                            <Typography variant="body2" sx={{ fontWeight: 500 }}>
                              #{customer.customerId}
                            </Typography>
                          </Box>
                        </TableCell>
                        <TableCell>
                          <Typography variant="body2" sx={{ fontWeight: 500 }}>
                            {customer.fullName || 'N/A'}
                          </Typography>
                        </TableCell>
                        <TableCell>{customer.age || 'N/A'}</TableCell>
                        <TableCell>{getGenderName(customer.gender)}</TableCell>
                        <TableCell>
                          <Chip 
                            label={customer.isActive ? 'Active' : 'Inactive'} 
                            color={customer.isActive ? 'success' : 'default'}
                            size="small"
                          />
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </TabPanel>
        </Card>


      </Container>
    </Layout>
  );
};

export default AdminUsersPage;