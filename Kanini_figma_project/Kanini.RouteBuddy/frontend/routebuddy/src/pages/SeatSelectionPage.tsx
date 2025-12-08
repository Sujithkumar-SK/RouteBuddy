import { useEffect, useState } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import {
  Container,
  Grid,
  Stepper,
  Step,
  StepLabel,
  Button,
  Box,
  Typography,
  Alert,
  CircularProgress,
} from '@mui/material';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchSeatLayout, fetchRouteStops, clearBookingData, createBooking } from '../features/booking/bookingSlice';
import Layout from '../components/layout/Layout';
import SeatLayoutGrid from '../features/booking/SeatLayoutGrid';
import BoardingPointSelector from '../features/booking/BoardingPointSelector';
import PassengerForm from '../features/booking/PassengerForm';

const steps = ['Select Seats', 'Boarding & Dropping Points', 'Passenger Details', 'Review & Book'];

const SeatSelectionPage = () => {
  const { scheduleId } = useParams<{ scheduleId: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const dispatch = useAppDispatch();
  
  const [activeStep, setActiveStep] = useState(0);
  const { 
    seatLayout, 
    selectedSeats, 
    boardingStopId, 
    droppingStopId, 
    passengers, 
    totalPrice,
    loading, 
    error 
  } = useAppSelector((state) => state.booking);

  // Get travel date from location state or current search params
  const travelDate = location.state?.travelDate || new Date().toISOString().split('T')[0];

  useEffect(() => {
    if (scheduleId) {
      dispatch(fetchSeatLayout({ 
        scheduleId: parseInt(scheduleId), 
        travelDate 
      }));
      dispatch(fetchRouteStops(parseInt(scheduleId)));
    }

    return () => {
      dispatch(clearBookingData());
    };
  }, [dispatch, scheduleId, travelDate]);

  const handleNext = () => {
    setActiveStep((prevStep) => prevStep + 1);
  };

  const handleBack = () => {
    setActiveStep((prevStep) => prevStep - 1);
  };

  const canProceedToNext = () => {
    switch (activeStep) {
      case 0: // Seat Selection
        return selectedSeats.length > 0;
      case 1: // Boarding & Dropping Points
        return boardingStopId && droppingStopId;
      case 2: // Passenger Details
        return passengers.every(p => p.name && p.age > 0);
      default:
        return true;
    }
  };

  const handleBooking = async () => {
    try {
      // Create booking
      const bookingData = {
        scheduleId: parseInt(scheduleId!),
        travelDate,
        seatNumbers: selectedSeats,
        passengers,
        customerId: 1, // TODO: Get from auth state
        boardingStopId: boardingStopId!,
        droppingStopId: droppingStopId!,
      };

      const bookingResult = await dispatch(createBooking(bookingData));
      
      if (createBooking.fulfilled.match(bookingResult)) {
        // Navigate to payment page
        navigate(`/payment/${bookingResult.payload.bookingId}`, {
          state: {
            bookingDetails: {
              ...bookingResult.payload,
              route: `${seatLayout?.bus.busName}`,
              travelDate,
              seats: selectedSeats,
              passengerCount: passengers.length,
              totalAmount: totalPrice,
            }
          }
        });
      }
    } catch (error) {
      console.error('Booking failed:', error);
    }
  };

  const renderStepContent = () => {
    switch (activeStep) {
      case 0:
        return <SeatLayoutGrid />;
      case 1:
        return <BoardingPointSelector />;
      case 2:
        return <PassengerForm />;
      case 3:
        return (
          <Box sx={{ p: 3, textAlign: 'center' }}>
            <Typography variant="h6" sx={{ mb: 2 }}>
              Booking Summary
            </Typography>
            <Typography>Seats: {selectedSeats.join(', ')}</Typography>
            <Typography>Total Amount: ₹{totalPrice}</Typography>
            <Typography>Ready to book!</Typography>
          </Box>
        );
      default:
        return null;
    }
  };

  if (loading) {
    return (
      <Layout>
        <Container maxWidth="lg" sx={{ py: 4 }}>
          <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 200 }}>
            <CircularProgress />
            <Typography sx={{ ml: 2 }}>Loading seat layout...</Typography>
          </Box>
        </Container>
      </Layout>
    );
  }

  if (error) {
    return (
      <Layout>
        <Container maxWidth="lg" sx={{ py: 4 }}>
          <Alert severity="error">{error}</Alert>
        </Container>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="seat-selection-container">
        <Container maxWidth="lg" sx={{ py: 4 }}>
          {/* Header */}
          <div className="seat-selection-header">
            <h2 className="seat-selection-title">Select Seats</h2>
            {seatLayout && (
              <p className="seat-selection-info">
                {seatLayout.bus.busName} • {new Date(travelDate).toLocaleDateString()}
              </p>
            )}
          </div>

        {/* Stepper */}
        <div className="stepper-container">
          <Stepper activeStep={activeStep}>
            {steps.map((label) => (
              <Step key={label}>
                <StepLabel>{label}</StepLabel>
              </Step>
            ))}
          </Stepper>
        </div>

        {/* Content */}
        <Grid container spacing={3}>
          <Grid item xs={12}>
            {renderStepContent()}
          </Grid>
        </Grid>

        {/* Navigation */}
        <div className="navigation-buttons">
          <Button
            onClick={handleBack}
            disabled={activeStep === 0}
          >
            Back
          </Button>
          
          <div className="navigation-right">
            <Button
              variant="outlined"
              onClick={() => navigate('/search-results')}
            >
              Cancel
            </Button>
            
            {activeStep === steps.length - 1 ? (
              <Button
                variant="contained"
                onClick={handleBooking}
                disabled={!canProceedToNext()}
                className="payment-button"
              >
                Book Now
              </Button>
            ) : (
              <Button
                variant="contained"
                onClick={handleNext}
                disabled={!canProceedToNext()}
                className="search-button"
              >
                Next
              </Button>
            )}
          </div>
        </div>
      </Container>
      </div>
    </Layout>
  );
};

export default SeatSelectionPage;