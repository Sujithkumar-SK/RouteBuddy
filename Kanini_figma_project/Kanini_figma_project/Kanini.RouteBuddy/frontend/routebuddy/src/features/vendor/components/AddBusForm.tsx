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
import { useAppDispatch, useAppSelector } from '../../../hooks/useAppDispatch';
import { createBus, fetchSeatLayoutTemplates } from '../busFleetSlice';
import type { CreateBusRequest } from '../busFleetAPI';

const schema = yup.object({
  busName: yup
    .string()
    .required('Bus name is required')
    .min(3, 'Bus name must be at least 3 characters')
    .max(100, 'Bus name cannot exceed 100 characters')
    .matches(/^[a-zA-Z0-9\s\-_]+$/, 'Bus name can only contain letters, numbers, spaces, hyphens and underscores'),
  busType: yup.number().required('Bus type is required'),
  totalSeats: yup
    .number()
    .required('Total seats is required')
    .min(10, 'Minimum 10 seats required')
    .max(200, 'Maximum 200 seats allowed'),
  registrationNo: yup
    .string()
    .required('Registration number is required')
    .matches(/^[A-Z]{2}[0-9]{2}[A-Z]{1,2}[0-9]{4}$/, 'Invalid registration format (e.g., MH12AB1234)'),
  driverName: yup
    .string()
    .optional()
    .test('driver-name', 'Driver name must be 2-100 characters', (value) => 
      !value || (value.length >= 2 && value.length <= 100))
    .matches(/^[a-zA-Z\s]*$/, 'Driver name can only contain letters and spaces'),
  driverContact: yup
    .string()
    .optional()
    .test('driver-contact', 'Invalid mobile number', (value) => 
      !value || /^[6-9]\d{9}$/.test(value)),
});

interface FormData {
  busName: string;
  busType: number;
  totalSeats: number;
  registrationNo: string;
  driverName?: string;
  driverContact?: string;
  seatLayoutTemplateId?: number;
  registrationCertificate?: FileList;
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
];

const AddBusForm: React.FC = () => {
  const dispatch = useAppDispatch();
  const { loading, error, seatLayoutTemplates } = useAppSelector((state) => state.busFleet);
  
  const [selectedAmenities, setSelectedAmenities] = useState<number[]>([]);
  const [selectedBusType, setSelectedBusType] = useState<number>(1);

  const {
    control,
    handleSubmit,
    watch,
    formState: { errors },
    reset,
  } = useForm<FormData>({
    resolver: yupResolver(schema),
    defaultValues: {
      busName: '',
      busType: 1,
      totalSeats: 40,
      registrationNo: '',
      driverName: '',
      driverContact: '',
    },
  });

  const watchedBusType = watch('busType');

  useEffect(() => {
    if (watchedBusType) {
      setSelectedBusType(watchedBusType);
      dispatch(fetchSeatLayoutTemplates(watchedBusType));
    }
  }, [watchedBusType, dispatch]);

  const handleAmenityChange = (amenityValue: number) => {
    setSelectedAmenities(prev => 
      prev.includes(amenityValue)
        ? prev.filter(a => a !== amenityValue)
        : [...prev, amenityValue]
    );
  };

  const onSubmit = async (data: FormData) => {
    try {
      if (!data.registrationCertificate || data.registrationCertificate.length === 0) {
        alert('Please select a registration certificate file');
        return;
      }

      const amenitiesValue = selectedAmenities.reduce((sum, amenity) => sum + amenity, 0);
      
      const createBusData: CreateBusRequest = {
        busName: data.busName,
        busType: data.busType,
        totalSeats: data.totalSeats,
        registrationNo: data.registrationNo.toUpperCase(),
        amenities: amenitiesValue,
        driverName: data.driverName || undefined,
        driverContact: data.driverContact || undefined,
        seatLayoutTemplateId: data.seatLayoutTemplateId || undefined,
        registrationCertificate: data.registrationCertificate[0],
      };

      await dispatch(createBus(createBusData)).unwrap();
      
      // Reset form on success
      reset();
      setSelectedAmenities([]);
      
      // Show success message or navigate back
      alert('Bus created successfully!');
      
    } catch (error) {
      console.error('Failed to create bus:', error);
    }
  };

  return (
    <Box maxWidth="800px" mx="auto">
      <Typography variant="h4" component="h1" gutterBottom>
        Add New Bus
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
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
                      placeholder="e.g., Volvo Express, City Connect"
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <Controller
                  name="busType"
                  control={control}
                  render={({ field }) => (
                    <FormControl fullWidth error={!!errors.busType}>
                      <InputLabel>Bus Type</InputLabel>
                      <Select {...field} label="Bus Type">
                        {busTypes.map((type) => (
                          <MenuItem key={type.value} value={type.value}>
                            {type.label}
                          </MenuItem>
                        ))}
                      </Select>
                    </FormControl>
                  )}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <Controller
                  name="totalSeats"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="number"
                      label="Total Seats"
                      error={!!errors.totalSeats}
                      helperText={errors.totalSeats?.message}
                      inputProps={{ min: 10, max: 200 }}
                    />
                  )}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <Controller
                  name="registrationNo"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Registration Number"
                      error={!!errors.registrationNo}
                      helperText={errors.registrationNo?.message}
                      placeholder="e.g., MH12AB1234"
                      inputProps={{ style: { textTransform: 'uppercase' } }}
                    />
                  )}
                />
              </Grid>

              {/* Driver Information */}
              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                  Driver Information (Optional)
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
                      placeholder="e.g., Rajesh Kumar"
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
                      placeholder="e.g., 9876543210"
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

              {/* Seat Layout Template */}
              {seatLayoutTemplates.length > 0 && (
                <Grid item xs={12} md={6}>
                  <Controller
                    name="seatLayoutTemplateId"
                    control={control}
                    render={({ field }) => (
                      <FormControl fullWidth>
                        <InputLabel>Seat Layout Template (Optional)</InputLabel>
                        <Select {...field} label="Seat Layout Template (Optional)">
                          <MenuItem value="">None - Configure Later</MenuItem>
                          {seatLayoutTemplates.map((template) => (
                            <MenuItem key={template.seatLayoutTemplateId} value={template.seatLayoutTemplateId}>
                              {template.templateName} ({template.totalSeats} seats)
                            </MenuItem>
                          ))}
                        </Select>
                      </FormControl>
                    )}
                  />
                </Grid>
              )}

              {/* Registration Certificate */}
              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                  Registration Certificate
                </Typography>
                <Divider sx={{ mb: 2 }} />
                <Controller
                  name="registrationCertificate"
                  control={control}
                  render={({ field: { onChange, value, ...field } }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type="file"
                      onChange={(e) => onChange((e.target as HTMLInputElement).files)}
                      inputProps={{
                        accept: '.pdf,.jpg,.jpeg,.png',
                      }}
                      helperText="Upload PDF, JPG, JPEG, or PNG file (Max 5MB)"
                    />
                  )}
                />
              </Grid>

              {/* Submit Button */}
              <Grid item xs={12}>
                <Box display="flex" gap={2} justifyContent="flex-end" mt={2}>
                  <Button
                    variant="outlined"
                    onClick={() => {/* Navigate back */}}
                  >
                    Cancel
                  </Button>
                  <Button
                    type="submit"
                    variant="contained"
                    disabled={loading}
                    startIcon={loading ? <CircularProgress size={20} /> : null}
                  >
                    {loading ? 'Creating...' : 'Create Bus'}
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

export default AddBusForm;