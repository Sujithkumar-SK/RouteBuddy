import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  TextField,
  Button,
  MenuItem,
  Alert,
  CircularProgress,
} from '@mui/material';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { registerWithOtp, setTempEmail, setTempRole, setTempOtpToken } from './authSlice';
import { ROUTES } from '../../utils/constants';

const RECAPTCHA_SITE_KEY = '6Ldi0xIsAAAAAJCkvLZYqwRQaAmuNP3O7muuEivn';

const Signup = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { loading, error } = useAppSelector((state) => state.auth);
  const recaptchaRef = useRef<number | null>(null);
  const [recaptchaLoaded, setRecaptchaLoaded] = useState(false);

  const [formData, setFormData] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    role: '',
  });

  const [formErrors, setFormErrors] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    role: '',
  });
  const [recaptchaToken, setRecaptchaToken] = useState<string>('');

  useEffect(() => {
    const checkRecaptcha = () => {
      if (typeof window.grecaptcha !== 'undefined' && recaptchaRef.current === null) {
        window.grecaptcha.ready(() => {
          const container = document.getElementById('recaptcha-container');
          if (container && container.children.length === 0) {
            recaptchaRef.current = window.grecaptcha.render('recaptcha-container', {
              sitekey: RECAPTCHA_SITE_KEY,
              size: 'normal',
              callback: (token: string) => {
                setRecaptchaToken(token);
              },
              'expired-callback': () => {
                setRecaptchaToken('');
              },
            });
            setRecaptchaLoaded(true);
          }
        });
      } else if (typeof window.grecaptcha === 'undefined') {
        setTimeout(checkRecaptcha, 100);
      }
    };
    checkRecaptcha();
  }, []);

  const validateEmail = (email: string) => {
    if (!email) return 'Email is required';
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return 'Invalid email format';
    return '';
  };

  const validatePassword = (password: string) => {
    if (!password) return 'Password is required';
    if (password.length < 8) return 'Password must be at least 8 characters';
    if (!/(?=.*[a-z])/.test(password)) return 'Must contain at least one lowercase letter';
    if (!/(?=.*[A-Z])/.test(password)) return 'Must contain at least one uppercase letter';
    if (!/(?=.*\d)/.test(password)) return 'Must contain at least one number';
    return '';
  };

  const validatePhone = (phone: string) => {
    if (!phone) return 'Phone is required';
    if (!/^[6-9]\d{9}$/.test(phone)) return 'Invalid phone number (10 digits starting with 6-9)';
    return '';
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    let filteredValue = value;

    if (name === 'phone') {
      filteredValue = value.replace(/\D/g, '').slice(0, 10);
    }

    setFormData({ ...formData, [name]: filteredValue });

    let fieldError = '';
    switch (name) {
      case 'email':
        fieldError = validateEmail(filteredValue);
        break;
      case 'password':
        fieldError = validatePassword(filteredValue);
        if (formData.confirmPassword) {
          setFormErrors(prev => ({
            ...prev,
            confirmPassword: filteredValue !== formData.confirmPassword ? 'Passwords do not match' : ''
          }));
        }
        break;
      case 'confirmPassword':
        fieldError = filteredValue !== formData.password ? 'Passwords do not match' : '';
        break;
      case 'phone':
        fieldError = validatePhone(filteredValue);
        break;
    }

    setFormErrors({ ...formErrors, [name]: fieldError });
  };

  const isFormValid = () => {
    const hasNoErrors = Object.values(formErrors).every(error => error === '');
    const hasAllFields = formData.email && formData.password && formData.confirmPassword && formData.phone && formData.role;
    return hasNoErrors && hasAllFields;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!recaptchaToken) {
      alert('Please complete the reCAPTCHA verification');
      return;
    }

    try {
      const response = await dispatch(registerWithOtp({
        email: formData.email,
        password: formData.password,
        phone: formData.phone,
        role: parseInt(formData.role),
        recaptchaToken: recaptchaToken,
      })).unwrap();
      
      dispatch(setTempEmail(formData.email));
      dispatch(setTempRole(formData.role));
      dispatch(setTempOtpToken(response.otpToken));

      navigate(ROUTES.VERIFY_OTP, {
        state: {
          email: formData.email,
          otpToken: response.otpToken,
          role: parseInt(formData.role),
          formData: {
            email: formData.email,
            password: formData.password,
            phone: formData.phone,
            role: parseInt(formData.role),
            recaptchaToken: recaptchaToken,
          }
        }
      });
    } catch (err) {
      console.error('Signup failed:', err);
      if (recaptchaRef.current !== null && typeof window.grecaptcha !== 'undefined') {
        window.grecaptcha.reset(recaptchaRef.current);
        setRecaptchaToken('');
      }
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-card-content">
          <h1 className="auth-title">Sign Up</h1>
          <p className="auth-subtitle">Create your RouteBuddy account</p>

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
              label="Phone"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
              error={!!formErrors.phone}
              helperText={formErrors.phone}
              placeholder="9876543210"
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

            <TextField
              fullWidth
              label="Confirm Password"
              name="confirmPassword"
              type="password"
              value={formData.confirmPassword}
              onChange={handleChange}
              error={!!formErrors.confirmPassword}
              helperText={formErrors.confirmPassword}
            />

            <TextField
              select
              fullWidth
              label="I am a"
              value={formData.role}
              onChange={(e) => setFormData({ ...formData, role: e.target.value })}
              error={!!formErrors.role}
              helperText={formErrors.role}
            >
              <MenuItem value="1">Customer</MenuItem>
              <MenuItem value="2">Vendor</MenuItem>
            </TextField>

            <div className="recaptcha-wrapper">
              <div id="recaptcha-container"></div>
            </div>

            <Button
              fullWidth
              variant="contained"
              type="submit"
              disabled={loading || !recaptchaToken || !isFormValid()}
              className="auth-button"
            >
              {loading ? <CircularProgress size={24} /> : 'Send OTP'}
            </Button>

            <Button
              fullWidth
              variant="text"
              onClick={() => navigate(ROUTES.LOGIN)}
              className="auth-link-button"
            >
              Already have an account? Login
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Signup;
