import { useState, useEffect } from 'react';
import {
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  Grid,
  Box,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress,
  Avatar,
  IconButton
} from '@mui/material';
import { Save, Cancel, Person, PhotoCamera, AccountCircle } from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { updateCustomerProfile, updateProfilePicture, clearError, clearUpdateSuccess, fetchCustomerProfile } from './customerProfileSlice';
import type { UpdateCustomerProfile } from './customerProfileAPI';

interface ProfileEditProps {
  onCancel: () => void;
  onSuccess: () => void;
}

const ProfileEdit = ({ onCancel, onSuccess }: ProfileEditProps) => {
  const dispatch = useAppDispatch();
  const { profile, loading, error, updateSuccess } = useAppSelector((state) => state.customerProfile);

  const [formData, setFormData] = useState<UpdateCustomerProfile>({
    firstName: '',
    middleName: '',
    lastName: '',
    dateOfBirth: '',
    gender: 1,
    phone: '',
  });

  const [formErrors, setFormErrors] = useState<Record<string, string>>({});
  const [profileImage, setProfileImage] = useState<string | null>(null);
  const [imageFile, setImageFile] = useState<File | null>(null);

  useEffect(() => {
    if (profile) {
      setFormData({
        firstName: profile.firstName,
        middleName: profile.middleName || '',
        lastName: profile.lastName,
        dateOfBirth: profile.dateOfBirth.split('T')[0],
        gender: profile.gender,
        phone: profile.phone,
      });
    }
  }, [profile]);

  useEffect(() => {
    if (updateSuccess) {
      onSuccess();
      dispatch(clearUpdateSuccess());
    }
  }, [updateSuccess, onSuccess, dispatch]);

  useEffect(() => {
    return () => {
      dispatch(clearError());
      dispatch(clearUpdateSuccess());
    };
  }, [dispatch]);

  const validateField = (name: string, value: string | number) => {
    let error = '';

    switch (name) {
      case 'firstName':
      case 'lastName':
        if (!value) error = `${name === 'firstName' ? 'First' : 'Last'} name is required`;
        else if (typeof value === 'string' && value.length < 2) error = 'Must be at least 2 characters';
        else if (typeof value === 'string' && value.length > 50) error = 'Cannot exceed 50 characters';
        else if (typeof value === 'string' && !/^[A-Za-z\s]+$/.test(value)) error = 'Only letters and spaces allowed';
        break;
      case 'middleName':
        if (value && typeof value === 'string' && value.length > 50) error = 'Cannot exceed 50 characters';
        else if (value && typeof value === 'string' && !/^[A-Za-z\s]*$/.test(value)) error = 'Only letters and spaces allowed';
        break;
      case 'phone':
        if (!value) error = 'Phone number is required';
        else if (typeof value === 'string' && !/^[6-9]\d{9}$/.test(value)) error = 'Enter valid 10-digit mobile number';
        break;
      case 'dateOfBirth':
        if (!value) error = 'Date of birth is required';
        else {
          const birthDate = new Date(value as string);
          const today = new Date();
          const age = today.getFullYear() - birthDate.getFullYear();
          if (birthDate > today) error = 'Date of birth cannot be in future';
          else if (age < 18) error = 'Must be at least 18 years old';
          else if (age > 100) error = 'Invalid date of birth';
        }
        break;
    }

    return error;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | any) => {
    const { name, value } = e.target;
    
    let filteredValue = value;
    if (name === 'firstName' || name === 'lastName' || name === 'middleName') {
      filteredValue = value.replace(/[^A-Za-z\s]/g, '');
    } else if (name === 'phone') {
      filteredValue = value.replace(/[^0-9]/g, '').slice(0, 10);
    }

    setFormData(prev => ({ ...prev, [name]: filteredValue }));
    
    const error = validateField(name, filteredValue);
    setFormErrors(prev => ({ ...prev, [name]: error }));
  };

  const isFormValid = () => {
    const requiredFields = ['firstName', 'lastName', 'phone', 'dateOfBirth'];
    const hasAllRequired = requiredFields.every(field => formData[field as keyof UpdateCustomerProfile]);
    const hasNoErrors = Object.values(formErrors).every(error => !error);
    return hasAllRequired && hasNoErrors;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validate all fields
    const errors: Record<string, string> = {};
    Object.keys(formData).forEach(key => {
      const value = formData[key as keyof UpdateCustomerProfile];
      const error = validateField(key, value ?? '');
      if (error) errors[key] = error;
    });

    if (Object.keys(errors).length > 0) {
      setFormErrors(errors);
      return;
    }

    console.log('Form data being sent:', formData);

    try {
      // Update profile data
      const profileResult = await dispatch(updateCustomerProfile(formData));
      
      // Upload profile picture if selected
      if (imageFile && updateCustomerProfile.fulfilled.match(profileResult)) {
        await dispatch(updateProfilePicture(imageFile));
        // Refresh profile to get updated picture
        await dispatch(fetchCustomerProfile());
      }
    } catch (error) {
      console.error('Profile update failed:', error);
    }
  };

  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      if (file.size > 5 * 1024 * 1024) {
        setFormErrors(prev => ({ ...prev, image: 'Image size must be less than 5MB' }));
        return;
      }
      if (!file.type.startsWith('image/')) {
        setFormErrors(prev => ({ ...prev, image: 'Please select a valid image file' }));
        return;
      }
      setImageFile(file);
      const reader = new FileReader();
      reader.onload = (e) => {
        setProfileImage(e.target?.result as string);
      };
      reader.readAsDataURL(file);
      setFormErrors(prev => ({ ...prev, image: '' }));
    }
  };

  const removeImage = () => {
    setProfileImage(null);
    setImageFile(null);
    setFormErrors(prev => ({ ...prev, image: '' }));
  };

  return (
    <Card>
      <CardContent>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 3 }}>
          <Person color="primary" />
          <Typography variant="h5">
            Edit Profile
          </Typography>
        </Box>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <form onSubmit={handleSubmit}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mb: 2 }}>
                <Box sx={{ position: 'relative', mb: 2 }}>
                  {profileImage ? (
                    <Avatar
                      src={profileImage}
                      sx={{ width: 120, height: 120, border: 3, borderColor: 'primary.main' }}
                    />
                  ) : (
                    <Avatar sx={{ width: 120, height: 120, bgcolor: 'grey.300' }}>
                      <AccountCircle sx={{ fontSize: 80, color: 'grey.600' }} />
                    </Avatar>
                  )}
                  <IconButton
                    sx={{
                      position: 'absolute',
                      bottom: 0,
                      right: 0,
                      bgcolor: 'primary.main',
                      color: 'white',
                      '&:hover': { bgcolor: 'primary.dark' },
                      width: 40,
                      height: 40
                    }}
                    component="label"
                    disabled={loading}
                  >
                    <PhotoCamera fontSize="small" />
                    <input
                      type="file"
                      hidden
                      accept="image/*"
                      onChange={handleImageChange}
                    />
                  </IconButton>
                </Box>
                <Typography variant="body2" color="text.secondary" align="center">
                  Click camera icon to upload profile picture
                </Typography>
                {profileImage && (
                  <Button
                    size="small"
                    color="error"
                    onClick={removeImage}
                    sx={{ mt: 1 }}
                    disabled={loading}
                  >
                    Remove Picture
                  </Button>
                )}
                {formErrors.image && (
                  <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                    {formErrors.image}
                  </Typography>
                )}
              </Box>
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="First Name"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                error={!!formErrors.firstName}
                helperText={formErrors.firstName}
                required
                disabled={loading}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Middle Name"
                name="middleName"
                value={formData.middleName}
                onChange={handleChange}
                error={!!formErrors.middleName}
                helperText={formErrors.middleName}
                disabled={loading}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Last Name"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                error={!!formErrors.lastName}
                helperText={formErrors.lastName}
                required
                disabled={loading}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Phone Number"
                name="phone"
                value={formData.phone}
                onChange={handleChange}
                error={!!formErrors.phone}
                helperText={formErrors.phone}
                placeholder="9876543210"
                required
                disabled={loading}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
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
                inputProps={{ 
                  max: new Date().toISOString().split('T')[0],
                  min: new Date(new Date().getFullYear() - 100, 0, 1).toISOString().split('T')[0]
                }}
                required
                disabled={loading}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth disabled={loading}>
                <InputLabel>Gender</InputLabel>
                <Select
                  name="gender"
                  value={formData.gender}
                  onChange={handleChange}
                  label="Gender"
                >
                  <MenuItem value={1}>Male</MenuItem>
                  <MenuItem value={2}>Female</MenuItem>
                  <MenuItem value={3}>Other</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end', mt: 2 }}>
                <Button
                  variant="outlined"
                  startIcon={<Cancel />}
                  onClick={onCancel}
                  disabled={loading}
                >
                  Cancel
                </Button>
                <Button
                  type="submit"
                  variant="contained"
                  startIcon={loading ? <CircularProgress size={20} /> : <Save />}
                  disabled={loading || !isFormValid()}
                >
                  {loading ? 'Saving...' : 'Save Changes'}
                </Button>
              </Box>
            </Grid>
          </Grid>
        </form>
      </CardContent>
    </Card>
  );
};

export default ProfileEdit;