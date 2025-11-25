import { useState } from 'react';
import { Box, Card, TextField, Button, Typography, Alert } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { searchBuses } from './busSlice';
import { ROUTES } from '../../utils/constants';

const BusSearch = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { loading, error } = useAppSelector((state) => state.bus);

  const [formData, setFormData] = useState({
    source: '',
    destination: '',
    travelDate: '',
  });

  const [formErrors, setFormErrors] = useState({
    source: '',
    destination: '',
    travelDate: '',
  });

  const validateSource = (value: string) => {
    if (!value) return 'Source is required';
    if (value.length < 2) return 'Source must be at least 2 characters';
    return '';
  };

  const validateDestination = (value: string) => {
    if (!value) return 'Destination is required';
    if (value.length < 2) return 'Destination must be at least 2 characters';
    if (value.toLowerCase() === formData.source.toLowerCase()) return 'Source and destination cannot be same';
    return '';
  };

  const validateDate = (value: string) => {
    if (!value) return 'Travel date is required';
    const selectedDate = new Date(value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (selectedDate < today) return 'Travel date cannot be in the past';
    return '';
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    let filteredValue = value;

    if (name === 'source' || name === 'destination') {
      filteredValue = value.replace(/[^A-Za-z\s]/g, '');
    }

    setFormData({ ...formData, [name]: filteredValue });

    let fieldError = '';
    switch (name) {
      case 'source':
        fieldError = validateSource(filteredValue);
        if (formData.destination) {
          setFormErrors(prev => ({
            ...prev,
            destination: validateDestination(formData.destination)
          }));
        }
        break;
      case 'destination':
        fieldError = validateDestination(filteredValue);
        break;
      case 'travelDate':
        fieldError = validateDate(filteredValue);
        break;
    }

    setFormErrors({ ...formErrors, [name]: fieldError });
  };

  const isFormValid = () => {
    const hasNoErrors = Object.values(formErrors).every(error => error === '');
    const hasAllFields = formData.source && formData.destination && formData.travelDate;
    return hasNoErrors && hasAllFields;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isFormValid()) return;

    const result = await dispatch(searchBuses(formData));
    if (searchBuses.fulfilled.match(result)) {
      // Navigate to search results page if not already there
      if (window.location.pathname !== ROUTES.SEARCH_RESULTS) {
        navigate(ROUTES.SEARCH_RESULTS);
      }
    }
  };

  return (
    <Card sx={{ p: 3, mb: 3 }}>
      <Typography variant="h5" gutterBottom sx={{ mb: 3, fontWeight: 600 }}>
        🚌 Search Buses
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <form onSubmit={handleSubmit}>
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr 1fr auto' }, gap: 2 }}>
          <TextField
            fullWidth
            label="From"
            name="source"
            value={formData.source}
            onChange={handleChange}
            error={!!formErrors.source}
            helperText={formErrors.source}
            placeholder="Mumbai"
            required
          />

          <TextField
            fullWidth
            label="To"
            name="destination"
            value={formData.destination}
            onChange={handleChange}
            error={!!formErrors.destination}
            helperText={formErrors.destination}
            placeholder="Pune"
            required
          />

          <TextField
            fullWidth
            label="Travel Date"
            name="travelDate"
            type="date"
            value={formData.travelDate}
            onChange={handleChange}
            error={!!formErrors.travelDate}
            helperText={formErrors.travelDate}
            InputLabelProps={{ shrink: true }}
            inputProps={{ min: new Date().toISOString().split('T')[0] }}
            required
          />

          <Button
            variant="contained"
            type="submit"
            disabled={loading || !isFormValid()}
            sx={{ height: 56, px: 4 }}
          >
            {loading ? 'Searching...' : 'Search'}
          </Button>
        </Box>
      </form>
    </Card>
  );
};

export default BusSearch;
