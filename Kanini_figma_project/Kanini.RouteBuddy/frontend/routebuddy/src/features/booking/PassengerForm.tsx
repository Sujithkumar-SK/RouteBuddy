import {
  Paper,
  Typography,
  TextField,
  FormControl,
  FormLabel,
  RadioGroup,
  FormControlLabel,
  Radio,
  Box,
  Grid,
} from '@mui/material';
import { useAppSelector, useAppDispatch } from '../../hooks/useAppDispatch';
import { updatePassenger } from './bookingSlice';
import { Gender } from '../../utils/constants';

const PassengerForm = () => {
  const dispatch = useAppDispatch();
  const { selectedSeats, passengers } = useAppSelector((state) => state.booking);

  const handlePassengerChange = (index: number, field: string, value: string | number) => {
    const updatedPassenger = {
      ...passengers[index],
      [field]: value,
    };
    dispatch(updatePassenger({ index, passenger: updatedPassenger }));
  };

  if (selectedSeats.length === 0) {
    return (
      <Paper sx={{ p: 3, textAlign: 'center' }}>
        <Typography color="text.secondary">
          Please select seats to enter passenger details
        </Typography>
      </Paper>
    );
  }

  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h6" sx={{ mb: 3 }}>
        Passenger Details
      </Typography>

      <Grid container spacing={3}>
        {selectedSeats.map((seatNumber, index) => (
          <Grid item xs={12} md={6} key={seatNumber}>
            <Box sx={{ p: 2, border: 1, borderColor: 'divider', borderRadius: 1 }}>
              <Typography variant="subtitle1" sx={{ mb: 2, fontWeight: 600 }}>
                Passenger {index + 1} (Seat {seatNumber})
              </Typography>

              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                <TextField
                  fullWidth
                  label="Full Name"
                  value={passengers[index]?.name || ''}
                  onChange={(e) => handlePassengerChange(index, 'name', e.target.value)}
                  required
                  placeholder="Enter full name as per ID"
                />

                <TextField
                  fullWidth
                  label="Age"
                  type="number"
                  value={passengers[index]?.age || ''}
                  onChange={(e) => handlePassengerChange(index, 'age', parseInt(e.target.value) || 0)}
                  required
                  inputProps={{ min: 1, max: 120 }}
                />

                <FormControl component="fieldset">
                  <FormLabel component="legend">Gender</FormLabel>
                  <RadioGroup
                    row
                    value={passengers[index]?.gender || Gender.Male}
                    onChange={(e) => handlePassengerChange(index, 'gender', parseInt(e.target.value))}
                  >
                    <FormControlLabel
                      value={Gender.Male}
                      control={<Radio />}
                      label="Male"
                    />
                    <FormControlLabel
                      value={Gender.Female}
                      control={<Radio />}
                      label="Female"
                    />
                    <FormControlLabel
                      value={Gender.Other}
                      control={<Radio />}
                      label="Other"
                    />
                  </RadioGroup>
                </FormControl>
              </Box>
            </Box>
          </Grid>
        ))}
      </Grid>

      {/* Validation Summary */}
      <Box sx={{ mt: 3, p: 2, bgcolor: 'info.50', borderRadius: 1 }}>
        <Typography variant="body2" color="info.main">
          💡 Please ensure all passenger details are accurate as per government ID
        </Typography>
      </Box>
    </Paper>
  );
};

export default PassengerForm;