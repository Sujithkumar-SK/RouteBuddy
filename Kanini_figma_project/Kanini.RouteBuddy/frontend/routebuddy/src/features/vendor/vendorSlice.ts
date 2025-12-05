import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { vendorAPI } from './vendorAPI';
import type {
  VendorDashboardSummary,
  VendorProfile,
  RevenueAnalytics,
  PerformanceMetrics,
  VendorFleetStatus,
  QuickStats,
  RecentBooking,
  VendorNotification,
  VendorAlert
} from './vendorAPI';

interface VendorState {
  dashboard: VendorDashboardSummary | null;
  profile: VendorProfile | null;
  revenueAnalytics: RevenueAnalytics | null;
  performanceMetrics: PerformanceMetrics | null;
  fleetStatus: VendorFleetStatus | null;
  quickStats: QuickStats | null;
  recentBookings: RecentBooking[];
  notifications: VendorNotification[];
  alerts: VendorAlert[];
  loading: {
    dashboard: boolean;
    profile: boolean;
    analytics: boolean;
  };
  error: string | null;
}

const initialState: VendorState = {
  dashboard: null,
  profile: null,
  revenueAnalytics: null,
  performanceMetrics: null,
  fleetStatus: null,
  quickStats: null,
  recentBookings: [],
  notifications: [],
  alerts: [],
  loading: {
    dashboard: false,
    profile: false,
    analytics: false,
  },
  error: null,
};

// Dashboard async thunk
export const fetchVendorDashboard = createAsyncThunk(
  'vendor/fetchDashboard',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getDashboardData();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch dashboard data');
    }
  }
);

// Analytics async thunk
export const fetchVendorAnalytics = createAsyncThunk(
  'vendor/fetchAnalytics',
  async (_, { rejectWithValue }) => {
    try {
      const response = await vendorAPI.getAnalytics();
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch analytics data');
    }
  }
);

const vendorSlice = createSlice({
  name: 'vendor',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    clearVendorData: (state) => {
      return initialState;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchVendorDashboard.pending, (state) => {
        state.loading.dashboard = true;
        state.error = null;
      })
      .addCase(fetchVendorDashboard.fulfilled, (state, action) => {
        state.loading.dashboard = false;
        state.dashboard = action.payload;
      })
      .addCase(fetchVendorDashboard.rejected, (state, action) => {
        state.loading.dashboard = false;
        state.error = action.payload as string;
      })
      .addCase(fetchVendorAnalytics.pending, (state) => {
        state.loading.analytics = true;
        state.error = null;
      })
      .addCase(fetchVendorAnalytics.fulfilled, (state, action) => {
        state.loading.analytics = false;
        const analytics = action.payload;
        state.revenueAnalytics = analytics.revenueAnalytics;
        state.performanceMetrics = analytics.performanceMetrics;
        state.fleetStatus = analytics.fleetStatus;
        state.quickStats = analytics.quickStats;
        state.recentBookings = analytics.recentBookings;
        state.notifications = analytics.notifications;
        state.alerts = analytics.alerts;
      })
      .addCase(fetchVendorAnalytics.rejected, (state, action) => {
        state.loading.analytics = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearError, clearVendorData } = vendorSlice.actions;
export default vendorSlice.reducer;