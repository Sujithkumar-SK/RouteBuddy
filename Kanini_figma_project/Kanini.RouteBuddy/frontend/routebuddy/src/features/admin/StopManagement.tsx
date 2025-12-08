import { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Chip,
  Typography,
  Alert,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import type { Stop, UpdateStopRequest } from './adminRouteAPI';
import { adminRouteAPI } from './adminRouteAPI';
import AddStopForm from './AddStopForm';

const StopManagement = () => {
  const [stops, setStops] = useState<Stop[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addFormOpen, setAddFormOpen] = useState(false);
  const [editFormOpen, setEditFormOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedStop, setSelectedStop] = useState<Stop | null>(null);
  const [editFormData, setEditFormData] = useState<UpdateStopRequest>({
    name: '',
    landmark: '',
  });

  const fetchStops = async () => {
    try {
      setLoading(true);
      const response = await adminRouteAPI.getAllStops(1, 100);
      setStops(response.data);
    } catch (err: any) {
      setError('Failed to fetch stops');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStops();
  }, []);

  const handleEdit = (stop: Stop) => {
    setSelectedStop(stop);
    setEditFormData({
      name: stop.name,
      landmark: stop.landmark || '',
    });
    setEditFormOpen(true);
  };

  const handleEditSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedStop) return;

    try {
      await adminRouteAPI.updateStop(selectedStop.stopId, editFormData);
      setEditFormOpen(false);
      fetchStops();
    } catch (err: any) {
      setError('Failed to update stop');
    }
  };

  const handleDelete = (stop: Stop) => {
    setSelectedStop(stop);
    setDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!selectedStop) return;

    try {
      await adminRouteAPI.deleteStop(selectedStop.stopId);
      setDeleteDialogOpen(false);
      fetchStops();
    } catch (err: any) {
      setError('Failed to delete stop');
    }
  };

  const handleEditChange = (field: keyof UpdateStopRequest) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    setEditFormData(prev => ({ ...prev, [field]: e.target.value }));
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h6">Stop Management</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={() => setAddFormOpen(true)}
        >
          Add Stop
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Stop Name</TableCell>
              <TableCell>Landmark</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Created On</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {stops.map((stop) => (
              <TableRow key={stop.stopId}>
                <TableCell>
                  <Typography variant="body2" sx={{ fontWeight: 500 }}>
                    {stop.name}
                  </Typography>
                </TableCell>
                <TableCell>{stop.landmark || '-'}</TableCell>
                <TableCell>
                  <Chip 
                    label={stop.isActive ? 'Active' : 'Inactive'} 
                    color={stop.isActive ? 'success' : 'default'}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  {new Date(stop.createdOn).toLocaleDateString()}
                </TableCell>
                <TableCell>
                  <IconButton onClick={() => handleEdit(stop)} size="small">
                    <Edit />
                  </IconButton>
                  <IconButton onClick={() => handleDelete(stop)} size="small" color="error">
                    <Delete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <AddStopForm
        open={addFormOpen}
        onClose={() => setAddFormOpen(false)}
        onSuccess={fetchStops}
      />

      <Dialog open={editFormOpen} onClose={() => setEditFormOpen(false)} maxWidth="sm" fullWidth>
        <form onSubmit={handleEditSubmit}>
          <DialogTitle>Edit Stop</DialogTitle>
          <DialogContent>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
              <TextField
                label="Stop Name"
                value={editFormData.name}
                onChange={handleEditChange('name')}
                required
                fullWidth
              />
              <TextField
                label="Landmark (Optional)"
                value={editFormData.landmark}
                onChange={handleEditChange('landmark')}
                fullWidth
              />
            </Box>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setEditFormOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained">Update</Button>
          </DialogActions>
        </form>
      </Dialog>

      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Delete Stop</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete the stop "{selectedStop?.name}"?
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button onClick={confirmDelete} color="error" variant="contained">Delete</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default StopManagement;