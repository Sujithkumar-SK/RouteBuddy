import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

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

export const adminRouteAPI = createApi({
  reducerPath: 'adminRouteAPI',
  baseQuery: fetchBaseQuery({
    baseUrl: '/api',
    prepareHeaders: (headers) => {
      const token = localStorage.getItem('token');
      if (token) {
        headers.set('authorization', `Bearer ${token}`);
      }
      return headers;
    },
  }),
  tagTypes: ['Route'],
  endpoints: (builder) => ({
    getAllRoutes: builder.query<{ data: Route[]; totalCount: number }, { pageNumber?: number; pageSize?: number }>({
      query: ({ pageNumber = 1, pageSize = 100 } = {}) => 
        `route?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      providesTags: ['Route'],
    }),
    getRouteAnalytics: builder.query<RouteAnalytics[], void>({
      query: () => 'admin/routes/analytics',
      providesTags: ['Route'],
    }),
  }),
});

export const { 
  useGetAllRoutesQuery, 
  useGetRouteAnalyticsQuery 
} = adminRouteAPI;