import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  TextField,
  Button,
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
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-card-content">
          <h1 className="auth-title">Verify Email</h1>
          <p className="auth-subtitle">Enter the 6-digit code sent to {email}</p>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <div className="auth-form">
            <TextField
              fullWidth
              label="OTP Code"
              value={otp}
              onChange={(e) => setOtp(e.target.value.replace(/\D/g, '').slice(0, 6))}
              inputProps={{ maxLength: 6 }}
              className="otp-input"
            />

            <Button
              fullWidth
              variant="contained"
              onClick={handleVerify}
              disabled={loading || otp.length !== 6}
              className="auth-button"
            >
              {loading ? <CircularProgress size={24} /> : 'Verify OTP'}
            </Button>

            <Button
              fullWidth
              variant="text"
              onClick={handleResend}
              disabled={!canResend || loading || resendLoading}
              className="auth-link-button"
            >
              {resendLoading ? <CircularProgress size={20} /> : canResend ? 'Resend OTP' : <span className="timer-text">Resend in {resendTimer}s</span>}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OTPVerification;
