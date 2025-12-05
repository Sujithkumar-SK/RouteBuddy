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
import type { CreateStopRequest } from './adminRouteAPI';
import { adminRouteAPI } from './adminRouteAPI';

interface AddStopFormProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

const AddStopForm = ({ open, onClose, onSuccess }: AddStopFormProps) => {
  const [formData, setFormData] = useState<CreateStopRequest>({
    name: '',
    landmark: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await adminRouteAPI.createStop(formData);
      onSuccess();
      onClose();
      setFormData({
        name: '',
        landmark: '',
      });
    } catch (err: any) {
      setError(err.response?.data?.error || 'Failed to create stop');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (field: keyof CreateStopRequest) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    setFormData(prev => ({ ...prev, [field]: e.target.value }));
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Add New Stop</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
            {error && <Alert severity="error">{error}</Alert>}
            
            <TextField
              label="Stop Name"
              value={formData.name}
              onChange={handleChange('name')}
              required
              fullWidth
              inputProps={{ maxLength: 100 }}
            />
            
            <TextField
              label="Landmark (Optional)"
              value={formData.landmark}
              onChange={handleChange('landmark')}
              fullWidth
              inputProps={{ maxLength: 200 }}
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
            Create Stop
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default AddStopForm;