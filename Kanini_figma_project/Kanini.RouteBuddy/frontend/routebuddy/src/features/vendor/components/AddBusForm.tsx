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
import { createBus, fetchSeatLayoutTemplates } from '../busFleetSlice';
import { busFleetAPI, type CreateBusRequest } from '../busFleetAPI';

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
    .min(1, 'Total seats must be greater than 0'),
  registrationNo: yup
    .string()
    .required('Registration number is required')
    .matches(/^[A-Z]{2}[0-9]{2}[A-Z]{1,2}[0-9]{4}$/, 'Invalid registration format (e.g., MH12AB1234)'),
  seatLayoutTemplateId: yup
    .number()
    .required('Seat layout template is required')
    .min(1, 'Please select a valid seat layout template'),
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
  seatLayoutTemplateId: number;
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
  { value: 2048, label: 'Emergency Exit' },
];

const AddBusForm: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { loading, error, seatLayoutTemplates } = useAppSelector((state) => state.busFleet);
  
  const [selectedAmenities, setSelectedAmenities] = useState<number[]>([]);
  const [selectedBusType, setSelectedBusType] = useState<number>(1);
  const [selectedTemplate, setSelectedTemplate] = useState<any>(null);
  const [seatLayoutDetails, setSeatLayoutDetails] = useState<any[]>([]);
  const [loadingLayout, setLoadingLayout] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string>('');

  const {
    control,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
    reset,
  } = useForm<FormData>({
    resolver: yupResolver(schema),
    defaultValues: {
      busName: '',
      busType: 1,
      totalSeats: 0,
      registrationNo: '',
      driverName: '',
      driverContact: '',
      seatLayoutTemplateId: 0,
    },
  });

  const watchedBusType = watch('busType');

  useEffect(() => {
    if (watchedBusType) {
      setSelectedBusType(watchedBusType);
    }
    // Load all templates regardless of bus type
    dispatch(fetchSeatLayoutTemplates());
  }, [watchedBusType, dispatch]);

  const handleAmenityChange = (amenityValue: number) => {
    setSelectedAmenities(prev => 
      prev.includes(amenityValue)
        ? prev.filter(a => a !== amenityValue)
        : [...prev, amenityValue]
    );
  };

  const handleTemplateChange = async (templateId: number) => {
    const template = seatLayoutTemplates.find(t => t.seatLayoutTemplateId === templateId);
    if (template) {
      // Auto-fill total seats and bus type from template
      setValue('totalSeats', template.totalSeats);
      setValue('busType', template.busType);
      setSelectedTemplate(template);
      
      // Fetch seat layout details
      setLoadingLayout(true);
      try {
        const layoutDetails = await busFleetAPI.getSeatLayoutDetails(templateId);
        setSeatLayoutDetails(layoutDetails.seatDetails || []);
      } catch (error) {
        console.error('Failed to fetch seat layout details:', error);
        setSeatLayoutDetails([]);
      } finally {
        setLoadingLayout(false);
      }
    } else {
      setSelectedTemplate(null);
      setSeatLayoutDetails([]);
    }
  };

  const renderSeatLayoutPreview = () => {
    if (!selectedTemplate) return null;

    const { templateName, totalSeats, busType } = selectedTemplate;

    if (loadingLayout) {
      return (
        <Card sx={{ mt: 2, p: 2 }}>
          <Typography variant="h6" gutterBottom>
            Loading Seat Layout Preview...
          </Typography>
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
            <CircularProgress />
          </Box>
        </Card>
      );
    }

    if (seatLayoutDetails.length === 0) {
      return (
        <Card sx={{ mt: 2, p: 2 }}>
          <Typography variant="h6" gutterBottom>
            Seat Layout Preview: {templateName}
          </Typography>
          <Alert severity="info">
            No seat layout details available for this template.
          </Alert>
        </Card>
      );
    }

    // Get max row and column to create grid
    const maxRow = Math.max(...seatLayoutDetails.map(seat => seat.rowNumber));
    const maxCol = Math.max(...seatLayoutDetails.map(seat => seat.columnNumber));

    // Create seat grid
    const seatGrid = Array(maxRow).fill(null).map(() => Array(maxCol).fill(null));
    
    // Populate grid with seat details
    seatLayoutDetails.forEach(seat => {
      seatGrid[seat.rowNumber - 1][seat.columnNumber - 1] = seat;
    });

    const getSeatColor = (seat: any) => {
      if (!seat) return 'transparent';
      
      // Color by seat position
      switch (seat.seatPosition) {
        case 1: return '#e3f2fd'; // Window - Blue
        case 2: return '#fff3e0'; // Aisle - Orange  
        case 3: return '#f3e5f5'; // Middle - Purple
        default: return '#f5f5f5'; // Default - Gray
      }
    };

    const getSeatTypeLabel = (seatType: number) => {
      switch (seatType) {
        case 1: return 'Seater';
        case 2: return 'Sleeper';
        case 3: return 'Semi-Sleeper';
        default: return 'Unknown';
      }
    };

    return (
      <Card sx={{ mt: 2, p: 2 }}>
        <Typography variant="h6" gutterBottom>
          Seat Layout Preview: {templateName}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Total Seats: {totalSeats} | Type: {busTypes.find(bt => bt.value === busType)?.label}
        </Typography>
        
        <Box sx={{ 
          display: 'flex', 
          flexDirection: 'column', 
          gap: 1, 
          maxWidth: 400, 
          mx: 'auto',
          p: 2,
          border: '2px solid #ddd',
          borderRadius: 2,
          bgcolor: '#f9f9f9'
        }}>
          {/* Driver Section */}
          <Box sx={{ 
            textAlign: 'center', 
            p: 1, 
            bgcolor: '#e3f2fd', 
            borderRadius: 1,
            mb: 1
          }}>
            <Typography variant="caption">🚗 Driver</Typography>
          </Box>
          
          {/* Dynamic Seat Grid */}
          {seatGrid.map((row, rowIndex) => (
            <Box key={rowIndex} sx={{ display: 'flex', gap: 0.5, justifyContent: 'center' }}>
              {row.map((seat, colIndex) => (
                <Box
                  key={colIndex}
                  sx={{
                    width: 35,
                    height: 25,
                    border: seat ? '1px solid #666' : 'none',
                    borderRadius: 0.5,
                    bgcolor: getSeatColor(seat),
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: '9px',
                    fontWeight: 'bold',
                    opacity: seat ? 1 : 0
                  }}
                >
                  {seat?.seatNumber}
                </Box>
              ))}
            </Box>
          ))}
          
          {/* Legend */}
          <Box sx={{ mt: 2, display: 'flex', gap: 2, justifyContent: 'center', flexWrap: 'wrap' }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <Box sx={{ width: 15, height: 15, bgcolor: '#e3f2fd', border: '1px solid #666' }} />
              <Typography variant="caption">Window</Typography>
            </Box>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <Box sx={{ width: 15, height: 15, bgcolor: '#fff3e0', border: '1px solid #666' }} />
              <Typography variant="caption">Aisle</Typography>
            </Box>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <Box sx={{ width: 15, height: 15, bgcolor: '#f3e5f5', border: '1px solid #666' }} />
              <Typography variant="caption">Middle</Typography>
            </Box>
          </Box>
        </Box>
      </Card>
    );
  };

  const onSubmit = async (data: FormData) => {
    try {
      if (!data.registrationCertificate || data.registrationCertificate.length === 0) {
        setSuccessMessage('Please select a registration certificate file');
        setTimeout(() => setSuccessMessage(''), 3000);
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
        seatLayoutTemplateId: data.seatLayoutTemplateId,
        registrationCertificate: data.registrationCertificate[0],
      };

      await dispatch(createBus(createBusData)).unwrap();
      
      // Show success message
      setSuccessMessage('Bus created successfully!');
      
      // Redirect to vendor dashboard after 2 seconds
      setTimeout(() => {
        navigate('/vendor/dashboard');
      }, 2000);
      
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
                      <InputLabel>Bus Type (Auto-filled)</InputLabel>
                      <Select {...field} label="Bus Type (Auto-filled)" disabled>
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
                      label="Total Seats (Auto-filled)"
                      error={!!errors.totalSeats}
                      helperText={errors.totalSeats?.message || "Automatically filled from selected template"}
                      inputProps={{ min: 10, max: 200, readOnly: true }}
                      InputProps={{ readOnly: true }}
                      sx={{ '& .MuiInputBase-input': { backgroundColor: '#f5f5f5' } }}
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

              {/* Seat Layout Template - Mandatory */}
              <Grid item xs={12} md={6}>
                <Controller
                  name="seatLayoutTemplateId"
                  control={control}
                  render={({ field }) => (
                    <FormControl fullWidth error={!!errors.seatLayoutTemplateId}>
                      <InputLabel>Seat Layout Template *</InputLabel>
                      <Select 
                        {...field} 
                        label="Seat Layout Template *" 
                        required
                        onChange={(e) => {
                          field.onChange(e);
                          handleTemplateChange(Number(e.target.value));
                        }}
                      >
                        <MenuItem value={0}>Select a seat layout template</MenuItem>
                        {seatLayoutTemplates.map((template) => (
                          <MenuItem key={template.seatLayoutTemplateId} value={template.seatLayoutTemplateId}>
                            {template.templateName} ({template.totalSeats} seats) - {busTypes.find(bt => bt.value === template.busType)?.label}
                          </MenuItem>
                        ))}
                      </Select>
                      {errors.seatLayoutTemplateId && (
                        <Typography variant="caption" color="error" sx={{ mt: 1 }}>
                          {errors.seatLayoutTemplateId.message}
                        </Typography>
                      )}
                    </FormControl>
                  )}
                />
              </Grid>
              
              {seatLayoutTemplates.length === 0 && (
                <Grid item xs={12}>
                  <Alert severity="warning">
                    No seat layout templates available for the selected bus type. Please contact admin to create templates.
                  </Alert>
                </Grid>
              )}
              
              {/* Seat Layout Preview */}
              {selectedTemplate && (
                <Grid item xs={12}>
                  {renderSeatLayoutPreview()}
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
                    onClick={() => navigate('/vendor/dashboard')}
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