import { useEffect } from 'react';
import { 
  Card, 
  CardContent, 
  Typography, 
  Grid, 
  Chip, 
  Box, 
  Button,
  Alert,
  Skeleton,
  Avatar
} from '@mui/material';
import { Edit, Person, Email, Phone, Cake, Wc, AccountCircle } from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { fetchCustomerProfile, clearError } from './customerProfileSlice';

interface ProfileViewProps {
  onEdit: () => void;
}

const ProfileView = ({ onEdit }: ProfileViewProps) => {
  const dispatch = useAppDispatch();
  const { profile, loading, error } = useAppSelector((state) => state.customerProfile);

  useEffect(() => {
    console.log('ProfileView: Fetching customer profile...');
    dispatch(fetchCustomerProfile());
    return () => {
      dispatch(clearError());
    };
  }, [dispatch]);

  // Debug logging
  useEffect(() => {
    console.log('ProfileView: Profile state changed:', { profile, loading, error });
  }, [profile, loading, error]);

  const getGenderText = (gender: number) => {
    switch (gender) {
      case 1: return 'Male';
      case 2: return 'Female';
      case 3: return 'Other';
      default: return 'Not specified';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-IN', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  const calculateAge = (dateOfBirth: string) => {
    const today = new Date();
    const birthDate = new Date(dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    return age;
  };

  if (loading) {
    return (
      <Card>
        <CardContent>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
            <Skeleton variant="text" width={200} height={40} />
            <Skeleton variant="rectangular" width={100} height={36} />
          </Box>
          <Grid container spacing={3}>
            {[1, 2, 3, 4, 5, 6].map((item) => (
              <Grid item xs={12} sm={6} key={item}>
                <Skeleton variant="text" width="100%" height={60} />
              </Grid>
            ))}
          </Grid>
        </CardContent>
      </Card>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ mb: 2 }}>
        {error}
      </Alert>
    );
  }

  if (!profile) {
    return (
      <Alert severity="info">
        Profile not found. Debug info: {JSON.stringify({ loading, error })}
      </Alert>
    );
  }

  // Debug: Show raw profile data
  console.log('ProfileView: Rendering profile:', profile);

  return (
    <Card>
      <CardContent>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Typography variant="h5" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <Person color="primary" />
            My Profile
          </Typography>
          <Button
            variant="contained"
            startIcon={<Edit />}
            onClick={onEdit}
            size="small"
          >
            Edit Profile
          </Button>
        </Box>

        {/* Profile Picture Section */}
        <Box sx={{ display: 'flex', justifyContent: 'center', mb: 3 }}>
          <Avatar 
            sx={{ width: 100, height: 100, bgcolor: 'grey.300' }}
            src={profile.profilePictureBase64 ? `data:image/jpeg;base64,${profile.profilePictureBase64}` : undefined}
          >
            {!profile.profilePictureBase64 && <AccountCircle sx={{ fontSize: 70, color: 'grey.600' }} />}
          </Avatar>
        </Box>

        <Grid container spacing={3}>
          <Grid item xs={12} sm={6}>
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                Full Name
              </Typography>
              <Typography variant="body1" sx={{ fontWeight: 500 }}>
                {`${profile.firstName} ${profile.middleName ? profile.middleName + ' ' : ''}${profile.lastName}`}
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle2" color="text.secondary" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <Email fontSize="small" />
                Email
              </Typography>
              <Typography variant="body1">
                {profile.email}
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle2" color="text.secondary" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <Phone fontSize="small" />
                Phone
              </Typography>
              <Typography variant="body1">
                +91 {profile.phone}
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle2" color="text.secondary" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <Cake fontSize="small" />
                Date of Birth
              </Typography>
              <Typography variant="body1">
                {formatDate(profile.dateOfBirth)} ({calculateAge(profile.dateOfBirth)} years)
              </Typography>
            </Box>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle2" color="text.secondary" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <Wc fontSize="small" />
                Gender
              </Typography>
              <Chip 
                label={getGenderText(profile.gender)} 
                size="small" 
                color="primary" 
                variant="outlined"
              />
            </Box>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                Account Status
              </Typography>
              <Chip 
                label={profile.isActive ? 'Active' : 'Inactive'} 
                size="small" 
                color={profile.isActive ? 'success' : 'error'}
              />
            </Box>
          </Grid>

          <Grid item xs={12}>
            <Box sx={{ mt: 2, pt: 2, borderTop: 1, borderColor: 'divider' }}>
              <Typography variant="caption" color="text.secondary">
                Account created: {formatDate(profile.createdOn)} | 
                Last updated: {formatDate(profile.updatedOn)}
              </Typography>
            </Box>
          </Grid>
        </Grid>
      </CardContent>
    </Card>
  );
};

export default ProfileView;