import React, { useEffect, useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Alert,
  Chip,
  Paper,
} from '@mui/material';
import { useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../../hooks/useAppDispatch';
import {
  fetchSeatLayoutTemplates,
  fetchBusDetails,
} from '../busFleetSlice';

const SeatLayoutConfig: React.FC = () => {
  const { busId } = useParams<{ busId: string }>();
  const dispatch = useAppDispatch();
  const { currentBus, seatLayoutTemplates, loading, error } = useAppSelector((state) => state.busFleet);

  const [selectedTemplate, setSelectedTemplate] = useState<number | null>(null);

  useEffect(() => {
    if (busId) {
      dispatch(fetchBusDetails(Number(busId)));
    }
  }, [dispatch, busId]);

  useEffect(() => {
    if (currentBus) {
      dispatch(fetchSeatLayoutTemplates(Number(currentBus.busType)));
    }
  }, [dispatch, currentBus]);

  const handleApplyTemplate = async () => {
    if (!selectedTemplate || !busId) return;

    try {
      // Apply template logic here
      await dispatch(/* applySeatLayoutTemplate({ busId: Number(busId), templateId: selectedTemplate }) */).unwrap();
      alert('Seat layout template applied successfully!');
    } catch (error) {
      console.error('Failed to apply template:', error);
    }
  };

  const renderSeatPreview = () => {
    // Simple seat layout preview
    const rows = 10;
    const seatsPerRow = 4;
    
    return (
      <Paper sx={{ p: 2, mt: 2 }}>
        <Typography variant="h6" gutterBottom>
          Seat Layout Preview
        </Typography>
        <Box display="flex" flexDirection="column" gap={1}>
          {Array.from({ length: rows }, (_, rowIndex) => (
            <Box key={rowIndex} display="flex" gap={1} justifyContent="center">
              {Array.from({ length: seatsPerRow }, (_, seatIndex) => {
                const seatNumber = `${String.fromCharCode(65 + Math.floor(seatIndex / 2))}${rowIndex + 1}${seatIndex % 2 === 0 ? 'L' : 'U'}`;
                return (
                  <Box
                    key={seatIndex}
                    sx={{
                      width: 40,
                      height: 30,
                      border: '1px solid #ccc',
                      borderRadius: 1,
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: '10px',
                      backgroundColor: seatIndex % 2 === 0 ? '#e3f2fd' : '#f3e5f5',
                      cursor: 'pointer',
                      '&:hover': {
                        backgroundColor: '#ffeb3b',
                      },
                    }}
                  >
                    {seatNumber}
                  </Box>
                );
              })}
              {/* Aisle gap */}
              {seatIndex === 1 && <Box sx={{ width: 20 }} />}
            </Box>
          ))}
        </Box>
        
        <Box mt={2} display="flex" gap={2} flexWrap="wrap">
          <Box display="flex" alignItems="center" gap={1}>
            <Box sx={{ width: 20, height: 15, backgroundColor: '#e3f2fd', border: '1px solid #ccc' }} />
            <Typography variant="caption">Lower Berth</Typography>
          </Box>
          <Box display="flex" alignItems="center" gap={1}>
            <Box sx={{ width: 20, height: 15, backgroundColor: '#f3e5f5', border: '1px solid #ccc' }} />
            <Typography variant="caption">Upper Berth</Typography>
          </Box>
        </Box>
      </Paper>
    );
  };

  if (!currentBus) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <Typography>Loading bus details...</Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom>
        Seat Layout Configuration
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Grid container spacing={3}>
        {/* Bus Information */}
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Bus Information
              </Typography>
              <Typography variant="body2" gutterBottom>
                <strong>Name:</strong> {currentBus.busName}
              </Typography>
              <Typography variant="body2" gutterBottom>
                <strong>Type:</strong> {currentBus.busType}
              </Typography>
              <Typography variant="body2" gutterBottom>
                <strong>Total Seats:</strong> {currentBus.totalSeats}
              </Typography>
              <Typography variant="body2" gutterBottom>
                <strong>Registration:</strong> {currentBus.registrationNo}
              </Typography>
              <Chip
                label={currentBus.status}
                color={currentBus.isActive ? 'success' : 'error'}
                size="small"
                sx={{ mt: 1 }}
              />
            </CardContent>
          </Card>
        </Grid>

        {/* Template Selection */}
        <Grid item xs={12} md={8}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Select Seat Layout Template
              </Typography>
              
              {seatLayoutTemplates.length > 0 ? (
                <>
                  <FormControl fullWidth sx={{ mb: 2 }}>
                    <InputLabel>Choose Template</InputLabel>
                    <Select
                      value={selectedTemplate || ''}
                      onChange={(e) => setSelectedTemplate(Number(e.target.value))}
                      label="Choose Template"
                    >
                      {seatLayoutTemplates.map((template) => (
                        <MenuItem key={template.seatLayoutTemplateId} value={template.seatLayoutTemplateId}>
                          {template.templateName} ({template.totalSeats} seats)
                          {template.description && ` - ${template.description}`}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>

                  <Box display="flex" gap={2}>
                    <Button
                      variant="contained"
                      onClick={handleApplyTemplate}
                      disabled={!selectedTemplate || loading}
                    >
                      Apply Template
                    </Button>
                    <Button
                      variant="outlined"
                      onClick={() => {/* Navigate to custom layout designer */}}
                    >
                      Custom Layout
                    </Button>
                  </Box>
                </>
              ) : (
                <Alert severity="info">
                  No seat layout templates available for this bus type. You can create a custom layout.
                </Alert>
              )}
            </CardContent>
          </Card>
        </Grid>

        {/* Seat Layout Preview */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              {renderSeatPreview()}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};

export default SeatLayoutConfig;