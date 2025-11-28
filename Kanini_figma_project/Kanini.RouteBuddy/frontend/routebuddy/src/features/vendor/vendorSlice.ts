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

// Async thunks
export const fetchDashboardSummary = createAsyncThunk(
  'vendor/fetchDashboardSummary',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getDashboardSummary();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch dashboard summary');
    }
  }
);

export const fetchVendorProfile = createAsyncThunk(
  'vendor/fetchProfile',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getProfile();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch profile');
    }
  }
);

export const fetchRevenueAnalytics = createAsyncThunk(
  'vendor/fetchRevenueAnalytics',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getRevenueAnalytics();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch revenue analytics');
    }
  }
);

export const fetchPerformanceMetrics = createAsyncThunk(
  'vendor/fetchPerformanceMetrics',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getPerformanceMetrics();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch performance metrics');
    }
  }
);

export const fetchFleetStatus = createAsyncThunk(
  'vendor/fetchFleetStatus',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getFleetStatus();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch fleet status');
    }
  }
);

export const fetchQuickStats = createAsyncThunk(
  'vendor/fetchQuickStats',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getQuickStats();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch quick stats');
    }
  }
);

export const fetchRecentBookings = createAsyncThunk(
  'vendor/fetchRecentBookings',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getRecentBookings();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch recent bookings');
    }
  }
);

export const fetchNotifications = createAsyncThunk(
  'vendor/fetchNotifications',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getNotifications();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch notifications');
    }
  }
);

export const fetchAlerts = createAsyncThunk(
  'vendor/fetchAlerts',
  async (_, { rejectWithValue }) => {
    try {
      return await vendorAPI.getAlerts();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch alerts');
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
    // Dashboard Summary
    builder
      .addCase(fetchDashboardSummary.pending, (state) => {
        state.loading.dashboard = true;
        state.error = null;
      })
      .addCase(fetchDashboardSummary.fulfilled, (state, action) => {
        state.loading.dashboard = false;
        state.dashboard = action.payload;
      })
      .addCase(fetchDashboardSummary.rejected, (state, action) => {
        state.loading.dashboard = false;
        state.error = action.payload as string;
      })

    // Profile
      .addCase(fetchVendorProfile.pending, (state) => {
        state.loading.profile = true;
        state.error = null;
      })
      .addCase(fetchVendorProfile.fulfilled, (state, action) => {
        state.loading.profile = false;
        state.profile = action.payload;
      })
      .addCase(fetchVendorProfile.rejected, (state, action) => {
        state.loading.profile = false;
        state.error = action.payload as string;
      })

    // Analytics
      .addCase(fetchRevenueAnalytics.pending, (state) => {
        state.loading.analytics = true;
      })
      .addCase(fetchRevenueAnalytics.fulfilled, (state, action) => {
        state.loading.analytics = false;
        state.revenueAnalytics = action.payload;
      })
      .addCase(fetchRevenueAnalytics.rejected, (state, action) => {
        state.loading.analytics = false;
        state.error = action.payload as string;
      })

      .addCase(fetchPerformanceMetrics.fulfilled, (state, action) => {
        state.performanceMetrics = action.payload;
      })
      .addCase(fetchFleetStatus.fulfilled, (state, action) => {
        state.fleetStatus = action.payload;
      })
      .addCase(fetchQuickStats.fulfilled, (state, action) => {
        state.quickStats = action.payload;
      })
      .addCase(fetchRecentBookings.fulfilled, (state, action) => {
        state.recentBookings = action.payload;
      })
      .addCase(fetchNotifications.fulfilled, (state, action) => {
        state.notifications = action.payload;
      })
      .addCase(fetchAlerts.fulfilled, (state, action) => {
        state.alerts = action.payload;
      });
  },
});

export const { clearError, clearVendorData } = vendorSlice.actions;
export default vendorSlice.reducer;