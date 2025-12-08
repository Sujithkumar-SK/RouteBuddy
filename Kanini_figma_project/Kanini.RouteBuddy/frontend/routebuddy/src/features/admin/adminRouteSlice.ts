import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import api from '../../services/api';

export interface Route {
  routeId: number;
  source: string;
  destination: string;
  distance: number;
  duration: string;
  basePrice: number;
  isActive: boolean;
  createdOn: string;
  createdBy: string;
}

export interface RouteAnalytics {
  routeId: number;
  source: string;
  destination: string;
  totalBookings: number;
  confirmedBookings: number;
  cancelledBookings: number;
  totalRevenue: number;
  averageBookingValue: number;
  successRate: number;
  isActive: boolean;
}

interface AdminRouteState {
  routes: Route[];
  routeAnalytics: RouteAnalytics[];
  loading: boolean;
  error: string | null;
}

const initialState: AdminRouteState = {
  routes: [],
  routeAnalytics: [],
  loading: false,
  error: null,
};

export const fetchAllRoutes = createAsyncThunk(
  'adminRoute/fetchAllRoutes',
  async (params: { pageNumber?: number; pageSize?: number } = {}) => {
    const { pageNumber = 1, pageSize = 100 } = params;
    const response = await api.get(`/route?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    return response.data;
  }
);

export const fetchRouteAnalytics = createAsyncThunk(
  'adminRoute/fetchRouteAnalytics',
  async () => {
    const response = await api.get('/admin/routes/analytics');
    return response.data;
  }
);

const adminRouteSlice = createSlice({
  name: 'adminRoute',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchAllRoutes.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchAllRoutes.fulfilled, (state, action) => {
        state.loading = false;
        state.routes = action.payload.data || action.payload;
      })
      .addCase(fetchAllRoutes.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch routes';
      })
      .addCase(fetchRouteAnalytics.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchRouteAnalytics.fulfilled, (state, action) => {
        state.loading = false;
        state.routeAnalytics = action.payload;
      })
      .addCase(fetchRouteAnalytics.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch route analytics';
      });
  },
});

export default adminRouteSlice.reducer;