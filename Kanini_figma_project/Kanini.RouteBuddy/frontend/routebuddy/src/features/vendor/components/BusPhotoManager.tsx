import React, { useEffect, useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Grid,
  ImageList,
  ImageListItem,
  ImageListItemBar,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Alert,
  CircularProgress,
} from '@mui/material';
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  Star as StarIcon,
  StarBorder as StarBorderIcon,
} from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../../hooks/useAppDispatch';
import {
  fetchBusPhotos,
  addBusPhoto,
  deleteBusPhoto,
} from '../busFleetSlice';

interface BusPhotoManagerProps {
  busId: number;
  onBack: () => void;
}

const BusPhotoManager: React.FC<BusPhotoManagerProps> = ({ busId, onBack }) => {
  const dispatch = useAppDispatch();
  const { busPhotos, loading, error } = useAppSelector((state) => state.busFleet);

  const [addPhotoDialogOpen, setAddPhotoDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [caption, setCaption] = useState('');
  const [photoToDelete, setPhotoToDelete] = useState<number | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);

  useEffect(() => {
    dispatch(fetchBusPhotos(busId));
  }, [dispatch, busId]);

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      // Validate file type
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];
      if (!allowedTypes.includes(file.type)) {
        alert('Please select a JPG, JPEG, or PNG image');
        return;
      }

      // Validate file size (5MB)
      if (file.size > 5 * 1024 * 1024) {
        alert('File size must be under 5MB');
        return;
      }

      setSelectedFile(file);
      
      // Create preview URL
      const url = URL.createObjectURL(file);
      setPreviewUrl(url);
    }
  };

  const handleAddPhoto = async () => {
    if (!selectedFile || !busId) return;

    try {
      await dispatch(addBusPhoto({
        busId,
        photo: selectedFile,
        caption: caption || undefined,
      })).unwrap();

      // Reset form
      setSelectedFile(null);
      setCaption('');
      setPreviewUrl(null);
      setAddPhotoDialogOpen(false);

      // Refresh photos
      dispatch(fetchBusPhotos(busId));
    } catch (error) {
      console.error('Failed to add photo:', error);
    }
  };

  const handleDeletePhoto = async () => {
    if (!photoToDelete) return;

    try {
      await dispatch(deleteBusPhoto(photoToDelete)).unwrap();
      setDeleteDialogOpen(false);
      setPhotoToDelete(null);

      // Refresh photos
      dispatch(fetchBusPhotos(busId));
    } catch (error) {
      console.error('Failed to delete photo:', error);
    }
  };

  const openDeleteDialog = (photoId: number) => {
    setPhotoToDelete(photoId);
    setDeleteDialogOpen(true);
  };

  const closeAddDialog = () => {
    setAddPhotoDialogOpen(false);
    setSelectedFile(null);
    setCaption('');
    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
      setPreviewUrl(null);
    }
  };

  if (loading && busPhotos.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="between" alignItems="center" mb={3}>
        <Typography variant="h4" component="h1">
          Bus Photo Management
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => setAddPhotoDialogOpen(true)}
          disabled={busPhotos.length >= 5}
        >
          Add Photo
        </Button>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {busPhotos.length >= 5 && (
        <Alert severity="info" sx={{ mb: 2 }}>
          Maximum 5 photos allowed per bus. Delete existing photos to add new ones.
        </Alert>
      )}

      {busPhotos.length === 0 ? (
        <Card>
          <CardContent>
            <Box textAlign="center" py={4}>
              <Typography variant="h6" color="text.secondary" gutterBottom>
                No photos uploaded yet
              </Typography>
              <Typography variant="body2" color="text.secondary" mb={2}>
                Add photos to showcase your bus to customers
              </Typography>
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={() => setAddPhotoDialogOpen(true)}
              >
                Upload First Photo
              </Button>
            </Box>
          </CardContent>
        </Card>
      ) : (
        <ImageList cols={3} gap={16}>
          {busPhotos.map((photo) => (
            <ImageListItem key={photo.busPhotoId}>
              <img
                src={`http://localhost:5172/${photo.imagePath}`}
                alt={photo.caption || 'Bus photo'}
                loading="lazy"
                style={{ height: 200, objectFit: 'cover' }}
              />
              <ImageListItemBar
                title={photo.caption || 'Bus Photo'}
                subtitle={photo.isPrimary ? 'Primary Photo' : ''}
                actionIcon={
                  <Box>
                    <IconButton
                      sx={{ color: 'rgba(255, 255, 255, 0.54)' }}
                      onClick={() => {/* Set as primary */}}
                    >
                      {photo.isPrimary ? <StarIcon /> : <StarBorderIcon />}
                    </IconButton>
                    <IconButton
                      sx={{ color: 'rgba(255, 255, 255, 0.54)' }}
                      onClick={() => openDeleteDialog(photo.busPhotoId)}
                    >
                      <DeleteIcon />
                    </IconButton>
                  </Box>
                }
              />
            </ImageListItem>
          ))}
        </ImageList>
      )}

      {/* Add Photo Dialog */}
      <Dialog open={addPhotoDialogOpen} onClose={closeAddDialog} maxWidth="sm" fullWidth>
        <DialogTitle>Add Bus Photo</DialogTitle>
        <DialogContent>
          <Box mb={2}>
            <input
              type="file"
              accept="image/jpeg,image/jpg,image/png"
              onChange={handleFileSelect}
              style={{ display: 'none' }}
              id="photo-upload"
            />
            <label htmlFor="photo-upload">
              <Button variant="outlined" component="span" fullWidth>
                Select Photo
              </Button>
            </label>
          </Box>

          {previewUrl && (
            <Box mb={2}>
              <img
                src={previewUrl}
                alt="Preview"
                style={{ width: '100%', maxHeight: 200, objectFit: 'cover' }}
              />
            </Box>
          )}

          <TextField
            fullWidth
            label="Caption (Optional)"
            value={caption}
            onChange={(e) => setCaption(e.target.value)}
            placeholder="Describe this photo..."
            multiline
            rows={2}
            inputProps={{ maxLength: 100 }}
            helperText={`${caption.length}/100 characters`}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={closeAddDialog}>Cancel</Button>
          <Button
            onClick={handleAddPhoto}
            variant="contained"
            disabled={!selectedFile || loading}
          >
            {loading ? 'Uploading...' : 'Add Photo'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Delete Photo</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete this photo? This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button
            onClick={handleDeletePhoto}
            color="error"
            variant="contained"
            disabled={loading}
          >
            {loading ? 'Deleting...' : 'Delete'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default BusPhotoManager;