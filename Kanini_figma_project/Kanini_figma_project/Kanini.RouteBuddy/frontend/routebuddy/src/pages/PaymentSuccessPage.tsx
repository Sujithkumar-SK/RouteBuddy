import { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  Box,
  Button,
  Card,
  CardContent,
  Alert,
  CircularProgress,
} from '@mui/material';
import { CheckCircle, Download, Home, BookmarkBorder } from '@mui/icons-material';
import Layout from '../components/layout/Layout';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchCustomerProfile } from '../features/auth/customerProfileSlice';
import { downloadTicket } from '../features/auth/customerBookingsSlice';

const PaymentSuccessPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const dispatch = useAppDispatch();
  const { paymentId, bookingId, amount } = location.state || {};
  const { user } = useAppSelector((state) => state.auth);
  const { profile } = useAppSelector((state) => state.customerProfile);
  const { loading: downloadLoading, error: downloadError } = useAppSelector((state) => state.customerBookings);
  
  const [ticketError, setTicketError] = useState<string | null>(null);

  useEffect(() => {
    if (!paymentId || !bookingId) {
      navigate('/');
    }
  }, [paymentId, bookingId, navigate]);

  useEffect(() => {
    // Fetch customer profile to get customerId
    if (user && !profile) {
      dispatch(fetchCustomerProfile());
    }
  }, [dispatch, user, profile]);

  const handleDownloadTicket = async () => {
    if (!profile?.customerId || !bookingId) {
      setTicketError('Unable to download ticket. Missing customer or booking information.');
      return;
    }

    try {
      setTicketError(null);
      await dispatch(downloadTicket({ 
        customerId: profile.customerId, 
        bookingId: parseInt(bookingId.toString()) 
      })).unwrap();
    } catch (error: any) {
      setTicketError(error || 'Failed to download ticket. Please try again.');
    }
  };

  return (
    <Layout>
      <Container maxWidth="md" sx={{ py: 4 }}>
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <CheckCircle 
            sx={{ 
              fontSize: 80, 
              color: 'success.main', 
              mb: 2 
            }} 
          />
          
          <Typography variant="h4" sx={{ mb: 2, color: 'success.main', fontWeight: 600 }}>
            Payment Successful!
          </Typography>
          
          <Typography variant="h6" sx={{ mb: 4, color: 'text.secondary' }}>
            Your booking has been confirmed
          </Typography>

          <Card variant="outlined" sx={{ mb: 4, maxWidth: 400, mx: 'auto' }}>
            <CardContent>
              <Typography variant="h6" sx={{ mb: 2 }}>
                Payment Details
              </Typography>
              
              <Box sx={{ display: 'grid', gap: 1, textAlign: 'left' }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography>Payment ID:</Typography>
                  <Typography sx={{ fontFamily: 'monospace', fontSize: '0.9em' }}>
                    {paymentId}
                  </Typography>
                </Box>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography>Booking ID:</Typography>
                  <Typography sx={{ fontWeight: 600 }}>
                    {bookingId}
                  </Typography>
                </Box>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography>Amount Paid:</Typography>
                  <Typography sx={{ fontWeight: 600, color: 'success.main' }}>
                    ₹{amount}
                  </Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>

          {/* Error Alert */}
          {(ticketError || downloadError) && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {ticketError || downloadError}
            </Alert>
          )}

          <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center', flexWrap: 'wrap' }}>
            <Button
              variant="contained"
              startIcon={downloadLoading ? <CircularProgress size={16} color="inherit" /> : <Download />}
              onClick={handleDownloadTicket}
              disabled={downloadLoading || !profile?.customerId}
              sx={{ minWidth: 150 }}
            >
              {downloadLoading ? 'Downloading...' : 'Download Ticket'}
            </Button>
            
            <Button
              variant="outlined"
              startIcon={<BookmarkBorder />}
              onClick={() => navigate('/my-bookings')}
              sx={{ minWidth: 150 }}
            >
              My Bookings
            </Button>
            
            <Button
              variant="outlined"
              startIcon={<Home />}
              onClick={() => navigate('/')}
              sx={{ minWidth: 150 }}
            >
              Go to Home
            </Button>
          </Box>

          <Typography variant="body2" sx={{ mt: 4, color: 'text.secondary' }}>
            A confirmation email has been sent to your registered email address.
            <br />
            Please carry a valid ID proof while traveling.
          </Typography>
        </Paper>
      </Container>
    </Layout>
  );
};

export default PaymentSuccessPage;