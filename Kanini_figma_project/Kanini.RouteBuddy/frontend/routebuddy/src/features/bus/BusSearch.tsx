import { useState } from 'react';
import { TextField, Button, Alert } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { searchBuses } from './busSlice';
import { ROUTES } from '../../utils/constants';
import PlaceAutocomplete from '../../components/ui/PlaceAutocomplete';

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
    if (value.length < 1) return 'Source must be at least 1 character';
    return '';
  };

  const validateDestination = (value: string) => {
    if (!value) return 'Destination is required';
    if (value.length < 1) return 'Destination must be at least 1 character';
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
    setFormData({ ...formData, [name]: value });

    let fieldError = '';
    if (name === 'travelDate') {
      fieldError = validateDate(value);
      setFormErrors({ ...formErrors, [name]: fieldError });
    }
  };

  const handlePlaceChange = (field: 'source' | 'destination', value: string) => {
    setFormData({ ...formData, [field]: value });
    
    let fieldError = '';
    if (field === 'source') {
      fieldError = validateSource(value);
      // Re-validate destination if source changes
      if (formData.destination) {
        setFormErrors(prev => ({
          ...prev,
          destination: validateDestination(formData.destination)
        }));
      }
    } else {
      fieldError = validateDestination(value);
    }
    
    setFormErrors({ ...formErrors, [field]: fieldError });
  };

  const handlePlaceError = (field: 'source' | 'destination', error: string) => {
    setFormErrors({ ...formErrors, [field]: error });
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
    <div className="bus-search-card">
      <h2 className="bus-search-title">🚌 Search Buses</h2>

      {error && (
        <Alert severity="error" sx={{ mb: 2, borderRadius: '8px' }}>
          {error}
        </Alert>
      )}

      <form onSubmit={handleSubmit}>
        <div className="bus-search-form">
          <PlaceAutocomplete
            label="From"
            value={formData.source}
            onChange={(value) => handlePlaceChange('source', value)}
            onError={(error) => handlePlaceError('source', error)}
            error={formErrors.source}
            placeholder="Mumbai"
            required
          />

          <PlaceAutocomplete
            label="To"
            value={formData.destination}
            onChange={(value) => handlePlaceChange('destination', value)}
            onError={(error) => handlePlaceError('destination', error)}
            error={formErrors.destination}
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
            className="search-button"
          >
            {loading ? 'Searching...' : 'Search'}
          </Button>
        </div>
      </form>
    </div>
  );
};

export default BusSearch;
