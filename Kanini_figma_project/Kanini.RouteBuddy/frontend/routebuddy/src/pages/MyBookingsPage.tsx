import { useEffect, useState } from 'react';
import {
  Container,
  Typography,
  Box,
  Alert,
  Skeleton,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Button,
  Grid,
  Paper,
  Snackbar
} from '@mui/material';
import { BookmarkBorder, FilterList } from '@mui/icons-material';
import Layout from '../components/layout/Layout';
import BookingCard from '../features/auth/BookingCard';
import CancelBookingDialog from '../features/auth/CancelBookingDialog';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchCustomerBookings, downloadTicket, cancelBooking, setFilters, clearError } from '../features/auth/customerBookingsSlice';
import { checkAuthState } from '../features/auth/authSlice';
import type { CustomerBooking } from '../features/auth/customerBookingsAPI';

const MyBookingsPage = () => {
  const dispatch = useAppDispatch();
  const { user } = useAppSelector((state) => state.auth);
  const { bookings, loading, error, filters } = useAppSelector((state) => state.customerBookings);

  const [localFilters, setLocalFilters] = useState({
    status: '',
    fromDate: '',
    toDate: '',
  });

  const [cancelDialog, setCancelDialog] = useState<{
    open: boolean;
    booking: CustomerBooking | null;
  }>({ open: false, booking: null });

  const [snackbar, setSnackbar] = useState<{
    open: boolean;
    message: string;
    severity: 'success' | 'error';
  }>({ open: false, message: '', severity: 'success' });

  useEffect(() => {
    // Check auth state first
    dispatch(checkAuthState());
  }, [dispatch]);

  useEffect(() => {
    if (user?.userId) {
      dispatch(fetchCustomerBookings({ 
        ...filters 
      }));
    }
    return () => {
      dispatch(clearError());
    };
  }, [dispatch, user?.userId, filters]);

  const handleFilterChange = (field: string, value: string) => {
    setLocalFilters(prev => ({ ...prev, [field]: value }));
  };

  const applyFilters = () => {
    const filterParams: any = {};
    
    if (localFilters.status) filterParams.status = parseInt(localFilters.status);
    if (localFilters.fromDate) filterParams.fromDate = localFilters.fromDate;
    if (localFilters.toDate) filterParams.toDate = localFilters.toDate;

    dispatch(setFilters(filterParams));
    dispatch(fetchCustomerBookings(filterParams));
  };

  const clearFilters = () => {
    setLocalFilters({ status: '', fromDate: '', toDate: '' });
    dispatch(setFilters({}));
    dispatch(fetchCustomerBookings({}));
  };

  const handleDownloadTicket = (bookingId: number) => {
    if (user?.userId) {
      dispatch(downloadTicket({ customerId: user.userId, bookingId }));
    }
  };

  const handleCancelBooking = (booking: CustomerBooking) => {
    setCancelDialog({ open: true, booking });
  };

  const handleCancelConfirm = async (bookingId: number, reason: string) => {
    if (user?.userId) {
      try {
        const result = await dispatch(cancelBooking({ 
          customerId: user.userId, 
          request: { bookingId, reason } 
        })).unwrap();
        
        setCancelDialog({ open: false, booking: null });
        setSnackbar({ 
          open: true, 
          message: `Booking cancelled successfully. Refund: ₹${result.response.refundAmount}`, 
          severity: 'success' 
        });
        
        // Refresh bookings
        dispatch(fetchCustomerBookings({ ...filters }));
      } catch (error: any) {
        setSnackbar({ 
          open: true, 
          message: error || 'Failed to cancel booking', 
          severity: 'error' 
        });
      }
    }
  };

  const handleCancelDialogClose = () => {
    setCancelDialog({ open: false, booking: null });
  };

  const handleSnackbarClose = () => {
    setSnackbar({ ...snackbar, open: false });
  };

  if (!user) {
    return (
      <Layout>
        <Container maxWidth="md" sx={{ py: 4 }}>
          <Alert severity="error">
            Please log in to view your bookings
          </Alert>
        </Container>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="bookings-container">
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <div className="bookings-header">
          <BookmarkBorder style={{ fontSize: '2.5rem', color: '#667eea' }} />
          <h1 className="bookings-title">My Bookings</h1>
        </div>

        {/* Filters */}
        <div className="bookings-filters-paper">
          <div className="bookings-filters-header">
            <FilterList style={{ color: '#666' }} />
            <h3 className="bookings-filters-title">Filters</h3>
          </div>
          
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6} md={3}>
              <FormControl fullWidth size="small">
                <InputLabel>Status</InputLabel>
                <Select
                  value={localFilters.status}
                  onChange={(e) => handleFilterChange('status', e.target.value)}
                  label="Status"
                >
                  <MenuItem value="">All</MenuItem>
                  <MenuItem value="1">Pending</MenuItem>
                  <MenuItem value="2">Confirmed</MenuItem>
                  <MenuItem value="3">Cancelled</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6} md={3}>
              <TextField
                fullWidth
                size="small"
                label="From Date"
                type="date"
                value={localFilters.fromDate}
                onChange={(e) => handleFilterChange('fromDate', e.target.value)}
                InputLabelProps={{ shrink: true }}
              />
            </Grid>

            <Grid item xs={12} sm={6} md={3}>
              <TextField
                fullWidth
                size="small"
                label="To Date"
                type="date"
                value={localFilters.toDate}
                onChange={(e) => handleFilterChange('toDate', e.target.value)}
                InputLabelProps={{ shrink: true }}
              />
            </Grid>

            <Grid item xs={12} sm={6} md={3}>
              <Box sx={{ display: 'flex', gap: 1 }}>
                <Button variant="contained" onClick={applyFilters} size="small">
                  Apply
                </Button>
                <Button variant="outlined" onClick={clearFilters} size="small">
                  Clear
                </Button>
              </Box>
            </Grid>
          </Grid>
        </div>

        {/* Error Alert */}
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {/* Loading State */}
        {loading && (
          <Box>
            {[1, 2, 3].map((item) => (
              <Skeleton key={item} variant="rectangular" height={200} sx={{ mb: 2, borderRadius: 1 }} />
            ))}
          </Box>
        )}

        {/* Bookings List */}
        {!loading && bookings.length === 0 && (
          <Paper sx={{ p: 4, textAlign: 'center' }}>
            <BookmarkBorder sx={{ fontSize: 64, color: 'grey.400', mb: 2 }} />
            <Typography variant="h6" color="text.secondary">
              No bookings found
            </Typography>
            <Typography variant="body2" color="text.secondary">
              You haven't made any bookings yet
            </Typography>
          </Paper>
        )}

        {!loading && bookings.length > 0 && (
          <Box>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              Found {bookings.length} booking(s)
            </Typography>
            {bookings.map((booking) => (
              <BookingCard
                key={booking.bookingId}
                booking={booking}
                onDownloadTicket={handleDownloadTicket}
                onCancelBooking={handleCancelBooking}
              />
            ))}
          </Box>
        )}

        {/* Cancel Booking Dialog */}
        <CancelBookingDialog
          open={cancelDialog.open}
          booking={cancelDialog.booking}
          onClose={handleCancelDialogClose}
          onConfirm={handleCancelConfirm}
          loading={loading}
        />

        {/* Success/Error Snackbar */}
        <Snackbar
          open={snackbar.open}
          autoHideDuration={6000}
          onClose={handleSnackbarClose}
        >
          <Alert onClose={handleSnackbarClose} severity={snackbar.severity}>
            {snackbar.message}
          </Alert>
        </Snackbar>
      </Container>
      </div>
    </Layout>
  );
};

export default MyBookingsPage;