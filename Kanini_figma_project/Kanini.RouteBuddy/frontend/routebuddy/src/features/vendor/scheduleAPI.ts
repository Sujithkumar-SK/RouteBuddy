import api from '../../services/api';

export interface VendorSchedule {
  scheduleId: number;
  busId: number;
  busName: string;
  routeId: number;
  source: string;
  destination: string;
  travelDate: string;
  departureTime: string;
  arrivalTime: string;
  availableSeats: number;
  status: number;
  isActive: boolean;
}

export interface RouteSearch {
  routeId: number;
  source: string;
  destination: string;
  distance: number;
  duration: string;
  basePrice: number;
  totalStops: number;
  isActive: boolean;
}

export interface CreateScheduleRequest {
  busId: number;
  routeId: number;
  travelDate: string;
  departureTime: string;
  arrivalTime: string;
}

export interface CreateBulkScheduleRequest {
  busId: number;
  routeId: number;
  startDate: string;
  endDate: string;
  departureTime: string;
  arrivalTime: string;
  operatingDays: number[]; // 0=Sunday, 1=Monday, etc.
}

export interface CreateRouteStopRequest {
  stopId: number;
  orderNumber: number;
  arrivalTime?: string;
  departureTime?: string;
}

export const scheduleAPI = {
  getMySchedules: async (pageNumber = 1, pageSize = 10) => {
    const response = await api.get('/vendor/schedules', {
      params: { pageNumber, pageSize }
    });
    return response.data;
  },

  getAllRoutes: async () => {
    const response = await api.get('/vendor/schedules/routes');
    return response.data;
  },

  getAvailableStops: async () => {
    const response = await api.get('/vendor/schedules/available-stops');
    return response.data;
  },

  createRouteStops: async (routeId: number, stops: CreateRouteStopRequest[]) => {
    const response = await api.post(`/vendor/schedules/routes/${routeId}/stops`, stops);
    return response.data;
  },

  createScheduleStops: async (scheduleId: number, stops: CreateRouteStopRequest[]) => {
    const response = await api.post(`/vendor/schedules/${scheduleId}/stops`, stops);
    return response.data;
  },

  getRouteStops: async (routeId: number) => {
    const response = await api.get(`/vendor/schedules/routes/${routeId}/stops`);
    return response.data;
  },

  createSchedule: async (data: CreateScheduleRequest) => {
    const response = await api.post('/vendor/schedules', data);
    return response.data;
  },

  getSchedule: async (scheduleId: number) => {
    const response = await api.get(`/vendor/schedules/${scheduleId}`);
    return response.data;
  },

  createBulkSchedule: async (data: CreateBulkScheduleRequest) => {
    const response = await api.post('/vendor/schedules/bulk', data);
    return response.data;
  }
};