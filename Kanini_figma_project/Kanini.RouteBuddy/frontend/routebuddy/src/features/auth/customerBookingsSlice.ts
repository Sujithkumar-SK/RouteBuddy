import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { customerBookingsAPI, type CustomerBooking, type CancelBookingRequest } from './customerBookingsAPI';

interface CustomerBookingsState {
  bookings: CustomerBooking[];
  loading: boolean;
  error: string | null;
  filters: {
    status?: number;
    fromDate?: string;
    toDate?: string;
  };
}

const initialState: CustomerBookingsState = {
  bookings: [],
  loading: false,
  error: null,
  filters: {},
};

export const fetchCustomerBookings = createAsyncThunk(
  'customerBookings/fetchBookings',
  async ({ status, fromDate, toDate }: { 
    status?: number; 
    fromDate?: string; 
    toDate?: string; 
  }, { rejectWithValue }) => {
    try {
      const response = await customerBookingsAPI.getBookings(status, fromDate, toDate);
      return response;
    } catch (error: any) {
      const errorMsg = error.response?.data?.error || error.message || 'Failed to fetch bookings';
      return rejectWithValue(errorMsg);
    }
  }
);

export const downloadTicket = createAsyncThunk(
  'customerBookings/downloadTicket',
  async ({ customerId, bookingId }: { customerId: number; bookingId: number }, { rejectWithValue }) => {
    try {
      const blob = await customerBookingsAPI.downloadTicket(customerId, bookingId);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `Ticket-${bookingId}.pdf`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
      return { success: true };
    } catch (error: any) {
      const errorMsg = error.response?.data?.error || error.message || 'Failed to download ticket';
      return rejectWithValue(errorMsg);
    }
  }
);

export const cancelBooking = createAsyncThunk(
  'customerBookings/cancelBooking',
  async ({ customerId, request }: { customerId: number; request: CancelBookingRequest }, { rejectWithValue }) => {
    try {
      const response = await customerBookingsAPI.cancelBooking(customerId, request);
      return { bookingId: request.bookingId, response };
    } catch (error: any) {
      const errorMsg = error.response?.data?.error || error.message || 'Failed to cancel booking';
      return rejectWithValue(errorMsg);
    }
  }
);

const customerBookingsSlice = createSlice({
  name: 'customerBookings',
  initialState,
  reducers: {
    setFilters: (state, action) => {
      state.filters = action.payload;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchCustomerBookings.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCustomerBookings.fulfilled, (state, action) => {
        state.loading = false;
        state.bookings = action.payload;
      })
      .addCase(fetchCustomerBookings.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(downloadTicket.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(downloadTicket.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(downloadTicket.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(cancelBooking.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(cancelBooking.fulfilled, (state, action) => {
        state.loading = false;
        // Update the booking status to cancelled
        const booking = state.bookings.find(b => b.bookingId === action.payload.bookingId);
        if (booking) {
          booking.status = 3; // Cancelled
        }
      })
      .addCase(cancelBooking.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { setFilters, clearError } = customerBookingsSlice.actions;
export default customerBookingsSlice.reducer;