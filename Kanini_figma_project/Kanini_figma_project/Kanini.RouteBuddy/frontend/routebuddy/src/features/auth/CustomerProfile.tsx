import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  Typography,
  MenuItem,
  Alert,
  CircularProgress,
} from '@mui/material';
import { authAPI } from './authAPI';
import { ROUTES, Gender } from '../../utils/constants';

const CustomerProfile = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [formData, setFormData] = useState({
    firstName: '',
    middleName: '',
    lastName: '',
    dateOfBirth: '',
    gender: '',
  });

  const [formErrors, setFormErrors] = useState({
    firstName: '',
    middleName: '',
    lastName: '',
    dateOfBirth: '',
    gender: '',
  });

  const validateName = (name: string, fieldName: string, required = true) => {
    if (required && !name) return `${fieldName} is required`;
    if (name && !/^[A-Za-z\s]+$/.test(name)) return `${fieldName} should contain only letters`;
    if (name && name.length < 2) return `${fieldName} should be at least 2 characters`;
    return '';
  };

  const validateDateOfBirth = (date: string) => {
    if (!date) return 'Date of birth is required';
    const birthDate = new Date(date);
    const today = new Date();
    const age = today.getFullYear() - birthDate.getFullYear();
    if (age < 18) return 'You must be at least 18 years old';
    if (age > 100) return 'Invalid date of birth';
    return '';
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    let filteredValue = value;

    if (['firstName', 'middleName', 'lastName'].includes(name)) {
      filteredValue = value.replace(/[^A-Za-z\s]/g, '');
    }

    setFormData({ ...formData, [name]: filteredValue });

    let fieldError = '';
    switch (name) {
      case 'firstName':
        fieldError = validateName(filteredValue, 'First name', true);
        break;
      case 'middleName':
        fieldError = validateName(filteredValue, 'Middle name', false);
        break;
      case 'lastName':
        fieldError = validateName(filteredValue, 'Last name', true);
        break;
      case 'dateOfBirth':
        fieldError = validateDateOfBirth(filteredValue);
        break;
    }

    setFormErrors({ ...formErrors, [name]: fieldError });
  };

  const isFormValid = () => {
    const hasNoErrors = Object.values(formErrors).every(error => error === '');
    const hasAllFields = formData.firstName && formData.lastName && formData.dateOfBirth && formData.gender;
    return hasNoErrors && hasAllFields;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    const userId = sessionStorage.getItem('tempUserId');
    if (!userId) {
      setError('Session expired. Please register again.');
      setLoading(false);
      return;
    }

    try {
      await authAPI.completeCustomerProfile(parseInt(userId), {
        firstName: formData.firstName,
        middleName: formData.middleName || undefined,
        lastName: formData.lastName,
        dateOfBirth: formData.dateOfBirth,
        gender: parseInt(formData.gender),
      });

      sessionStorage.removeItem('tempUserId');
      navigate(ROUTES.LOGIN);
    } catch (err: any) {
      setError(err.response?.data?.error?.description || 'Failed to complete profile');
    } finally {
      setLoading(false);
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
      <Card sx={{ maxWidth: 500, width: '100%' }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h4" gutterBottom align="center">
            Complete Your Profile
          </Typography>
          <Typography variant="body2" color="text.secondary" align="center" sx={{ mb: 3 }}>
            Tell us a bit about yourself
          </Typography>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <form onSubmit={handleSubmit}>
            <TextField
              fullWidth
              label="First Name"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              error={!!formErrors.firstName}
              helperText={formErrors.firstName}
              required
              sx={{ mb: 2 }}
            />

            <TextField
              fullWidth
              label="Middle Name (Optional)"
              name="middleName"
              value={formData.middleName}
              onChange={handleChange}
              error={!!formErrors.middleName}
              helperText={formErrors.middleName}
              sx={{ mb: 2 }}
            />

            <TextField
              fullWidth
              label="Last Name"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              error={!!formErrors.lastName}
              helperText={formErrors.lastName}
              required
              sx={{ mb: 2 }}
            />

            <TextField
              fullWidth
              label="Date of Birth"
              name="dateOfBirth"
              type="date"
              value={formData.dateOfBirth}
              onChange={handleChange}
              error={!!formErrors.dateOfBirth}
              helperText={formErrors.dateOfBirth}
              InputLabelProps={{ shrink: true }}
              inputProps={{ max: new Date().toISOString().split('T')[0] }}
              required
              sx={{ mb: 2 }}
            />

            <TextField
              select
              fullWidth
              label="Gender"
              name="gender"
              value={formData.gender}
              onChange={(e) => {
                setFormData({ ...formData, gender: e.target.value });
                setFormErrors({ ...formErrors, gender: '' });
              }}
              error={!!formErrors.gender}
              helperText={formErrors.gender}
              required
              sx={{ mb: 3 }}
            >
              <MenuItem value={Gender.Male}>Male</MenuItem>
              <MenuItem value={Gender.Female}>Female</MenuItem>
              <MenuItem value={Gender.Other}>Other</MenuItem>
            </TextField>

            <Button
              fullWidth
              variant="contained"
              type="submit"
              disabled={loading || !isFormValid()}
              sx={{ py: 1.5 }}
            >
              {loading ? <CircularProgress size={24} /> : 'Complete Registration'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
};

export default CustomerProfile;
