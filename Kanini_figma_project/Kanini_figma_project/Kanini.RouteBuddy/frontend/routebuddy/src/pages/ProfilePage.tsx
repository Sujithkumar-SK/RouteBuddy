import { useState, useEffect } from 'react';
import { Container, Alert, Snackbar } from '@mui/material';
import Layout from '../components/layout/Layout';
import ProfileView from '../features/auth/ProfileView';
import ProfileEdit from '../features/auth/ProfileEdit';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { checkAuthState } from '../features/auth/authSlice';

const ProfilePage = () => {
  const dispatch = useAppDispatch();
  const { user } = useAppSelector((state) => state.auth);
  const [isEditing, setIsEditing] = useState(false);
  const [showSuccess, setShowSuccess] = useState(false);

  useEffect(() => {
    dispatch(checkAuthState());
  }, [dispatch]);

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleCancel = () => {
    setIsEditing(false);
  };

  const handleSuccess = () => {
    setIsEditing(false);
    setShowSuccess(true);
  };

  const handleCloseSuccess = () => {
    setShowSuccess(false);
  };

  if (!user) {
    return (
      <Layout>
        <Container maxWidth="md" sx={{ py: 4 }}>
          <Alert severity="error">
            Please log in to view your profile
          </Alert>
        </Container>
      </Layout>
    );
  }

  return (
    <Layout>
      <Container maxWidth="md" sx={{ py: 4 }}>
        {isEditing ? (
          <ProfileEdit
            onCancel={handleCancel}
            onSuccess={handleSuccess}
          />
        ) : (
          <ProfileView
            onEdit={handleEdit}
          />
        )}

        <Snackbar
          open={showSuccess}
          autoHideDuration={4000}
          onClose={handleCloseSuccess}
          anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
        >
          <Alert onClose={handleCloseSuccess} severity="success" sx={{ width: '100%' }}>
            Profile updated successfully!
          </Alert>
        </Snackbar>
      </Container>
    </Layout>
  );
};

export default ProfilePage;