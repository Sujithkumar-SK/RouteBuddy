import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  TextField,
  Button,
  Alert,
  CircularProgress,
} from '@mui/material';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { forgotPassword, setTempEmail, setTempOtpToken } from './authSlice';
import { ROUTES } from '../../utils/constants';

const ForgotPassword = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { loading, error } = useAppSelector((state) => state.auth);

  const [email, setEmail] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const result = await dispatch(forgotPassword({ email })).unwrap();
      
      // Store email and OTP token for next step
      dispatch(setTempEmail(email));
      dispatch(setTempOtpToken(result.otpToken));
      
      // Navigate to OTP verification
      navigate(ROUTES.VERIFY_FORGOT_OTP);
    } catch (err) {
      console.error('Forgot password failed:', err);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-card-content">
          <h1 className="auth-title">Forgot Password</h1>
          <p className="auth-subtitle">Enter your email address and we'll send you an OTP to reset your password</p>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <form onSubmit={handleSubmit} className="auth-form">
            <TextField
              fullWidth
              label="Email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />

            <Button
              fullWidth
              variant="contained"
              type="submit"
              disabled={loading}
              className="auth-button"
            >
              {loading ? <CircularProgress size={24} /> : 'Send Reset OTP'}
            </Button>

            <Button
              fullWidth
              variant="text"
              onClick={() => navigate(ROUTES.LOGIN)}
              className="auth-link-button"
            >
              Back to Login
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default ForgotPassword;