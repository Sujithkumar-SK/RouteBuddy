import api from '../../services/api';
import { API_ENDPOINTS } from '../../services/apiEndpoints';

export interface VendorDashboardSummary {
  totalBuses: number;
  activeBuses: number;
  pendingBuses: number;
  totalRoutes: number;
  totalSchedules: number;
  upcomingSchedules: number;
  vendorStatus: string;
  lastUpdated: string;
}

export interface VendorProfile {
  vendorId: number;
  agencyName: string;
  ownerName: string;
  businessLicenseNumber: string;
  officeAddress: string;
  fleetSize: number;
  taxRegistrationNumber?: string;
  status: string;
  isActive: boolean;
  email: string;
  phone: string;
}

export interface RevenueAnalytics {
  totalRevenue: number;
  monthlyRevenue: number;
  weeklyRevenue: number;
}

export interface PerformanceMetrics {
  monthlyBookings: number;
  onTimePerformance: number;
}

export interface VendorFleetStatus {
  totalBuses: number;
  activeBuses: number;
  maintenanceBuses: number;
  idleBuses: number;
}

export interface QuickStats {
  totalBookings: number;
  activeRoutes: number;
  totalRevenue: number;
}

export interface RecentBooking {
  bookingId: number;
  customerName: string;
  route: string;
  bookingDate: string;
  status: string;
}

export interface VendorNotification {
  type: string;
  message: string;
  time: string;
}

export interface VendorAlert {
  alertId: number;
  type: string;
  message: string;
  severity: string;
  createdAt: string;
}

export const vendorAPI = {
  getDashboardSummary: async (): Promise<VendorDashboardSummary> => {
    const response = await api.get('/api/vendor/dashboard');
    return response.data.data;
  },

  getProfile: async (): Promise<VendorProfile> => {
    const response = await api.get('/api/vendor/me');
    return response.data.data;
  },

  updateProfile: async (profileData: Partial<VendorProfile>): Promise<VendorProfile> => {
    const response = await api.put('/api/vendor/profile', profileData);
    return response.data.data;
  },

  getRevenueAnalytics: async (): Promise<RevenueAnalytics> => {
    const response = await api.get('/api/vendor/revenue-analytics');
    return response.data.data;
  },

  getPerformanceMetrics: async (): Promise<PerformanceMetrics> => {
    const response = await api.get('/api/vendor/performance');
    return response.data.data;
  },

  getFleetStatus: async (): Promise<VendorFleetStatus> => {
    const response = await api.get('/api/vendor/fleet-status');
    return response.data.data;
  },

  getQuickStats: async (): Promise<QuickStats> => {
    const response = await api.get('/api/vendor/quick-stats');
    return response.data.data;
  },

  getRecentBookings: async (): Promise<RecentBooking[]> => {
    const response = await api.get('/api/vendor/recent-bookings');
    return response.data.data;
  },

  getNotifications: async (): Promise<VendorNotification[]> => {
    const response = await api.get('/api/vendor/notifications');
    return response.data.data;
  },

  getAlerts: async (): Promise<VendorAlert[]> => {
    const response = await api.get('/api/vendor/alerts');
    return response.data.data;
  },
};