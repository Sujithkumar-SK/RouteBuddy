import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { adminBusAPI, type AdminBus } from './adminBusAPI';

interface AdminBusState {
  buses: AdminBus[];
  pendingBuses: AdminBus[];
  loading: boolean;
  error: string | null;
}

const initialState: AdminBusState = {
  buses: [],
  pendingBuses: [],
  loading: false,
  error: null,
};

export const fetchAllBuses = createAsyncThunk(
  'adminBus/fetchAllBuses',
  async (_, { rejectWithValue }) => {
    try {
      return await adminBusAPI.getAllBuses();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch buses');
    }
  }
);

export const fetchPendingBuses = createAsyncThunk(
  'adminBus/fetchPendingBuses',
  async (_, { rejectWithValue }) => {
    try {
      return await adminBusAPI.getPendingBuses();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch pending buses');
    }
  }
);

const adminBusSlice = createSlice({
  name: 'adminBus',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchAllBuses.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchAllBuses.fulfilled, (state, action) => {
        state.loading = false;
        state.buses = action.payload;
      })
      .addCase(fetchAllBuses.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(fetchPendingBuses.fulfilled, (state, action) => {
        state.pendingBuses = action.payload;
      });
  },
});

export const { clearError } = adminBusSlice.actions;
export default adminBusSlice.reducer;