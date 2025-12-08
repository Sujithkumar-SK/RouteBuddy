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
  StepLabel
} from '@mui/material';
import { ArrowBack as BackIcon } from '@mui/icons-material';
import { scheduleAPI, type RouteSearch, type CreateScheduleRequest } from '../scheduleAPI';
import { busFleetAPI, type BusFleetItem } from '../busFleetAPI';

interface CreateScheduleFormProps {
  onBack: () => void;
  onSuccess: () => void;
}

const CreateScheduleForm = ({ onBack, onSuccess }: CreateScheduleFormProps) => {
  const [activeStep, setActiveStep] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Form data
  const [selectedBus, setSelectedBus] = useState<BusFleetItem | null>(null);
  const [selectedRoute, setSelectedRoute] = useState<RouteSearch | null>(null);
  const [formData, setFormData] = useState({
    travelDate: '',
    departureTime: '',
    arrivalTime: ''
  });

  // Options
  const [buses, setBuses] = useState<BusFleetItem[]>([]);
  const [routes, setRoutes] = useState<RouteSearch[]>([]);
  const [filteredRoutes, setFilteredRoutes] = useState<RouteSearch[]>([]);
  const [routeSearch, setRouteSearch] = useState('');
  const [availableStops, setAvailableStops] = useState<any[]>([]);
  const [selectedStops, setSelectedStops] = useState<any[]>([]);

  const steps = ['Select Bus', 'Select Route', 'Configure Stops', 'Set Schedule'];

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
      const response = await busFleetAPI.getBusFleet({ status: 1 }); // Active buses only
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
      // Fetch available stops when moving from route selection to stops configuration
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
    
    // Don't create route stops here - do it after schedule creation
    
    setActiveStep((prev) => prev + 1);
  };

  const handleBack = () => {
    setActiveStep((prev) => prev - 1);
  };

  const handleSubmit = async () => {
    if (!selectedBus || !selectedRoute || selectedStops.length === 0) return;

    let createdScheduleId: number | null = null;
    
    try {
      setLoading(true);
      
      // First create schedule
      const scheduleData: CreateScheduleRequest = {
        busId: selectedBus.busId,
        routeId: selectedRoute.routeId,
        travelDate: formData.travelDate,
        departureTime: formData.departureTime,
        arrivalTime: formData.arrivalTime
      };

      const scheduleResponse = await scheduleAPI.createSchedule(scheduleData);
      createdScheduleId = scheduleResponse.data?.scheduleId || scheduleResponse.scheduleId;
      
      if (!createdScheduleId) {
        throw new Error('Schedule created but no ID returned');
      }
      
      // Then create schedule-specific route stops
      const routeStopsData = selectedStops.map(stop => ({
        stopId: stop.stopId,
        orderNumber: stop.orderNumber,
        arrivalTime: stop.arrivalTime || null,
        departureTime: stop.departureTime || null
      }));
      
      try {
        await scheduleAPI.createScheduleStops(createdScheduleId, routeStopsData);
      } catch (stopsError: any) {
        // If route stops creation fails, try to delete the created schedule
        try {
          // Note: You'll need to add deleteSchedule method to scheduleAPI
          console.error('Route stops creation failed, schedule may be orphaned:', createdScheduleId);
        } catch (deleteError) {
          console.error('Failed to cleanup orphaned schedule:', deleteError);
        }
        throw stopsError;
      }
      
      onSuccess();
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.response?.data?.error || err.message || 'Failed to create schedule';
      setError(errorMessage);
      
      if (createdScheduleId) {
        setError(`${errorMessage}. Schedule ${createdScheduleId} may need manual cleanup.`);
      }
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
            
            {/* Search and Add Stops */}
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

            {/* Selected Stops */}
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
                    .map((stop, index) => (
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
                                // Reorder remaining stops
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
          <Grid container spacing={2}>
            <Grid item xs={12}>
              <TextField
                label="Travel Date"
                type="date"
                value={formData.travelDate}
                onChange={(e) => setFormData({ ...formData, travelDate: e.target.value })}
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
      case 2: return selectedStops.length >= 2; // Must have at least 2 stops (source + destination)
      case 3: return formData.travelDate && formData.departureTime && formData.arrivalTime;
      default: return false;
    }
  };

  return (
    <Box>
      <Box display="flex" alignItems="center" mb={3}>
        <Button startIcon={<BackIcon />} onClick={onBack} sx={{ mr: 2 }}>
          Back
        </Button>
        <Typography variant="h5">Create New Schedule</Typography>
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
                {loading ? <CircularProgress size={24} /> : 'Create Schedule'}
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

export default CreateScheduleForm;