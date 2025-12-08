import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  Typography,
  Alert,
  CircularProgress,
} from '@mui/material';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { verifyForgotPasswordOtp, forgotPassword } from './authSlice';
import { ROUTES } from '../../utils/constants';

const VerifyForgotOtp = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { loading, error, tempEmail, tempOtpToken } = useAppSelector((state) => state.auth);

  const [otp, setOtp] = useState('');
  const [countdown, setCountdown] = useState(120);
  const [canResend, setCanResend] = useState(false);

  useEffect(() => {
    if (!tempEmail) {
      navigate(ROUTES.FORGOT_PASSWORD);
      return;
    }

    const timer = setInterval(() => {
      setCountdown((prev) => {
        if (prev <= 1) {
          setCanResend(true);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [tempEmail, navigate]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!tempEmail || !tempOtpToken) {
      navigate(ROUTES.FORGOT_PASSWORD);
      return;
    }

    try {
      await dispatch(verifyForgotPasswordOtp({
        email: tempEmail,
        otp,
        otpToken: tempOtpToken,
      })).unwrap();
      
      // Navigate to reset password
      navigate(ROUTES.RESET_PASSWORD);
    } catch (err) {
      console.error('OTP verification failed:', err);
    }
  };

  const handleResendOtp = async () => {
    if (!tempEmail) return;

    try {
      await dispatch(forgotPassword({ email: tempEmail })).unwrap();
      setCountdown(120);
      setCanResend(false);
      setOtp('');
    } catch (err) {
      console.error('Resend OTP failed:', err);
    }
  };

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        bgcolor: '#f5f5f5',
        p: 2,
      }}
    >
      <Card sx={{ maxWidth: 400, width: '100%' }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h4" gutterBottom align="center">
            Verify OTP
          </Typography>
          <Typography variant="body2" color="text.secondary" align="center" sx={{ mb: 3 }}>
            Enter the 6-digit OTP sent to {tempEmail}
          </Typography>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <form onSubmit={handleSubmit}>
            <TextField
              fullWidth
              label="Enter OTP"
              value={otp}
              onChange={(e) => setOtp(e.target.value.replace(/\D/g, '').slice(0, 6))}
              required
              inputProps={{ maxLength: 6, pattern: '[0-9]{6}' }}
              sx={{ mb: 3 }}
            />

            <Button
              fullWidth
              variant="contained"
              type="submit"
              disabled={loading || otp.length !== 6}
              sx={{ mb: 2, py: 1.5 }}
            >
              {loading ? <CircularProgress size={24} /> : 'Verify OTP'}
            </Button>

            <Button
              fullWidth
              variant="outlined"
              onClick={handleResendOtp}
              disabled={!canResend || loading}
              sx={{ mb: 2 }}
            >
              {canResend ? 'Resend OTP' : `Resend in ${countdown}s`}
            </Button>

            <Button
              fullWidth
              variant="text"
              onClick={() => navigate(ROUTES.FORGOT_PASSWORD)}
            >
              Back
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
};

export default VerifyForgotOtp;