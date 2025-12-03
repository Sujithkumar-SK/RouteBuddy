import { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  Grid,
  Autocomplete,
  Alert,
  CircularProgress,
  Stepper,
  Step,
  StepLabel,
  FormGroup,
  FormControlLabel,
  Checkbox
} from '@mui/material';
import { ArrowBack as BackIcon } from '@mui/icons-material';
import { scheduleAPI, type RouteSearch, type CreateBulkScheduleRequest } from '../scheduleAPI';
import { busFleetAPI, type BusFleetItem } from '../busFleetAPI';

interface CreateBulkScheduleFormProps {
  onBack: () => void;
  onSuccess: () => void;
}

const CreateBulkScheduleForm = ({ onBack, onSuccess }: CreateBulkScheduleFormProps) => {
  const [activeStep, setActiveStep] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Form data
  const [selectedBus, setSelectedBus] = useState<BusFleetItem | null>(null);
  const [selectedRoute, setSelectedRoute] = useState<RouteSearch | null>(null);
  const [formData, setFormData] = useState({
    startDate: '',
    endDate: '',
    departureTime: '',
    arrivalTime: ''
  });
  const [operatingDays, setOperatingDays] = useState<number[]>([]);

  // Options
  const [buses, setBuses] = useState<BusFleetItem[]>([]);
  const [routes, setRoutes] = useState<RouteSearch[]>([]);
  const [filteredRoutes, setFilteredRoutes] = useState<RouteSearch[]>([]);
  const [routeSearch, setRouteSearch] = useState('');
  const [availableStops, setAvailableStops] = useState<any[]>([]);
  const [selectedStops, setSelectedStops] = useState<any[]>([]);

  const steps = ['Select Bus', 'Select Route', 'Configure Stops', 'Set Bulk Schedule'];
  const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  useEffect(() => {
    fetchBuses();
    fetchAllRoutes();
  }, []);

  useEffect(() => {
    if (routeSearch.trim() === '') {
      setFilteredRoutes(routes);
    } else {
      const filtered = routes.filter(route => 
        route.source.toLowerCase().includes(routeSearch.toLowerCase()) ||
        route.destination.toLowerCase().includes(routeSearch.toLowerCase())
      );
      setFilteredRoutes(filtered);
    }
  }, [routeSearch, routes]);

  const fetchBuses = async () => {
    try {
      const response = await busFleetAPI.getBusFleet({ status: 1 });
      const busData = response.data?.data || response.data || [];
      setBuses(Array.isArray(busData) ? busData : []);
    } catch (err) {
      setError('Failed to fetch buses');
    }
  };

  const fetchAllRoutes = async () => {
    try {
      const response = await scheduleAPI.getAllRoutes();
      const routeData = response.data?.data || response.data || [];
      const routesArray = Array.isArray(routeData) ? routeData : [];
      setRoutes(routesArray);
      setFilteredRoutes(routesArray);
    } catch (err) {
      setError('Failed to fetch routes');
    }
  };

  const handleNext = async () => {
    if (activeStep === 1 && selectedRoute) {
      try {
        const response = await scheduleAPI.getAvailableStops();
        const stopsData = response.data || [];
        setAvailableStops(stopsData);
        setSelectedStops([]);
      } catch (err) {
        setError('Failed to fetch available stops');
        return;
      }
    }
    
    setActiveStep((prev) => prev + 1);
  };

  const handleBack = () => {
    setActiveStep((prev) => prev - 1);
  };

  const handleDayToggle = (dayIndex: number) => {
    setOperatingDays(prev => 
      prev.includes(dayIndex) 
        ? prev.filter(d => d !== dayIndex)
        : [...prev, dayIndex]
    );
  };

  const handleSubmit = async () => {
    if (!selectedBus || !selectedRoute || selectedStops.length === 0 || operatingDays.length === 0) return;

    let createdSchedules: any[] = [];
    
    try {
      setLoading(true);
      setError(null);
      
      const bulkScheduleData: CreateBulkScheduleRequest = {
        busId: selectedBus.busId,
        routeId: selectedRoute.routeId,
        startDate: formData.startDate,
        endDate: formData.endDate,
        departureTime: formData.departureTime,
        arrivalTime: formData.arrivalTime,
        operatingDays: operatingDays
      };

      const scheduleResponse = await scheduleAPI.createBulkSchedule(bulkScheduleData);
      createdSchedules = scheduleResponse.data || [];
      
      if (createdSchedules.length === 0) {
        throw new Error('No schedules were created. All dates may already exist or don\'t match operating days.');
      }

      // Create stops for each created schedule
      const routeStopsData = selectedStops.map(stop => ({
        stopId: stop.stopId,
        orderNumber: stop.orderNumber,
        arrivalTime: stop.arrivalTime || null,
        departureTime: stop.departureTime || null
      }));

      let stopsCreatedCount = 0;
      const failedStops: number[] = [];

      // Create stops for all schedules
      for (const schedule of createdSchedules) {
        try {
          await scheduleAPI.createScheduleStops(schedule.scheduleId, routeStopsData);
          stopsCreatedCount++;
        } catch (stopsError: any) {
          console.error(`Failed to create stops for schedule ${schedule.scheduleId}:`, stopsError);
          failedStops.push(schedule.scheduleId);
        }
      }

      if (failedStops.length > 0) {
        setError(`Created ${createdSchedules.length} schedules, but failed to create stops for ${failedStops.length} schedules. Please check and add stops manually for schedules: ${failedStops.join(', ')}`);
      }
      
      onSuccess();
    } catch (err: any) {
      let errorMessage = 'Failed to create bulk schedules';
      
      if (err.response?.data?.errors) {
        // Handle validation errors
        const validationErrors = Object.values(err.response.data.errors).flat();
        errorMessage = validationErrors.join('; ');
      } else if (err.response?.data?.message) {
        errorMessage = err.response.data.message;
      } else if (err.response?.data?.error) {
        errorMessage = err.response.data.error;
      } else if (err.message) {
        errorMessage = err.message;
      }
      
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const renderStepContent = () => {
    switch (activeStep) {
      case 0:
        return (
          <Autocomplete
            options={buses}
            getOptionLabel={(option) => `${option.busName} (${option.registrationNo})`}
            value={selectedBus}
            onChange={(_, newValue) => setSelectedBus(newValue)}
            renderInput={(params) => (
              <TextField {...params} label="Select Bus" fullWidth />
            )}
          />
        );
      
      case 1:
        return (
          <Box>
            <TextField
              label="Search Routes"
              value={routeSearch}
              onChange={(e) => setRouteSearch(e.target.value)}
              fullWidth
              sx={{ mb: 2 }}
              placeholder="Type source or destination..."
            />
            <Autocomplete
              options={filteredRoutes}
              getOptionLabel={(option) => `${option.source} → ${option.destination} (${option.distance}km)`}
              value={selectedRoute}
              onChange={(_, newValue) => setSelectedRoute(newValue)}
              renderInput={(params) => (
                <TextField {...params} label="Select Route" fullWidth />
              )}
            />
          </Box>
        );
      
      case 2:
        return (
          <Box>
            <Typography variant="h6" sx={{ mb: 2 }}>Configure Route Stops</Typography>
            <Typography variant="body2" sx={{ mb: 3 }}>Search and select stops, then set timing and order</Typography>
            
            <Box sx={{ mb: 3 }}>
              <Autocomplete
                options={availableStops.filter(stop => !selectedStops.find(s => s.stopId === stop.stopId))}
                getOptionLabel={(option) => `${option.name} - ${option.landmark || 'No landmark'}`}
                renderInput={(params) => (
                  <TextField 
                    {...params} 
                    label="Search and add stops" 
                    placeholder="Type stop name to search..."
                    fullWidth
                  />
                )}
                onChange={(_, newValue) => {
                  if (newValue) {
                    const newStop = {
                      stopId: newValue.stopId,
                      name: newValue.name,
                      landmark: newValue.landmark,
                      orderNumber: selectedStops.length + 1,
                      arrivalTime: '',
                      departureTime: ''
                    };
                    setSelectedStops([...selectedStops, newStop]);
                  }
                }}
                renderOption={(props, option) => {
                  const { key, ...otherProps } = props;
                  return (
                    <Box component="li" key={key} {...otherProps}>
                      <Box>
                        <Typography variant="subtitle2">{option.name}</Typography>
                        <Typography variant="caption" color="text.secondary">
                          {option.landmark || 'No landmark'}
                        </Typography>
                      </Box>
                    </Box>
                  );
                }}
              />
            </Box>

            {selectedStops.length === 0 ? (
              <Box sx={{ textAlign: 'center', py: 4 }}>
                <Typography color="text.secondary">No stops selected. Search and add stops above.</Typography>
              </Box>
            ) : (
              <Box>
                <Typography variant="subtitle1" sx={{ mb: 2 }}>Selected Stops ({selectedStops.length})</Typography>
                <Grid container spacing={2}>
                  {selectedStops
                    .sort((a, b) => a.orderNumber - b.orderNumber)
                    .map((stop) => (
                    <Grid item xs={12} key={stop.stopId}>
                      <Card variant="outlined" sx={{ p: 2 }}>
                        <Grid container spacing={2} alignItems="center">
                          <Grid item xs={3}>
                            <Typography variant="subtitle1">{stop.name}</Typography>
                            <Typography variant="body2" color="text.secondary">
                              {stop.landmark || 'No landmark'}
                            </Typography>
                          </Grid>
                          <Grid item xs={2}>
                            <TextField
                              label="Order"
                              type="number"
                              size="small"
                              value={stop.orderNumber}
                              onChange={(e) => {
                                const newOrder = parseInt(e.target.value);
                                const updatedStops = selectedStops.map(s => 
                                  s.stopId === stop.stopId 
                                    ? { ...s, orderNumber: newOrder }
                                    : s
                                );
                                setSelectedStops(updatedStops);
                              }}
                              InputProps={{ inputProps: { min: 1, max: selectedStops.length } }}
                            />
                          </Grid>
                          <Grid item xs={2.5}>
                            <TextField
                              label="Arrival Time"
                              type="time"
                              size="small"
                              value={stop.arrivalTime}
                              onChange={(e) => {
                                const updatedStops = selectedStops.map(s => 
                                  s.stopId === stop.stopId 
                                    ? { ...s, arrivalTime: e.target.value }
                                    : s
                                );
                                setSelectedStops(updatedStops);
                              }}
                              InputLabelProps={{ shrink: true }}
                            />
                          </Grid>
                          <Grid item xs={2.5}>
                            <TextField
                              label="Departure Time"
                              type="time"
                              size="small"
                              value={stop.departureTime}
                              onChange={(e) => {
                                const updatedStops = selectedStops.map(s => 
                                  s.stopId === stop.stopId 
                                    ? { ...s, departureTime: e.target.value }
                                    : s
                                );
                                setSelectedStops(updatedStops);
                              }}
                              InputLabelProps={{ shrink: true }}
                            />
                          </Grid>
                          <Grid item xs={2}>
                            <Button
                              color="error"
                              size="small"
                              onClick={() => {
                                const updatedStops = selectedStops.filter(s => s.stopId !== stop.stopId);
                                const reorderedStops = updatedStops.map((s, idx) => ({
                                  ...s,
                                  orderNumber: idx + 1
                                }));
                                setSelectedStops(reorderedStops);
                              }}
                            >
                              Remove
                            </Button>
                          </Grid>
                        </Grid>
                      </Card>
                    </Grid>
                  ))}
                </Grid>
              </Box>
            )}
          </Box>
        );
      
      case 3:
        return (
          <Grid container spacing={3}>
            <Grid item xs={6}>
              <TextField
                label="Start Date"
                type="date"
                value={formData.startDate}
                onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                fullWidth
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="End Date"
                type="date"
                value={formData.endDate}
                onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                fullWidth
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="Departure Time"
                type="time"
                value={formData.departureTime}
                onChange={(e) => setFormData({ ...formData, departureTime: e.target.value })}
                fullWidth
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="Arrival Time"
                type="time"
                value={formData.arrivalTime}
                onChange={(e) => setFormData({ ...formData, arrivalTime: e.target.value })}
                fullWidth
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={12}>
              <Typography variant="h6" sx={{ mb: 2 }}>Operating Days</Typography>
              <FormGroup row>
                {dayNames.map((day, index) => (
                  <FormControlLabel
                    key={index}
                    control={
                      <Checkbox
                        checked={operatingDays.includes(index)}
                        onChange={() => handleDayToggle(index)}
                      />
                    }
                    label={day}
                  />
                ))}
              </FormGroup>
            </Grid>
          </Grid>
        );
      
      default:
        return null;
    }
  };

  const isStepValid = () => {
    switch (activeStep) {
      case 0: return selectedBus !== null;
      case 1: return selectedRoute !== null;
      case 2: return selectedStops.length >= 2;
      case 3: return formData.startDate && formData.endDate && formData.departureTime && formData.arrivalTime && operatingDays.length > 0;
      default: return false;
    }
  };

  return (
    <Box>
      <Box display="flex" alignItems="center" mb={3}>
        <Button startIcon={<BackIcon />} onClick={onBack} sx={{ mr: 2 }}>
          Back
        </Button>
        <Typography variant="h5">Create Bulk Schedules</Typography>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Card>
        <CardContent>
          <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
            {steps.map((label) => (
              <Step key={label}>
                <StepLabel>{label}</StepLabel>
              </Step>
            ))}
          </Stepper>

          <Box sx={{ mb: 4 }}>
            {renderStepContent()}
          </Box>

          <Box display="flex" justifyContent="between">
            <Button
              disabled={activeStep === 0}
              onClick={handleBack}
            >
              Back
            </Button>
            
            {activeStep === steps.length - 1 ? (
              <Button
                variant="contained"
                onClick={handleSubmit}
                disabled={!isStepValid() || loading}
              >
                {loading ? <CircularProgress size={24} /> : 'Create Bulk Schedules'}
              </Button>
            ) : (
              <Button
                variant="contained"
                onClick={handleNext}
                disabled={!isStepValid()}
              >
                Next
              </Button>
            )}
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
};

export default CreateBulkScheduleForm;