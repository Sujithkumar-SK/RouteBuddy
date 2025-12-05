import { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Typography,
  Box,
  Alert,
  Divider
} from '@mui/material';
import { Warning, Cancel } from '@mui/icons-material';
import type { CustomerBooking } from './customerBookingsAPI';

interface CancelBookingDialogProps {
  open: boolean;
  booking: CustomerBooking | null;
  onClose: () => void;
  onConfirm: (bookingId: number, reason: string) => void;
  loading: boolean;
}

const CancelBookingDialog = ({ open, booking, onClose, onConfirm, loading }: CancelBookingDialogProps) => {
  const [reason, setReason] = useState('');
  const [error, setError] = useState('');

  const handleClose = () => {
    setReason('');
    setError('');
    onClose();
  };

  const handleConfirm = () => {
    if (!reason.trim()) {
      setError('Cancellation reason is required');
      return;
    }
    if (reason.trim().length < 10) {
      setError('Reason must be at least 10 characters');
      return;
    }
    if (reason.trim().length > 250) {
      setError('Reason cannot exceed 250 characters');
      return;
    }

    if (booking) {
      onConfirm(booking.bookingId, reason.trim());
    }
  };

  const calculateHoursUntilTravel = () => {
    if (!booking) return 0;
    const travelDate = new Date(booking.travelDate);
    const now = new Date();
    return Math.floor((travelDate.getTime() - now.getTime()) / (1000 * 60 * 60));
  };

  const getPenaltyInfo = () => {
    const hours = calculateHoursUntilTravel();
    if (hours <= 2) return { penalty: '100%', refund: '0%', allowed: false };
    if (hours <= 6) return { penalty: '75%', refund: '25%', allowed: true };
    if (hours <= 12) return { penalty: '50%', refund: '50%', allowed: true };
    if (hours <= 24) return { penalty: '25%', refund: '75%', allowed: true };
    return { penalty: '10%', refund: '90%', allowed: true };
  };

  const penaltyInfo = getPenaltyInfo();

  if (!booking) return null;

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <Cancel color="error" />
        Cancel Booking
      </DialogTitle>

      <DialogContent>
        <Box sx={{ mb: 2 }}>
          <Typography variant="h6" gutterBottom>
            Booking Details
          </Typography>
          <Typography variant="body2" color="text.secondary">
            PNR: {booking.pnrNo}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Route: {booking.route}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Travel Date: {new Date(booking.travelDate).toLocaleDateString()}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Amount: ₹{booking.totalAmount}
          </Typography>
        </Box>

        <Divider sx={{ my: 2 }} />

        {!penaltyInfo.allowed ? (
          <Alert severity="error" sx={{ mb: 2 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <Warning />
              <Typography variant="body2">
                Cancellation not allowed within 2 hours of travel time
              </Typography>
            </Box>
          </Alert>
        ) : (
          <>
            <Alert severity="warning" sx={{ mb: 2 }}>
              <Typography variant="body2" sx={{ fontWeight: 600 }}>
                Cancellation Policy
              </Typography>
              <Typography variant="body2">
                Penalty: {penaltyInfo.penalty} | Refund: {penaltyInfo.refund}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Hours until travel: {calculateHoursUntilTravel()}
              </Typography>
            </Alert>

            <TextField
              fullWidth
              multiline
              rows={4}
              label="Cancellation Reason"
              placeholder="Please provide a reason for cancellation (minimum 10 characters)"
              value={reason}
              onChange={(e) => {
                setReason(e.target.value);
                setError('');
              }}
              error={!!error}
              helperText={error || `${reason.length}/250 characters`}
              sx={{ mb: 2 }}
            />
          </>
        )}
      </DialogContent>

      <DialogActions>
        <Button onClick={handleClose} disabled={loading}>
          Cancel
        </Button>
        {penaltyInfo.allowed && (
          <Button
            onClick={handleConfirm}
            variant="contained"
            color="error"
            disabled={loading || !reason.trim() || reason.length < 10}
          >
            {loading ? 'Cancelling...' : 'Confirm Cancellation'}
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
};

export default CancelBookingDialog;