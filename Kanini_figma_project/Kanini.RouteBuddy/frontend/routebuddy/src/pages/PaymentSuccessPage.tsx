import { useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  Box,
  Button,
  Card,
  CardContent,
} from '@mui/material';
import { CheckCircle, Download, Home } from '@mui/icons-material';
import Layout from '../components/layout/Layout';

const PaymentSuccessPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { paymentId, bookingId, amount } = location.state || {};

  useEffect(() => {
    if (!paymentId || !bookingId) {
      navigate('/');
    }
  }, [paymentId, bookingId, navigate]);

  const handleDownloadTicket = () => {
    // TODO: Implement ticket download
    alert('Ticket download will be implemented soon!');
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

          <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center', flexWrap: 'wrap' }}>
            <Button
              variant="contained"
              startIcon={<Download />}
              onClick={handleDownloadTicket}
              sx={{ minWidth: 150 }}
            >
              Download Ticket
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