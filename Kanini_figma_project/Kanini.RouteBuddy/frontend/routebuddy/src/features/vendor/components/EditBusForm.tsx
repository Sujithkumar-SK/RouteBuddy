import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormControlLabel,
  Checkbox,
  Grid,
  Alert,
  CircularProgress,
  Divider,
} from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../../hooks/useAppDispatch';
import { updateBus, fetchBusDetails, clearCurrentBus } from '../busFleetSlice';
import type { UpdateBusRequest } from '../busFleetAPI';

const schema = yup.object({
  busName: yup
    .string()
    .required('Bus name is required')
    .min(3, 'Bus name must be at least 3 characters')
    .max(100, 'Bus name cannot exceed 100 characters'),
  driverName: yup
    .string()
    .optional()
    .test('driver-name', 'Driver name must be 2-100 characters', (value) => 
      !value || (value.length >= 2 && value.length <= 100)),
  driverContact: yup
    .string()
    .optional()
    .test('driver-contact', 'Invalid mobile number', (value) => 
      !value || /^[6-9]\d{9}$/.test(value)),
});

interface FormData {
  busName: string;
  driverName?: string;
  driverContact?: string;
}

const busTypes = [
  { value: 1, label: 'AC' },
  { value: 2, label: 'Non-AC' },
  { value: 3, label: 'Sleeper' },
  { value: 4, label: 'Semi-Sleeper' },
  { value: 5, label: 'Volvo' },
  { value: 6, label: 'Luxury' },
];

const amenitiesOptions = [
  { value: 1, label: 'AC' },
  { value: 2, label: 'WiFi' },
  { value: 4, label: 'Charging' },
  { value: 8, label: 'Blanket' },
  { value: 16, label: 'Pillow' },
  { value: 32, label: 'Meals' },
  { value: 64, label: 'Washroom' },
  { value: 128, label: 'USB' },
  { value: 256, label: 'Reading Light' },
  { value: 512, label: 'Entertainment' },
  { value: 1024, label: 'Reclining Seats' },
  { value: 2048, label: 'Emergency Exit' },
];

interface EditBusFormProps {
  busId: number;
  onBack: () => void;
}

const EditBusForm: React.FC<EditBusFormProps> = ({ busId, onBack }) => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { currentBus, loading, error } = useAppSelector((state) => state.busFleet);
  
  const [selectedAmenities, setSelectedAmenities] = useState<number[]>([]);
  const [successMessage, setSuccessMessage] = useState<string>('');

  const {
    control,
    handleSubmit,
    setValue,
    formState: { errors },
    reset,
  } = useForm<FormData>({
    resolver: yupResolver(schema),
    defaultValues: {
      busName: '',
      driverName: '',
      driverContact: '',
    },
  });

  useEffect(() => {
    if (busId) {
      dispatch(fetchBusDetails(busId));
    }
    
    return () => {
      dispatch(clearCurrentBus());
    };
  }, [dispatch, busId]);

  useEffect(() => {
    if (currentBus) {
      setValue('busName', currentBus.busName);
      setValue('driverName', currentBus.driverName || '');
      setValue('driverContact', currentBus.driverContact || '');
      
      // Parse amenities
      const amenities: number[] = [];
      amenitiesOptions.forEach(option => {
        if (currentBus.amenities & option.value) {
          amenities.push(option.value);
        }
      });
      setSelectedAmenities(amenities);
    }
  }, [currentBus, setValue]);

  const handleAmenityChange = (amenityValue: number) => {
    setSelectedAmenities(prev => 
      prev.includes(amenityValue)
        ? prev.filter(a => a !== amenityValue)
        : [...prev, amenityValue]
    );
  };

  const onSubmit = async (data: FormData) => {
    try {
      if (!busId) return;

      const amenitiesValue = selectedAmenities.reduce((sum, amenity) => sum + amenity, 0);
      
      const updateBusData: UpdateBusRequest = {
        busName: data.busName,
        busType: currentBus!.busType,
        totalSeats: currentBus!.totalSeats,
        amenities: amenitiesValue,
        driverName: data.driverName || undefined,
        driverContact: data.driverContact || undefined,
      };

      await dispatch(updateBus({ busId, data: updateBusData })).unwrap();
      
      setSuccessMessage('Bus updated successfully!');
      
      setTimeout(() => {
        onBack();
      }, 2000);
      
    } catch (error) {
      console.error('Failed to update bus:', error);
    }
  };

  if (loading && !currentBus) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  if (!currentBus && !loading) {
    return (
      <Box>
        <Alert severity="error">
          Bus not found or you don't have permission to edit this bus.
        </Alert>
      </Box>
    );
  }

  return (
    <Box maxWidth="800px" mx="auto">
      <Typography variant="h4" component="h1" gutterBottom>
        Edit Bus: {currentBus?.busName}
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {successMessage && (
        <Box sx={{ mb: 2, p: 2, bgcolor: '#d4edda', border: '1px solid #c3e6cb', borderRadius: 1 }}>
          <Typography component="span" sx={{ color: '#155724', fontWeight: 'medium' }}>
            {successMessage}
          </Typography>
        </Box>
      )}

      <Card>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={3}>
              {/* Basic Information */}
              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom>
                  Basic Information
                </Typography>
                <Divider sx={{ mb: 2 }} />
              </Grid>

              <Grid item xs={12} md={6}>
                <Controller
                  name="busName"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Bus Name"
                      error={!!errors.busName}
                      helperText={errors.busName?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Bus Type"
                  value={busTypes.find(t => t.value === currentBus?.busType)?.label || ''}
                  disabled
                  helperText="Bus type is determined by seat layout template"
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Total Seats"
                  value={currentBus?.totalSeats || 0}
                  disabled
                  helperText="Total seats is determined by seat layout template"
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Registration Number"
                  value={currentBus?.registrationNo || ''}
                  disabled
                  helperText="Registration number cannot be changed"
                />
              </Grid>

              {/* Driver Information */}
              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                  Driver Information
                </Typography>
                <Divider sx={{ mb: 2 }} />
              </Grid>

              <Grid item xs={12} md={6}>
                <Controller
                  name="driverName"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Driver Name"
                      error={!!errors.driverName}
                      helperText={errors.driverName?.message}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <Controller
                  name="driverContact"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Driver Contact"
                      error={!!errors.driverContact}
                      helperText={errors.driverContact?.message}
                    />
                  )}
                />
              </Grid>

              {/* Amenities */}
              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                  Amenities
                </Typography>
                <Divider sx={{ mb: 2 }} />
                <Grid container spacing={1}>
                  {amenitiesOptions.map((amenity) => (
                    <Grid item xs={6} md={4} key={amenity.value}>
                      <FormControlLabel
                        control={
                          <Checkbox
                            checked={selectedAmenities.includes(amenity.value)}
                            onChange={() => handleAmenityChange(amenity.value)}
                          />
                        }
                        label={amenity.label}
                      />
                    </Grid>
                  ))}
                </Grid>
              </Grid>

              {/* Submit Button */}
              <Grid item xs={12}>
                <Box display="flex" gap={2} justifyContent="flex-end" mt={2}>
                  <Button
                    variant="outlined"
                    onClick={onBack}
                  >
                    Cancel
                  </Button>
                  <Button
                    type="submit"
                    variant="contained"
                    disabled={loading}
                    startIcon={loading ? <CircularProgress size={20} /> : null}
                  >
                    {loading ? 'Updating...' : 'Update Bus'}
                  </Button>
                </Box>
              </Grid>
            </Grid>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
};

export default EditBusForm;