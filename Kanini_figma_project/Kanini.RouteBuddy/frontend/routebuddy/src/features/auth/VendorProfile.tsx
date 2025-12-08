import { useState } from 'react';
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
import { authAPI } from './authAPI';
import { ROUTES } from '../../utils/constants';

const VendorProfile = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [formData, setFormData] = useState({
    agencyName: '',
    ownerName: '',
    businessLicenseNumber: '',
    officeAddress: '',
    fleetSize: '',
    taxRegistrationNumber: '',
  });

  const [formErrors, setFormErrors] = useState({
    agencyName: '',
    ownerName: '',
    businessLicenseNumber: '',
    officeAddress: '',
    fleetSize: '',
    taxRegistrationNumber: '',
    businessLicense: '',
  });

  const [documents, setDocuments] = useState<{
    businessLicense: File | null;
    taxRegistration: File | null;
    ownerIdentity: File | null;
  }>({
    businessLicense: null,
    taxRegistration: null,
    ownerIdentity: null,
  });

  const validateName = (name: string, fieldName: string) => {
    if (!name) return `${fieldName} is required`;
    if (name.length < 2) return `${fieldName} should be at least 2 characters`;
    return '';
  };

  const validateLicenseNumber = (license: string) => {
    if (!license) return 'Business license number is required';
    if (license.length < 5) return 'Invalid license number';
    return '';
  };

  const validateFleetSize = (size: string) => {
    if (!size) return 'Fleet size is required';
    const num = parseInt(size);
    if (num < 1) return 'Fleet size must be at least 1';
    if (num > 1000) return 'Fleet size seems too large';
    return '';
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    let filteredValue = value;

    if (name === 'ownerName') {
      filteredValue = value.replace(/[^A-Za-z\s]/g, '');
    }

    if (name === 'fleetSize') {
      filteredValue = value.replace(/\D/g, '');
    }

    setFormData({ ...formData, [name]: filteredValue });

    let fieldError = '';
    switch (name) {
      case 'agencyName':
        fieldError = validateName(filteredValue, 'Agency name');
        break;
      case 'ownerName':
        fieldError = validateName(filteredValue, 'Owner name');
        break;
      case 'businessLicenseNumber':
        fieldError = validateLicenseNumber(filteredValue);
        break;
      case 'officeAddress':
        fieldError = !filteredValue ? 'Office address is required' : '';
        break;
      case 'fleetSize':
        fieldError = validateFleetSize(filteredValue);
        break;
    }

    setFormErrors({ ...formErrors, [name]: fieldError });
  };

  const handleFileChange = (field: 'businessLicense' | 'taxRegistration' | 'ownerIdentity', file: File | null) => {
    setDocuments({ ...documents, [field]: file });
    if (field === 'businessLicense') {
      setFormErrors({ ...formErrors, businessLicense: file ? '' : 'Business license document is required' });
    }
  };

  const isFormValid = () => {
    const hasNoErrors = Object.values(formErrors).every(error => error === '');
    const hasAllFields = formData.agencyName && formData.ownerName && formData.businessLicenseNumber && 
                         formData.officeAddress && formData.fleetSize && documents.businessLicense;
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

    if (!documents.businessLicense) {
      setFormErrors({ ...formErrors, businessLicense: 'Business license document is required' });
      setLoading(false);
      return;
    }

    try {
      const formDataToSend = new FormData();
      formDataToSend.append('agencyName', formData.agencyName);
      formDataToSend.append('ownerName', formData.ownerName);
      formDataToSend.append('businessLicenseNumber', formData.businessLicenseNumber);
      formDataToSend.append('officeAddress', formData.officeAddress);
      formDataToSend.append('fleetSize', formData.fleetSize);
      if (formData.taxRegistrationNumber) {
        formDataToSend.append('taxRegistrationNumber', formData.taxRegistrationNumber);
      }
      formDataToSend.append('businessLicenseDocument', documents.businessLicense);
      if (documents.taxRegistration) {
        formDataToSend.append('taxRegistrationDocument', documents.taxRegistration);
      }
      if (documents.ownerIdentity) {
        formDataToSend.append('ownerIdentityDocument', documents.ownerIdentity);
      }

      await authAPI.completeVendorProfile(parseInt(userId), formDataToSend);
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
      <Card sx={{ maxWidth: 600, width: '100%' }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h4" gutterBottom align="center">
            Vendor Registration
          </Typography>
          <Typography variant="body2" color="text.secondary" align="center" sx={{ mb: 3 }}>
            Complete your business details
          </Typography>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <form onSubmit={handleSubmit}>
            <TextField
              fullWidth
              label="Agency Name"
              name="agencyName"
              value={formData.agencyName}
              onChange={handleChange}
              error={!!formErrors.agencyName}
              helperText={formErrors.agencyName}
              required
              sx={{ mb: 2 }}
            />

            <TextField
              fullWidth
              label="Owner Name"
              name="ownerName"
              value={formData.ownerName}
              onChange={handleChange}
              error={!!formErrors.ownerName}
              helperText={formErrors.ownerName}
              required
              sx={{ mb: 2 }}
            />

            <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
              <TextField
                fullWidth
                label="Business License Number"
                name="businessLicenseNumber"
                value={formData.businessLicenseNumber}
                onChange={handleChange}
                error={!!formErrors.businessLicenseNumber}
                helperText={formErrors.businessLicenseNumber}
                required
              />
              <TextField
                fullWidth
                label="Tax Registration Number (Optional)"
                name="taxRegistrationNumber"
                value={formData.taxRegistrationNumber}
                onChange={handleChange}
                error={!!formErrors.taxRegistrationNumber}
                helperText={formErrors.taxRegistrationNumber}
              />
            </Box>

            <TextField
              fullWidth
              label="Office Address"
              name="officeAddress"
              value={formData.officeAddress}
              onChange={handleChange}
              error={!!formErrors.officeAddress}
              helperText={formErrors.officeAddress}
              multiline
              rows={2}
              required
              sx={{ mb: 2 }}
            />

            <TextField
              fullWidth
              label="Fleet Size"
              name="fleetSize"
              value={formData.fleetSize}
              onChange={handleChange}
              error={!!formErrors.fleetSize}
              helperText={formErrors.fleetSize}
              placeholder="Number of buses"
              required
              sx={{ mb: 2 }}
            />

            <Typography variant="subtitle1" sx={{ mb: 1, mt: 2 }}>
              Upload Documents
            </Typography>

            <Box>
              <Button
                variant="outlined"
                component="label"
                fullWidth
                color={formErrors.businessLicense ? 'error' : 'primary'}
                sx={{ mb: 1 }}
              >
                Business License Document (Required) *
                <input
                  type="file"
                  hidden
                  accept=".pdf,.jpg,.jpeg,.png"
                  onChange={(e) => handleFileChange('businessLicense', e.target.files?.[0] || null)}
                />
              </Button>
              {documents.businessLicense && (
                <Typography variant="caption" color="success.main" sx={{ mb: 1, display: 'block' }}>
                  ✓ Selected: {documents.businessLicense.name}
                </Typography>
              )}
              {formErrors.businessLicense && (
                <Typography variant="caption" color="error" sx={{ mb: 1, display: 'block' }}>
                  {formErrors.businessLicense}
                </Typography>
              )}
            </Box>

            <Button
              variant="outlined"
              component="label"
              fullWidth
              sx={{ mb: 1, mt: 1 }}
            >
              Tax Registration Document (Optional)
              <input
                type="file"
                hidden
                accept=".pdf,.jpg,.jpeg,.png"
                onChange={(e) => handleFileChange('taxRegistration', e.target.files?.[0] || null)}
              />
            </Button>
            {documents.taxRegistration && (
              <Typography variant="caption" color="success.main" sx={{ mb: 1, display: 'block' }}>
                ✓ Selected: {documents.taxRegistration.name}
              </Typography>
            )}

            <Button
              variant="outlined"
              component="label"
              fullWidth
              sx={{ mb: 1, mt: 1 }}
            >
              Owner Identity Document (Optional)
              <input
                type="file"
                hidden
                accept=".pdf,.jpg,.jpeg,.png"
                onChange={(e) => handleFileChange('ownerIdentity', e.target.files?.[0] || null)}
              />
            </Button>
            {documents.ownerIdentity && (
              <Typography variant="caption" color="success.main" sx={{ mb: 2, display: 'block' }}>
                ✓ Selected: {documents.ownerIdentity.name}
              </Typography>
            )}

            <Button
              fullWidth
              variant="contained"
              type="submit"
              disabled={loading || !isFormValid()}
              sx={{ py: 1.5, mt: 2 }}
            >
              {loading ? <CircularProgress size={24} /> : 'Submit for Approval'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
};

export default VendorProfile;
