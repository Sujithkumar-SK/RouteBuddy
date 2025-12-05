import {
  Card,
  CardContent,
  Typography,
  Box,
  Chip,
  Button,
  Grid,
  Divider
} from '@mui/material';
import {
  DirectionsBus,
  Schedule,
  Person,
  Payment,
  Download,
  LocationOn,
  Cancel
} from '@mui/icons-material';
import type { CustomerBooking } from './customerBookingsAPI';

interface BookingCardProps {
  booking: CustomerBooking;
  onDownloadTicket: (bookingId: number) => void;
  onCancelBooking: (booking: CustomerBooking) => void;
}

const BookingCard = ({ booking, onDownloadTicket, onCancelBooking }: BookingCardProps) => {
  const getStatusColor = (status: number) => {
    switch (status) {
      case 1: return 'warning'; // Pending
      case 2: return 'success'; // Confirmed
      case 3: return 'error';   // Cancelled
      default: return 'default';
    }
  };

  const getStatusText = (status: number) => {
    switch (status) {
      case 1: return 'Pending';
      case 2: return 'Confirmed';
      case 3: return 'Cancelled';
      default: return 'Unknown';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-IN', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  };

  const formatTime = (timeString: string) => {
    return timeString.substring(0, 5);
  };

  return (
    <Card sx={{ mb: 2, '&:hover': { boxShadow: 3 } }}>
      <CardContent>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
          <Box>
            <Typography variant="h6" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <DirectionsBus color="primary" />
              {booking.busName}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              PNR: {booking.pnrNo}
            </Typography>
          </Box>
          <Chip 
            label={getStatusText(booking.status)} 
            color={getStatusColor(booking.status)}
            size="small"
          />
        </Box>

        <Grid container spacing={2}>
          <Grid item xs={12} sm={6}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
              <LocationOn fontSize="small" color="action" />
              <Typography variant="body2">{booking.route}</Typography>
            </Box>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
              <Schedule fontSize="small" color="action" />
              <Typography variant="body2">
                {formatTime(booking.departureTime)} - {formatTime(booking.arrivalTime)}
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
              <Person fontSize="small" color="action" />
              <Typography variant="body2">{booking.totalSeats} Seat(s)</Typography>
            </Box>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
              <Payment fontSize="small" color="action" />
              <Typography variant="body2">₹{booking.totalAmount}</Typography>
            </Box>
          </Grid>
        </Grid>

        <Divider sx={{ my: 2 }} />

        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Box>
            <Typography variant="caption" color="text.secondary">
              Travel Date: {formatDate(booking.travelDate)}
            </Typography>
            <br />
            <Typography variant="caption" color="text.secondary">
              Booked: {formatDate(booking.bookedAt)}
            </Typography>
          </Box>

          <Box sx={{ display: 'flex', gap: 1 }}>
            {booking.status === 2 && ( // Only show download for confirmed bookings
              <Button
                variant="outlined"
                size="small"
                startIcon={<Download />}
                onClick={() => onDownloadTicket(booking.bookingId)}
              >
                Download Ticket
              </Button>
            )}
            
            {(booking.status === 1 || booking.status === 2) && ( // Show cancel for pending and confirmed bookings
              <Button
                variant="outlined"
                color="error"
                size="small"
                startIcon={<Cancel />}
                onClick={() => onCancelBooking(booking)}
              >
                Cancel
              </Button>
            )}
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
};

export default BookingCard;