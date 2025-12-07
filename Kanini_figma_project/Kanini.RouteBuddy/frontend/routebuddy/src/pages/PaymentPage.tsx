import { useEffect, useState } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  Box,
  Button,
  Alert,
  CircularProgress,
  Card,
  CardContent,
  Divider,
} from '@mui/material';
import { Timer, Payment, CheckCircle } from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { bookingAPI } from '../features/booking/bookingAPI';
import Layout from '../components/layout/Layout';

declare global {
  interface Window {
    Razorpay: any;
  }
}

const PaymentPage = () => {
  const { bookingId } = useParams<{ bookingId: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const dispatch = useAppDispatch();
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [bookingDetails, setBookingDetails] = useState<any>(null);
  const [timeLeft, setTimeLeft] = useState(600); // 10 minutes in seconds

  useEffect(() => {
    // Load Razorpay script
    const script = document.createElement('script');
    script.src = 'https://checkout.razorpay.com/v1/checkout.js';
    script.async = true;
    document.body.appendChild(script);

    return () => {
      document.body.removeChild(script);
    };
  }, []);

  useEffect(() => {
    // Countdown timer
    const timer = setInterval(() => {
      setTimeLeft((prev) => {
        if (prev <= 1) {
          clearInterval(timer);
          navigate('/search-results', { 
            state: { message: 'Booking expired. Please try again.' }
          });
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [navigate]);

  useEffect(() => {
    // Get booking details from location state
    if (location.state?.bookingDetails) {
      setBookingDetails(location.state.bookingDetails);
    }
  }, [location.state]);

  const formatTime = (seconds: number) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  };

  const handlePayment = async () => {
    if (!bookingDetails || !bookingId) return;

    setLoading(true);
    setError(null);

    try {
      // Initiate payment
      const paymentData = await bookingAPI.initiatePayment({
        bookingId: parseInt(bookingId),
        paymentMethod: 2, // UPI/Card
        notes: `Payment for booking ${bookingId}`
      });

      const options = {
        key: paymentData.key,
        amount: paymentData.amount * 100, // Convert to paise
        currency: 'INR',
        name: 'RouteBuddy',
        description: paymentData.description,
        order_id: paymentData.razorpayOrderId,
        prefill: {
          name: paymentData.prefillName,
          email: paymentData.prefillEmail,
          contact: paymentData.prefillContact,
        },
        theme: {
          color: '#1976d2',
        },
        handler: async (response: any) => {
          try {
            // Verify payment
            await bookingAPI.verifyPayment({
              razorpayPaymentId: response.razorpay_payment_id,
              razorpayOrderId: response.razorpay_order_id,
              razorpaySignature: response.razorpay_signature,
              bookingId: parseInt(bookingId),
            });

            // Navigate to success page
            navigate('/payment-success', {
              state: {
                paymentId: response.razorpay_payment_id,
                bookingId: bookingId,
                amount: paymentData.amount,
              }
            });
          } catch (error: any) {
            console.error('Payment verification error:', error);
            // Redirect to search results with error message
            navigate('/search-results', {
              state: { 
                message: 'Payment verification failed. Your booking may have expired. Please try booking again.',
                type: 'error'
              }
            });
          }
        },
        modal: {
          ondismiss: () => {
            setLoading(false);
            setError('Payment cancelled by user');
          },
        },
      };

      const razorpay = new window.Razorpay(options);
      razorpay.open();
    } catch (error: any) {
      setError(error.response?.data?.error || 'Payment initiation failed');
      setLoading(false);
    }
  };

  if (!bookingDetails) {
    return (
      <Layout>
        <Container maxWidth="md" sx={{ py: 4 }}>
          <Alert severity="error">
            Booking details not found. Please try booking again.
          </Alert>
        </Container>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="payment-container">
      <Container maxWidth="md" sx={{ py: 4 }}>
        {/* Timer Alert */}
        <Alert 
          severity="warning" 
          icon={<Timer />}
          sx={{ mb: 3 }}
        >
          <Typography variant="h6">
            Complete payment within: {formatTime(timeLeft)}
          </Typography>
          <Typography variant="body2">
            Your seats will be released if payment is not completed in time.
          </Typography>
        </Alert>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            {error}
          </Alert>
        )}

        {/* Booking Summary */}
        <Paper sx={{ p: 3, mb: 3 }}>
          <Typography variant="h5" sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1 }}>
            <Payment />
            Payment Details
          </Typography>

          <Card variant="outlined">
            <CardContent>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                <Typography variant="h6">Booking Summary</Typography>
                <Typography variant="h6" color="primary">
                  PNR: {bookingDetails.pnr}
                </Typography>
              </Box>

              <Divider sx={{ my: 2 }} />

              <Box sx={{ display: 'grid', gap: 1 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography>Route:</Typography>
                  <Typography>{bookingDetails.route}</Typography>
                </Box>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography>Travel Date:</Typography>
                  <Typography>{bookingDetails.travelDate}</Typography>
                </Box>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography>Seats:</Typography>
                  <Typography>{bookingDetails.seats?.join(', ')}</Typography>
                </Box>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography>Passengers:</Typography>
                  <Typography>{bookingDetails.passengerCount}</Typography>
                </Box>
              </Box>

              <Divider sx={{ my: 2 }} />

              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Typography variant="h6">Total Amount:</Typography>
                <Typography variant="h5" color="primary" sx={{ fontWeight: 600 }}>
                  ₹{bookingDetails.totalAmount}
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Paper>

        {/* Payment Actions */}
        <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center' }}>
          <Button
            variant="outlined"
            onClick={() => navigate('/search-results')}
            disabled={loading}
          >
            Cancel
          </Button>
          <Button
            variant="contained"
            size="large"
            onClick={handlePayment}
            disabled={loading || timeLeft <= 0}
            startIcon={loading ? <CircularProgress size={20} /> : <Payment />}
            sx={{ minWidth: 200 }}
          >
            {loading ? 'Processing...' : `Pay ₹${bookingDetails.totalAmount}`}
          </Button>
        </Box>

        {/* Security Info */}
        <Box sx={{ mt: 4, textAlign: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            <CheckCircle sx={{ verticalAlign: 'middle', mr: 1, fontSize: 16 }} />
            Secure payment powered by Razorpay
          </Typography>
        </Box>
      </Container>
      </div>
    </Layout>
  );
};

export default PaymentPage;