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
  const [formErrors, setFormErrors] = useState({
    email: '',
    password: '',
  });
  const [successMessage, setSuccessMessage] = useState('');

  useEffect(() => {
    if (location.state?.message) {
      setSuccessMessage(location.state.message);
      window.history.replaceState({}, document.title);
    }
  }, [location]);

  const validateEmail = (email: string) => {
    if (!email) return 'Email is required';
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return 'Invalid email format';
    return '';
  };

  const validatePassword = (password: string) => {
    if (!password) return 'Password is required';
    if (password.length < 8) return 'Password must be at least 8 characters';
    return '';
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });

    let fieldError = '';
    if (name === 'email') {
      fieldError = validateEmail(value);
    } else if (name === 'password') {
      fieldError = validatePassword(value);
    }

    setFormErrors({ ...formErrors, [name]: fieldError });
  };

  const isFormValid = () => {
    const hasNoErrors = Object.values(formErrors).every(error => error === '');
    const hasAllFields = formData.email && formData.password;
    return hasNoErrors && hasAllFields;
  };

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
              name="email"
              type="email"
              value={formData.email}
              onChange={handleChange}
              error={!!formErrors.email}
              helperText={formErrors.email}
            />

            <TextField
              fullWidth
              label="Password"
              name="password"
              type="password"
              value={formData.password}
              onChange={handleChange}
              error={!!formErrors.password}
              helperText={formErrors.password}
            />

            <Button
              fullWidth
              variant="contained"
              type="submit"
              disabled={loading || !isFormValid()}
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
