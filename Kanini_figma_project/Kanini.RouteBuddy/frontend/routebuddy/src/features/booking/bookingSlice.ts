import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { bookingAPI } from './bookingAPI';
import type { SeatSelectionState, PassengerDetails } from './types';

const initialState: SeatSelectionState = {
  seatLayout: null,
  routeStops: [],
  selectedSeats: [],
  boardingStopId: null,
  droppingStopId: null,
  passengers: [],
  totalPrice: 0,
  loading: false,
  error: null,
  bookingTimer: 0,
};

export const fetchSeatLayout = createAsyncThunk(
  'booking/fetchSeatLayout',
  async ({ scheduleId, travelDate }: { scheduleId: number; travelDate: string }) => {
    const response = await bookingAPI.getSeatLayout(scheduleId, travelDate);
    return response;
  }
);

export const fetchRouteStops = createAsyncThunk(
  'booking/fetchRouteStops',
  async (scheduleId: number) => {
    const response = await bookingAPI.getRouteStops(scheduleId);
    return response;
  }
);

export const createBooking = createAsyncThunk(
  'booking/createBooking',
  async (bookingData: any) => {
    const response = await bookingAPI.bookSeats(bookingData);
    return response;
  }
);

const bookingSlice = createSlice({
  name: 'booking',
  initialState,
  reducers: {
    toggleSeatSelection: (state, action: PayloadAction<string>) => {
      const seatNumber = action.payload;
      const index = state.selectedSeats.indexOf(seatNumber);
      
      if (index > -1) {
        state.selectedSeats.splice(index, 1);
        state.passengers.splice(index, 1);
      } else if (state.selectedSeats.length < 6) {
        state.selectedSeats.push(seatNumber);
        state.passengers.push({ name: '', age: 0, gender: 1 });
      }
      
      state.totalPrice = calculateTotalPrice(state);
    },
    
    setBoardingStop: (state, action: PayloadAction<number>) => {
      state.boardingStopId = action.payload;
    },
    
    setDroppingStop: (state, action: PayloadAction<number>) => {
      state.droppingStopId = action.payload;
    },
    
    updatePassenger: (state, action: PayloadAction<{ index: number; passenger: PassengerDetails }>) => {
      const { index, passenger } = action.payload;
      if (state.passengers[index]) {
        state.passengers[index] = passenger;
      }
    },
    
    clearBookingData: (state) => {
      state.selectedSeats = [];
      state.passengers = [];
      state.boardingStopId = null;
      state.droppingStopId = null;
      state.totalPrice = 0;
      state.bookingTimer = 0;
    },
    
    startBookingTimer: (state) => {
      state.bookingTimer = 600; // 10 minutes in seconds
    },
    
    decrementTimer: (state) => {
      if (state.bookingTimer > 0) {
        state.bookingTimer -= 1;
      }
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchSeatLayout.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchSeatLayout.fulfilled, (state, action) => {
        state.loading = false;
        state.seatLayout = action.payload;
      })
      .addCase(fetchSeatLayout.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch seat layout';
      })
      .addCase(fetchRouteStops.fulfilled, (state, action) => {
        state.routeStops = action.payload;
      })
      .addCase(createBooking.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createBooking.fulfilled, (state) => {
        state.loading = false;
        state.bookingTimer = 600; // Start 10-minute timer
      })
      .addCase(createBooking.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Booking failed';
      });
  },
});

const calculateTotalPrice = (state: SeatSelectionState): number => {
  if (!state.seatLayout) return 0;
  
  return state.selectedSeats.reduce((total, seatNumber) => {
    const seat = state.seatLayout!.seats.find(s => s.seatNumber === seatNumber);
    return total + (seat?.price || 0);
  }, 0);
};

export const {
  toggleSeatSelection,
  setBoardingStop,
  setDroppingStop,
  updatePassenger,
  clearBookingData,
  startBookingTimer,
  decrementTimer,
} = bookingSlice.actions;

export default bookingSlice.reducer;