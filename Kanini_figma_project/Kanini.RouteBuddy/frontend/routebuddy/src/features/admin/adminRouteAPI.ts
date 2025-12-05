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

export interface CreateRouteRequest {
  source: string;
  destination: string;
  distance: number;
  duration: string;
  basePrice: number;
}

export interface UpdateRouteRequest {
  source: string;
  destination: string;
  distance: number;
  duration: string;
  basePrice: number;
}

export interface PagedRouteResponse {
  data: Route[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface Stop {
  stopId: number;
  name: string;
  landmark?: string;
  isActive: boolean;
  createdOn: string;
}

export interface CreateStopRequest {
  name: string;
  landmark?: string;
}

export interface UpdateStopRequest {
  name: string;
  landmark?: string;
}

export interface PagedStopResponse {
  data: Stop[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export const adminRouteAPI = {
  getAllRoutes: async (pageNumber = 1, pageSize = 10): Promise<PagedRouteResponse> => {
    const response = await api.get(`/route?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    return response.data;
  },

  getRouteById: async (id: number): Promise<Route> => {
    const response = await api.get(`/route/${id}`);
    return response.data;
  },

  createRoute: async (data: CreateRouteRequest): Promise<Route> => {
    const response = await api.post('/route', data);
    return response.data;
  },

  updateRoute: async (id: number, data: UpdateRouteRequest): Promise<Route> => {
    const response = await api.put(`/route/${id}`, data);
    return response.data;
  },

  deleteRoute: async (id: number): Promise<void> => {
    await api.delete(`/route/${id}`);
  },

  getAllStops: async (pageNumber = 1, pageSize = 10): Promise<PagedStopResponse> => {
    const response = await api.get(`/stop?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    return response.data;
  },

  getStopById: async (id: number): Promise<Stop> => {
    const response = await api.get(`/stop/${id}`);
    return response.data;
  },

  createStop: async (data: CreateStopRequest): Promise<Stop> => {
    const response = await api.post('/stop', data);
    return response.data;
  },

  updateStop: async (id: number, data: UpdateStopRequest): Promise<Stop> => {
    const response = await api.put(`/stop/${id}`, data);
    return response.data;
  },

  deleteStop: async (id: number): Promise<void> => {
    await api.delete(`/stop/${id}`);
  }
};