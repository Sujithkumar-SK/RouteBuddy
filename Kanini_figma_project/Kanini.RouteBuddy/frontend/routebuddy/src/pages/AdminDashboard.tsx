import { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Tabs,
  Tab,
  Card,
  CardContent,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Alert,
  CircularProgress,
} from '@mui/material';
import { CheckCircle, Cancel, Visibility } from '@mui/icons-material';
import Layout from '../components/layout/Layout';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchPendingVendors, fetchAllVendors, approveVendor, rejectVendor, clearError } from '../features/admin/adminSlice';
import type { AdminVendor } from '../features/admin/adminAPI';

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

const AdminDashboard = () => {
  const dispatch = useAppDispatch();
  const { pendingVendors, vendors, loading, error } = useAppSelector((state) => state.admin);
  
  const [tabValue, setTabValue] = useState(0);
  const [rejectDialog, setRejectDialog] = useState<{ open: boolean; vendor: AdminVendor | null }>({
    open: false,
    vendor: null,
  });
  const [rejectionReason, setRejectionReason] = useState('');

  useEffect(() => {
    dispatch(fetchPendingVendors({ pageNumber: 1, pageSize: 50 }));
    dispatch(fetchAllVendors({ pageNumber: 1, pageSize: 50 }));
  }, [dispatch]);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleApprove = async (vendorId: number) => {
    await dispatch(approveVendor(vendorId));
  };

  const handleRejectClick = (vendor: AdminVendor) => {
    setRejectDialog({ open: true, vendor });
    setRejectionReason('');
  };

  const handleRejectConfirm = async () => {
    if (rejectDialog.vendor && rejectionReason.trim()) {
      await dispatch(rejectVendor({
        vendorId: rejectDialog.vendor.vendorId,
        rejectionData: { rejectionReason: rejectionReason.trim() }
      }));
      setRejectDialog({ open: false, vendor: null });
      setRejectionReason('');
    }
  };

  const getStatusChip = (status: number) => {
    switch (status) {
      case 0: return <Chip label="Pending" color="warning" size="small" />;
      case 1: return <Chip label="Active" color="success" size="small" />;
      case 2: return <Chip label="Inactive" color="default" size="small" />;
      case 3: return <Chip label="Suspended" color="error" size="small" />;
      case 4: return <Chip label="Rejected" color="error" size="small" />;
      default: return <Chip label="Unknown" color="default" size="small" />;
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-IN', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
    });
  };

  return (
    <Layout>
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Typography variant="h4" sx={{ mb: 3, fontWeight: 600 }}>
          🛡️ Admin Dashboard
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }} onClose={() => dispatch(clearError())}>
            {error}
          </Alert>
        )}

        <Card>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label={`Pending Approvals (${pendingVendors.length})`} />
              <Tab label={`All Vendors (${vendors.length})`} />
            </Tabs>
          </Box>

          <TabPanel value={tabValue} index={0}>
            <Typography variant="h6" sx={{ mb: 2 }}>
              Vendors Awaiting Approval
            </Typography>
            
            {loading.pending ? (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            ) : pendingVendors.length === 0 ? (
              <Alert severity="info">No pending vendor approvals</Alert>
            ) : (
              <TableContainer component={Paper}>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Agency Name</TableCell>
                      <TableCell>Owner</TableCell>
                      <TableCell>Email</TableCell>
                      <TableCell>Phone</TableCell>
                      <TableCell>Applied On</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {pendingVendors.map((vendor) => (
                      <TableRow key={vendor.vendorId}>
                        <TableCell>{vendor.agencyName}</TableCell>
                        <TableCell>{vendor.ownerName}</TableCell>
                        <TableCell>{vendor.email}</TableCell>
                        <TableCell>{vendor.phone}</TableCell>
                        <TableCell>{formatDate(vendor.createdOn)}</TableCell>
                        <TableCell>{getStatusChip(vendor.status)}</TableCell>
                        <TableCell>
                          <Box display="flex" gap={1}>
                            <Button
                              size="small"
                              variant="contained"
                              color="success"
                              startIcon={<CheckCircle />}
                              onClick={() => handleApprove(vendor.vendorId)}
                              disabled={loading.action}
                            >
                              Approve
                            </Button>
                            <Button
                              size="small"
                              variant="outlined"
                              color="error"
                              startIcon={<Cancel />}
                              onClick={() => handleRejectClick(vendor)}
                              disabled={loading.action}
                            >
                              Reject
                            </Button>
                          </Box>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </TabPanel>

          <TabPanel value={tabValue} index={1}>
            <Typography variant="h6" sx={{ mb: 2 }}>
              All Registered Vendors
            </Typography>
            
            {loading.vendors ? (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            ) : (
              <TableContainer component={Paper}>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Agency Name</TableCell>
                      <TableCell>Owner</TableCell>
                      <TableCell>Email</TableCell>
                      <TableCell>Total Buses</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Active</TableCell>
                      <TableCell>Registered On</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {vendors.map((vendor) => (
                      <TableRow key={vendor.vendorId}>
                        <TableCell>{vendor.agencyName}</TableCell>
                        <TableCell>{vendor.ownerName}</TableCell>
                        <TableCell>{vendor.email}</TableCell>
                        <TableCell>{vendor.totalBuses}</TableCell>
                        <TableCell>{getStatusChip(vendor.status)}</TableCell>
                        <TableCell>
                          <Chip 
                            label={vendor.isActive ? 'Yes' : 'No'} 
                            color={vendor.isActive ? 'success' : 'default'} 
                            size="small" 
                          />
                        </TableCell>
                        <TableCell>{formatDate(vendor.createdOn)}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </TabPanel>
        </Card>

        {/* Rejection Dialog */}
        <Dialog open={rejectDialog.open} onClose={() => setRejectDialog({ open: false, vendor: null })} maxWidth="sm" fullWidth>
          <DialogTitle>Reject Vendor Application</DialogTitle>
          <DialogContent>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              Rejecting: {rejectDialog.vendor?.agencyName} ({rejectDialog.vendor?.ownerName})
            </Typography>
            <TextField
              fullWidth
              multiline
              rows={4}
              label="Rejection Reason"
              value={rejectionReason}
              onChange={(e) => setRejectionReason(e.target.value)}
              placeholder="Please provide a clear reason for rejection..."
              required
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setRejectDialog({ open: false, vendor: null })}>
              Cancel
            </Button>
            <Button 
              onClick={handleRejectConfirm} 
              color="error" 
              variant="contained"
              disabled={!rejectionReason.trim() || loading.action}
            >
              {loading.action ? <CircularProgress size={20} /> : 'Reject Vendor'}
            </Button>
          </DialogActions>
        </Dialog>
      </Container>
    </Layout>
  );
};

export default AdminDashboard;