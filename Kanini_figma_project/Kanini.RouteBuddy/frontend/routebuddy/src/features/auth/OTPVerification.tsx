import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
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
import { verifyRegistrationOtp, resendRegistrationOtp, setTempUserId, setTempOtpToken } from './authSlice';
import { ROUTES } from '../../utils/constants';

interface LocationState {
  email: string;
  otpToken: string;
  role: number;
  formData: {
    email: string;
    password: string;
    phone: string;
    role: number;
    recaptchaToken: string;
  };
}

const OTPVerification = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const dispatch = useAppDispatch();
  const { loading, error } = useAppSelector((state) => state.auth);
  const { email, otpToken, role, formData } = (location.state as LocationState) || {};

  const [otp, setOtp] = useState('');
  const [resendTimer, setResendTimer] = useState(120);
  const [canResend, setCanResend] = useState(false);
  const [resendLoading, setResendLoading] = useState(false);

  useEffect(() => {
    if (!email || !otpToken) {
      navigate(ROUTES.SIGNUP);
      return;
    }

    const timer = setInterval(() => {
      setResendTimer((prev) => {
        if (prev <= 1) {
          setCanResend(true);
          clearInterval(timer);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [email, otpToken, navigate]);

  const handleVerify = async () => {
    if (!email || !otpToken || otp.length !== 6) return;

    try {
      const response = await dispatch(verifyRegistrationOtp({ 
        email, 
        otp,
        otpToken,
        role
      })).unwrap();
      
      // Store userId for profile completion
      dispatch(setTempUserId(response.userId));
      
      // After successful OTP verification, redirect to profile completion
      if (response.requiresVendorProfile) {
        navigate(ROUTES.VENDOR_PROFILE);
      } else {
        navigate(ROUTES.CUSTOMER_PROFILE);
      }
    } catch (err) {
      console.error('OTP verification failed:', err);
    }
  };

  const handleResend = async () => {
    if (!canResend || !formData) return;
    
    setResendLoading(true);

    try {
      const response = await dispatch(resendRegistrationOtp({
        email: formData.email,
        password: formData.password,
        phone: formData.phone,
        role: formData.role,
      })).unwrap();
      
      location.state.otpToken = response.otpToken;
      dispatch(setTempOtpToken(response.otpToken));
      setResendTimer(120);
      setCanResend(false);
      setOtp('');
      
      const timer = setInterval(() => {
        setResendTimer((prev) => {
          if (prev <= 1) {
            setCanResend(true);
            clearInterval(timer);
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    } catch (err) {
      console.error('Resend OTP failed:', err);
    } finally {
      setResendLoading(false);
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
            Verify Email
          </Typography>
          <Typography variant="body2" color="text.secondary" align="center" sx={{ mb: 3 }}>
            Enter the 6-digit code sent to {email}
          </Typography>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <TextField
            fullWidth
            label="OTP Code"
            value={otp}
            onChange={(e) => setOtp(e.target.value.replace(/\D/g, '').slice(0, 6))}
            inputProps={{ maxLength: 6, style: { textAlign: 'center', fontSize: '24px', letterSpacing: '8px' } }}
            sx={{ mb: 3 }}
          />

          <Button
            fullWidth
            variant="contained"
            onClick={handleVerify}
            disabled={loading || otp.length !== 6}
            sx={{ mb: 2, py: 1.5 }}
          >
            {loading ? <CircularProgress size={24} /> : 'Verify OTP'}
          </Button>

          <Button
            fullWidth
            variant="text"
            onClick={handleResend}
            disabled={!canResend || loading || resendLoading}
          >
            {resendLoading ? <CircularProgress size={20} /> : canResend ? 'Resend OTP' : `Resend in ${resendTimer}s`}
          </Button>
        </CardContent>
      </Card>
    </Box>
  );
};

export default OTPVerification;
