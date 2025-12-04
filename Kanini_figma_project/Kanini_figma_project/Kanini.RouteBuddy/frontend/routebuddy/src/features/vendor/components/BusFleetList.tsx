import React, { useEffect, useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Chip,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Grid,
  Pagination,
  CircularProgress,
  Alert,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Photo as PhotoIcon,
  Settings as SettingsIcon,
  Search as SearchIcon,
} from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../../hooks/useAppDispatch';
import {
  fetchBusFleet,
  deleteBus,
  changeBusStatus,
  setSearchTerm,
  setStatusFilter,
  setCurrentPage,
  clearError,
} from '../busFleetSlice';
import type { BusFleetItem } from '../busFleetAPI';

const BusFleetList: React.FC = () => {
  const dispatch = useAppDispatch();
  const {
    buses,
    totalCount,
    currentPage,
    pageSize,
    loading,
    error,
    searchTerm,
    statusFilter,
  } = useAppSelector((state) => state.busFleet);

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [busToDelete, setBusToDelete] = useState<BusFleetItem | null>(null);

  useEffect(() => {
    dispatch(fetchBusFleet({
      pageNumber: currentPage,
      pageSize,
      status: statusFilter || undefined,
      search: searchTerm || undefined,
    }));
  }, [dispatch, currentPage, pageSize, statusFilter, searchTerm]);

  const handleSearch = (event: React.ChangeEvent<HTMLInputElement>) => {
    dispatch(setSearchTerm(event.target.value));
    dispatch(setCurrentPage(1));
  };

  const handleStatusFilter = (status: number | null) => {
    dispatch(setStatusFilter(status));
    dispatch(setCurrentPage(1));
  };

  const handlePageChange = (event: React.ChangeEvent<unknown>, page: number) => {
    dispatch(setCurrentPage(page));
  };

  const handleDeleteClick = (bus: BusFleetItem) => {
    setBusToDelete(bus);
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (busToDelete) {
      await dispatch(deleteBus(busToDelete.busId));
      setDeleteDialogOpen(false);
      setBusToDelete(null);
      // Refresh the list
      dispatch(fetchBusFleet({
        pageNumber: currentPage,
        pageSize,
        status: statusFilter || undefined,
        search: searchTerm || undefined,
      }));
    }
  };

  const handleStatusChange = async (busId: number, action: 'activate' | 'deactivate' | 'maintenance') => {
    await dispatch(changeBusStatus({ busId, action }));
    // Refresh the list
    dispatch(fetchBusFleet({
      pageNumber: currentPage,
      pageSize,
      status: statusFilter || undefined,
      search: searchTerm || undefined,
    }));
  };

  const getStatusColor = (status: string, isActive: boolean) => {
    if (!isActive) return 'error';
    switch (status.toLowerCase()) {
      case 'active': return 'success';
      case 'maintenance': return 'warning';
      case 'pendingapproval': return 'info';
      default: return 'default';
    }
  };

  const getBusTypeLabel = (busType: string) => {
    const types: { [key: string]: string } = {
      '1': 'AC',
      '2': 'Non-AC',
      '3': 'Sleeper',
      '4': 'Semi-Sleeper',
      '5': 'Volvo',
      '6': 'Luxury'
    };
    return types[busType] || busType;
  };

  if (loading && (!Array.isArray(buses) || buses.length === 0)) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      {/* Header */}
      <Box display="flex" justifyContent="between" alignItems="center" mb={3}>
        <Typography variant="h4" component="h1">
          Bus Fleet Management
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => {/* Navigate to add bus form */}}
        >
          Add New Bus
        </Button>
      </Box>

      {/* Filters */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Grid container spacing={2} alignItems="center">
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                placeholder="Search buses..."
                value={searchTerm}
                onChange={handleSearch}
                InputProps={{
                  startAdornment: <SearchIcon sx={{ mr: 1, color: 'text.secondary' }} />,
                }}
              />
            </Grid>
            <Grid item xs={12} md={3}>
              <FormControl fullWidth>
                <InputLabel>Status Filter</InputLabel>
                <Select
                  value={statusFilter || ''}
                  onChange={(e) => handleStatusFilter(e.target.value ? Number(e.target.value) : null)}
                  label="Status Filter"
                >
                  <MenuItem value="">All Status</MenuItem>
                  <MenuItem value={0}>Pending Approval</MenuItem>
                  <MenuItem value={1}>Active</MenuItem>
                  <MenuItem value={2}>Inactive</MenuItem>
                  <MenuItem value={3}>Maintenance</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} md={2}>
              <Typography variant="body2" color="text.secondary">
                Total: {totalCount} buses
              </Typography>
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      {/* Error Alert */}
      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => dispatch(clearError())}>
          {error}
        </Alert>
      )}

      {/* Bus List */}
      <Grid container spacing={2}>
        {Array.isArray(buses) && buses.map((bus) => (
          <Grid item xs={12} md={6} lg={4} key={bus.busId}>
            <Card>
              <CardContent>
                <Box display="flex" justifyContent="between" alignItems="start" mb={2}>
                  <Typography variant="h6" component="h3">
                    {bus.busName}
                  </Typography>
                  <Chip
                    label={bus.status}
                    color={getStatusColor(bus.status, bus.isActive)}
                    size="small"
                  />
                </Box>

                <Typography variant="body2" color="text.secondary" gutterBottom>
                  Registration: {bus.registrationNo}
                </Typography>
                
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  Type: {getBusTypeLabel(bus.busType)} | Seats: {bus.totalSeats}
                </Typography>

                {bus.driverName && (
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    Driver: {bus.driverName}
                    {bus.driverContact && ` (${bus.driverContact})`}
                  </Typography>
                )}

                <Typography variant="caption" color="text.secondary">
                  Added: {new Date(bus.createdOn).toLocaleDateString()}
                </Typography>

                <Box display="flex" justifyContent="between" alignItems="center" mt={2}>
                  <Box>
                    <IconButton
                      size="small"
                      onClick={() => {/* Navigate to edit */}}
                      title="Edit Bus"
                    >
                      <EditIcon />
                    </IconButton>
                    <IconButton
                      size="small"
                      onClick={() => {/* Navigate to photos */}}
                      title="Manage Photos"
                    >
                      <PhotoIcon />
                    </IconButton>
                    <IconButton
                      size="small"
                      onClick={() => {/* Navigate to seat layout */}}
                      title="Seat Layout"
                    >
                      <SettingsIcon />
                    </IconButton>
                    <IconButton
                      size="small"
                      onClick={() => handleDeleteClick(bus)}
                      title="Delete Bus"
                      color="error"
                    >
                      <DeleteIcon />
                    </IconButton>
                  </Box>

                  <Box>
                    {bus.status.toLowerCase() === 'active' ? (
                      <>
                        <Button
                          size="small"
                          onClick={() => handleStatusChange(bus.busId, 'deactivate')}
                        >
                          Deactivate
                        </Button>
                        <Button
                          size="small"
                          onClick={() => handleStatusChange(bus.busId, 'maintenance')}
                          sx={{ ml: 1 }}
                        >
                          Maintenance
                        </Button>
                      </>
                    ) : (
                      <Button
                        size="small"
                        onClick={() => handleStatusChange(bus.busId, 'activate')}
                      >
                        Activate
                      </Button>
                    )}
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* Pagination */}
      {totalCount > pageSize && (
        <Box display="flex" justifyContent="center" mt={3}>
          <Pagination
            count={Math.ceil(totalCount / pageSize)}
            page={currentPage}
            onChange={handlePageChange}
            color="primary"
          />
        </Box>
      )}

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Delete Bus</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete "{busToDelete?.busName}"? This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleDeleteConfirm} color="error" variant="contained">
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default BusFleetList;