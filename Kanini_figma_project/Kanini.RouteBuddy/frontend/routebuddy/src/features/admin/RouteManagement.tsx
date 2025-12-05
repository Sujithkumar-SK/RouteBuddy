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
import type { Route, UpdateRouteRequest } from './adminRouteAPI';
import { adminRouteAPI } from './adminRouteAPI';
import AddRouteForm from './AddRouteForm';

const RouteManagement = () => {
  const [routes, setRoutes] = useState<Route[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addFormOpen, setAddFormOpen] = useState(false);
  const [editFormOpen, setEditFormOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedRoute, setSelectedRoute] = useState<Route | null>(null);
  const [editFormData, setEditFormData] = useState<UpdateRouteRequest>({
    source: '',
    destination: '',
    distance: 0,
    duration: '',
    basePrice: 0,
  });

  const fetchRoutes = async () => {
    try {
      setLoading(true);
      const response = await adminRouteAPI.getAllRoutes(1, 100);
      setRoutes(response.data);
    } catch (err: any) {
      setError('Failed to fetch routes');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoutes();
  }, []);

  const handleEdit = (route: Route) => {
    setSelectedRoute(route);
    setEditFormData({
      source: route.source,
      destination: route.destination,
      distance: route.distance,
      duration: route.duration,
      basePrice: route.basePrice,
    });
    setEditFormOpen(true);
  };

  const handleEditSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedRoute) return;

    try {
      await adminRouteAPI.updateRoute(selectedRoute.routeId, editFormData);
      setEditFormOpen(false);
      fetchRoutes();
    } catch (err: any) {
      setError('Failed to update route');
    }
  };

  const handleDelete = (route: Route) => {
    setSelectedRoute(route);
    setDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!selectedRoute) return;

    try {
      await adminRouteAPI.deleteRoute(selectedRoute.routeId);
      setDeleteDialogOpen(false);
      fetchRoutes();
    } catch (err: any) {
      setError('Failed to delete route');
    }
  };

  const handleEditChange = (field: keyof UpdateRouteRequest) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = field === 'distance' || field === 'basePrice' 
      ? parseFloat(e.target.value) || 0 
      : e.target.value;
    setEditFormData(prev => ({ ...prev, [field]: value }));
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
        <Typography variant="h6">Route Management</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={() => setAddFormOpen(true)}
        >
          Add Route
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Route</TableCell>
              <TableCell>Distance</TableCell>
              <TableCell>Duration</TableCell>
              <TableCell>Base Price</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {routes.map((route) => (
              <TableRow key={route.routeId}>
                <TableCell>
                  <Typography variant="body2" sx={{ fontWeight: 500 }}>
                    {route.source} → {route.destination}
                  </Typography>
                </TableCell>
                <TableCell>{route.distance} km</TableCell>
                <TableCell>{route.duration}</TableCell>
                <TableCell>₹{route.basePrice}</TableCell>
                <TableCell>
                  <Chip 
                    label={route.isActive ? 'Active' : 'Inactive'} 
                    color={route.isActive ? 'success' : 'default'}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  <IconButton onClick={() => handleEdit(route)} size="small">
                    <Edit />
                  </IconButton>
                  <IconButton onClick={() => handleDelete(route)} size="small" color="error">
                    <Delete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <AddRouteForm
        open={addFormOpen}
        onClose={() => setAddFormOpen(false)}
        onSuccess={fetchRoutes}
      />

      <Dialog open={editFormOpen} onClose={() => setEditFormOpen(false)} maxWidth="sm" fullWidth>
        <form onSubmit={handleEditSubmit}>
          <DialogTitle>Edit Route</DialogTitle>
          <DialogContent>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
              <TextField
                label="Source"
                value={editFormData.source}
                onChange={handleEditChange('source')}
                required
                fullWidth
              />
              <TextField
                label="Destination"
                value={editFormData.destination}
                onChange={handleEditChange('destination')}
                required
                fullWidth
              />
              <TextField
                label="Distance (km)"
                type="number"
                value={editFormData.distance}
                onChange={handleEditChange('distance')}
                required
                fullWidth
              />
              <TextField
                label="Duration (HH:MM:SS)"
                value={editFormData.duration}
                onChange={handleEditChange('duration')}
                required
                fullWidth
              />
              <TextField
                label="Base Price (₹)"
                type="number"
                value={editFormData.basePrice}
                onChange={handleEditChange('basePrice')}
                required
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
        <DialogTitle>Delete Route</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete the route from {selectedRoute?.source} to {selectedRoute?.destination}?
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

export default RouteManagement;