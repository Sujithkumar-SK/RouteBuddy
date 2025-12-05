import { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Box,
  Alert,
  CircularProgress,
} from '@mui/material';
import type { CreateRouteRequest } from './adminRouteAPI';
import { adminRouteAPI } from './adminRouteAPI';

interface AddRouteFormProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

const AddRouteForm = ({ open, onClose, onSuccess }: AddRouteFormProps) => {
  const [formData, setFormData] = useState<CreateRouteRequest>({
    source: '',
    destination: '',
    distance: 0,
    duration: '',
    basePrice: 0,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await adminRouteAPI.createRoute(formData);
      onSuccess();
      onClose();
      setFormData({
        source: '',
        destination: '',
        distance: 0,
        duration: '',
        basePrice: 0,
      });
    } catch (err: any) {
      setError(err.response?.data?.error || 'Failed to create route');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (field: keyof CreateRouteRequest) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = field === 'distance' || field === 'basePrice' 
      ? parseFloat(e.target.value) || 0 
      : e.target.value;
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Add New Route</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
            {error && <Alert severity="error">{error}</Alert>}
            
            <TextField
              label="Source"
              value={formData.source}
              onChange={handleChange('source')}
              required
              fullWidth
            />
            
            <TextField
              label="Destination"
              value={formData.destination}
              onChange={handleChange('destination')}
              required
              fullWidth
            />
            
            <TextField
              label="Distance (km)"
              type="number"
              value={formData.distance}
              onChange={handleChange('distance')}
              required
              fullWidth
              inputProps={{ min: 1, max: 5000 }}
            />
            
            <TextField
              label="Duration (HH:MM:SS)"
              value={formData.duration}
              onChange={handleChange('duration')}
              placeholder="06:30:00"
              required
              fullWidth
            />
            
            <TextField
              label="Base Price (₹)"
              type="number"
              value={formData.basePrice}
              onChange={handleChange('basePrice')}
              required
              fullWidth
              inputProps={{ min: 1, max: 10000 }}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} disabled={loading}>
            Cancel
          </Button>
          <Button 
            type="submit" 
            variant="contained" 
            disabled={loading}
            startIcon={loading && <CircularProgress size={20} />}
          >
            Create Route
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default AddRouteForm;