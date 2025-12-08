import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { adminBookingAPI, type AdminBooking, type BookingStatusSummary } from './adminBookingAPI';

interface AdminBookingState {
  bookings: AdminBooking[];
  statusSummary: BookingStatusSummary | null;
  loading: boolean;
  error: string | null;
}

const initialState: AdminBookingState = {
  bookings: [],
  statusSummary: null,
  loading: false,
  error: null,
};

export const fetchAllBookings = createAsyncThunk(
  'adminBooking/fetchAllBookings',
  async (_, { rejectWithValue }) => {
    try {
      return await adminBookingAPI.getAllBookings();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch bookings');
    }
  }
);

export const fetchBookingStatusSummary = createAsyncThunk(
  'adminBooking/fetchBookingStatusSummary',
  async (_, { rejectWithValue }) => {
    try {
      return await adminBookingAPI.getBookingStatusSummary();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch booking status summary');
    }
  }
);

const adminBookingSlice = createSlice({
  name: 'adminBooking',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchAllBookings.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchAllBookings.fulfilled, (state, action) => {
        state.loading = false;
        state.bookings = action.payload;
      })
      .addCase(fetchAllBookings.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(fetchBookingStatusSummary.fulfilled, (state, action) => {
        state.statusSummary = action.payload;
      });
  },
});

export const { clearError } = adminBookingSlice.actions;
export default adminBookingSlice.reducer;