import { useState } from 'react';
import {
  Paper,
  Typography,
  FormControl,
  FormLabel,
  RadioGroup,
  FormControlLabel,
  Radio,
  Box,
  Chip,
} from '@mui/material';
import { AccessTime, LocationOn } from '@mui/icons-material';
import { useAppSelector, useAppDispatch } from '../../hooks/useAppDispatch';
import { setBoardingStop, setDroppingStop } from './bookingSlice';

const BoardingPointSelector = () => {
  const dispatch = useAppDispatch();
  const { routeStops, boardingStopId, droppingStopId } = useAppSelector((state) => state.booking);

  const formatTime = (time?: string) => {
    if (!time) return '';
    return new Date(`1970-01-01T${time}`).toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  };

  const handleBoardingChange = (stopId: string) => {
    const id = parseInt(stopId);
    dispatch(setBoardingStop(id));
    
    // Clear dropping stop if it's before boarding stop
    if (droppingStopId) {
      const boardingOrder = routeStops.find(s => s.stopId === id)?.orderNumber || 0;
      const droppingOrder = routeStops.find(s => s.stopId === droppingStopId)?.orderNumber || 0;
      if (droppingOrder <= boardingOrder) {
        dispatch(setDroppingStop(0));
      }
    }
  };

  const handleDroppingChange = (stopId: string) => {
    dispatch(setDroppingStop(parseInt(stopId)));
  };

  const getAvailableDropStops = () => {
    if (!boardingStopId) return [];
    const boardingOrder = routeStops.find(s => s.stopId === boardingStopId)?.orderNumber || 0;
    return routeStops.filter(stop => stop.orderNumber > boardingOrder);
  };

  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h6" sx={{ mb: 3 }}>
        Select Boarding & Dropping Points
      </Typography>

      {/* Boarding Point */}
      <Box sx={{ mb: 4 }}>
        <FormControl component="fieldset" fullWidth>
          <FormLabel component="legend" sx={{ mb: 2, fontWeight: 600 }}>
            <LocationOn sx={{ mr: 1, verticalAlign: 'middle' }} />
            Boarding Point
          </FormLabel>
          <RadioGroup
            value={boardingStopId?.toString() || ''}
            onChange={(e) => handleBoardingChange(e.target.value)}
          >
            {routeStops.map((stop) => (
              <FormControlLabel
                key={stop.stopId}
                value={stop.stopId.toString()}
                control={<Radio />}
                label={
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, py: 0.5 }}>
                    <Box>
                      <Typography variant="body1" sx={{ fontWeight: 500 }}>
                        {stop.stopName}
                      </Typography>
                      {stop.landmark && (
                        <Typography variant="body2" color="text.secondary">
                          {stop.landmark}
                        </Typography>
                      )}
                    </Box>
                    <Chip
                      icon={<AccessTime />}
                      label={formatTime(stop.departureTime)}
                      size="small"
                      variant="outlined"
                    />
                  </Box>
                }
                sx={{ 
                  alignItems: 'flex-start',
                  '& .MuiFormControlLabel-label': { flex: 1 }
                }}
              />
            ))}
          </RadioGroup>
        </FormControl>
      </Box>

      {/* Dropping Point */}
      <Box>
        <FormControl component="fieldset" fullWidth disabled={!boardingStopId}>
          <FormLabel component="legend" sx={{ mb: 2, fontWeight: 600 }}>
            <LocationOn sx={{ mr: 1, verticalAlign: 'middle' }} />
            Dropping Point
          </FormLabel>
          {!boardingStopId ? (
            <Typography variant="body2" color="text.secondary" sx={{ fontStyle: 'italic' }}>
              Please select boarding point first
            </Typography>
          ) : (
            <RadioGroup
              value={droppingStopId?.toString() || ''}
              onChange={(e) => handleDroppingChange(e.target.value)}
            >
              {getAvailableDropStops().map((stop) => (
                <FormControlLabel
                  key={stop.stopId}
                  value={stop.stopId.toString()}
                  control={<Radio />}
                  label={
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, py: 0.5 }}>
                      <Box>
                        <Typography variant="body1" sx={{ fontWeight: 500 }}>
                          {stop.stopName}
                        </Typography>
                        {stop.landmark && (
                          <Typography variant="body2" color="text.secondary">
                            {stop.landmark}
                          </Typography>
                        )}
                      </Box>
                      <Chip
                        icon={<AccessTime />}
                        label={formatTime(stop.arrivalTime)}
                        size="small"
                        variant="outlined"
                      />
                    </Box>
                  }
                  sx={{ 
                    alignItems: 'flex-start',
                    '& .MuiFormControlLabel-label': { flex: 1 }
                  }}
                />
              ))}
            </RadioGroup>
          )}
        </FormControl>
      </Box>
    </Paper>
  );
};

export default BoardingPointSelector;