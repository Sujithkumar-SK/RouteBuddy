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
import { CheckCircle, Cancel, Visibility, Description, CheckCircleOutline, ErrorOutline } from '@mui/icons-material';
import Layout from '../components/layout/Layout';
import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchPendingVendors, fetchAllVendors, approveVendor, rejectVendor, clearError } from '../features/admin/adminSlice';
import type { AdminVendor, VendorApproval } from '../features/admin/adminAPI';
import { adminAPI } from '../features/admin/adminAPI';

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
  const [viewDialog, setViewDialog] = useState<{ open: boolean; vendor: VendorApproval | null; loading: boolean }>({
    open: false,
    vendor: null,
    loading: false,
  });

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

  const handleViewVendor = async (vendorId: number) => {
    setViewDialog({ open: true, vendor: null, loading: true });
    try {
      const vendorData = await adminAPI.getVendorForApproval(vendorId);
      setViewDialog({ open: true, vendor: vendorData, loading: false });
    } catch (error) {
      console.error('Error fetching vendor details:', error);
      setViewDialog({ open: false, vendor: null, loading: false });
    }
  };

  const getDocumentTypeName = (type: number) => {
    switch (type) {
      case 1: return 'Business License';
      case 2: return 'Tax Registration';
      case 3: return 'Insurance Certificate';
      case 4: return 'Owner Identity';
      case 5: return 'Bank Details';
      default: return 'Unknown';
    }
  };

  const getDocumentStatusChip = (status: number) => {
    switch (status) {
      case 1: return <Chip label="Pending" color="warning" size="small" />;
      case 2: return <Chip label="Verified" color="success" size="small" />;
      case 3: return <Chip label="Rejected" color="error" size="small" />;
      default: return <Chip label="Unknown" color="default" size="small" />;
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
      <div className="admin-dashboard-container">
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <div className="admin-dashboard-header">
          <h1 className="admin-dashboard-title">🛡️ Admin Dashboard</h1>
        </div>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }} onClose={() => dispatch(clearError())}>
            {error}
          </Alert>
        )}

        <div className="admin-card">
          <div className="admin-tabs-container">
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label={`Pending Approvals (${pendingVendors.length})`} />
              <Tab label={`All Vendors (${vendors.length})`} />
            </Tabs>
          </div>

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
                              variant="outlined"
                              startIcon={<Visibility />}
                              onClick={() => handleViewVendor(vendor.vendorId)}
                              sx={{ mr: 1 }}
                            >
                              View
                            </Button>
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
        </div>

        {/* Vendor Details Dialog */}
        <Dialog open={viewDialog.open} onClose={() => setViewDialog({ open: false, vendor: null, loading: false })} maxWidth="md" fullWidth>
          <DialogTitle>Vendor Application Details</DialogTitle>
          <DialogContent>
            {viewDialog.loading ? (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            ) : viewDialog.vendor ? (
              <Box>
                <Typography variant="h6" sx={{ mb: 2 }}>Basic Information</Typography>
                <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 2, mb: 3 }}>
                  <TextField label="Agency Name" value={viewDialog.vendor.agencyName} InputProps={{ readOnly: true }} />
                  <TextField label="Owner Name" value={viewDialog.vendor.ownerName} InputProps={{ readOnly: true }} />
                  <TextField label="Email" value={viewDialog.vendor.email} InputProps={{ readOnly: true }} />
                  <TextField label="Phone" value={viewDialog.vendor.phone} InputProps={{ readOnly: true }} />
                  <TextField label="Fleet Size" value={viewDialog.vendor.fleetSize} InputProps={{ readOnly: true }} />
                  <TextField label="Status" value={viewDialog.vendor.statusText} InputProps={{ readOnly: true }} />
                </Box>
                <TextField 
                  fullWidth 
                  label="Office Address" 
                  value={viewDialog.vendor.officeAddress} 
                  InputProps={{ readOnly: true }} 
                  sx={{ mb: 3 }}
                />
                <TextField 
                  fullWidth 
                  label="Business License Number" 
                  value={viewDialog.vendor.businessLicenseNumber} 
                  InputProps={{ readOnly: true }} 
                  sx={{ mb: 3 }}
                />
                {viewDialog.vendor.taxRegistrationNumber && (
                  <TextField 
                    fullWidth 
                    label="Tax Registration Number" 
                    value={viewDialog.vendor.taxRegistrationNumber} 
                    InputProps={{ readOnly: true }} 
                    sx={{ mb: 3 }}
                  />
                )}
                
                <Typography variant="h6" sx={{ mb: 2 }}>Documents</Typography>
                {viewDialog.vendor.documents.length > 0 ? (
                  <TableContainer component={Paper} sx={{ mb: 2 }}>
                    <Table size="small">
                      <TableHead>
                        <TableRow>
                          <TableCell>Document Type</TableCell>
                          <TableCell>Status</TableCell>
                          <TableCell>Uploaded</TableCell>
                          <TableCell>Actions</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {viewDialog.vendor.documents.map((doc) => (
                          <TableRow key={doc.documentId}>
                            <TableCell>{getDocumentTypeName(doc.documentFile)}</TableCell>
                            <TableCell>{getDocumentStatusChip(doc.status)}</TableCell>
                            <TableCell>{new Date(doc.uploadedAt).toLocaleDateString()}</TableCell>
                            <TableCell>
                              <Button 
                                size="small" 
                                startIcon={<Description />} 
                                onClick={() => window.open(`${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5172'}${doc.documentPath}`, '_blank')}
                              >
                                View
                              </Button>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                ) : (
                  <Alert severity="warning">No documents uploaded</Alert>
                )}
              </Box>
            ) : null}
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setViewDialog({ open: false, vendor: null, loading: false })}>
              Close
            </Button>
            {viewDialog.vendor && viewDialog.vendor.status === 0 && (
              <>
                <Button 
                  color="success" 
                  variant="contained" 
                  startIcon={<CheckCircleOutline />}
                  onClick={() => {
                    handleApprove(viewDialog.vendor!.vendorId);
                    setViewDialog({ open: false, vendor: null, loading: false });
                  }}
                  disabled={loading.action}
                >
                  Approve
                </Button>
                <Button 
                  color="error" 
                  variant="outlined" 
                  startIcon={<ErrorOutline />}
                  onClick={() => {
                    const vendor = {
                      vendorId: viewDialog.vendor!.vendorId,
                      agencyName: viewDialog.vendor!.agencyName,
                      ownerName: viewDialog.vendor!.ownerName,
                      status: viewDialog.vendor!.status,
                      isActive: viewDialog.vendor!.isActive,
                      createdOn: viewDialog.vendor!.createdOn,
                      email: viewDialog.vendor!.email,
                      phone: viewDialog.vendor!.phone,
                      totalBuses: 0
                    };
                    handleRejectClick(vendor);
                    setViewDialog({ open: false, vendor: null, loading: false });
                  }}
                >
                  Reject
                </Button>
              </>
            )}
          </DialogActions>
        </Dialog>

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
      </div>
    </Layout>
  );
};

export default AdminDashboard;