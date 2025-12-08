import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  TextField,
  Button,
  Alert,
  CircularProgress,
} from '@mui/material';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { login } from './authSlice';
import { ROUTES } from '../../utils/constants';

const Login = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const dispatch = useAppDispatch();
  const { loading, error } = useAppSelector((state) => state.auth);

  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });
  const [successMessage, setSuccessMessage] = useState('');

  useEffect(() => {
    if (location.state?.message) {
      setSuccessMessage(location.state.message);
      // Clear the message from location state
      window.history.replaceState({}, document.title);
    }
  }, [location]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const result = await dispatch(login(formData)).unwrap();
      
      // Small delay to ensure Redux state is updated
      setTimeout(() => {
        switch (result.role) {
          case 'Customer':
            navigate(ROUTES.CUSTOMER_DASHBOARD);
            break;
          case 'Vendor':
            navigate(ROUTES.VENDOR_DASHBOARD);
            break;
          case 'Admin':
            navigate(ROUTES.ADMIN_DASHBOARD);
            break;
          default:
            navigate(ROUTES.HOME);
        }
      }, 100);
    } catch (err) {
      console.error('Login failed:', err);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-card-content">
          <h1 className="auth-title">Login</h1>
          <p className="auth-subtitle">Welcome back to RouteBuddy</p>

          {successMessage && (
            <Alert severity="success" sx={{ mb: 2 }}>
              {successMessage}
            </Alert>
          )}

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
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              required
            />

            <TextField
              fullWidth
              label="Password"
              type="password"
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })}
              required
            />

            <Button
              fullWidth
              variant="contained"
              type="submit"
              disabled={loading}
              className="auth-button"
            >
              {loading ? <CircularProgress size={24} /> : 'Login'}
            </Button>

            <Button
              fullWidth
              variant="text"
              onClick={() => navigate(ROUTES.FORGOT_PASSWORD)}
              className="auth-link-button"
            >
              Forgot Password?
            </Button>

            <Button
              fullWidth
              variant="text"
              onClick={() => navigate(ROUTES.SIGNUP)}
              className="auth-link-button"
            >
              Don't have an account? Sign Up
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Login;
