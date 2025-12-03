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
  getDashboardData: async () => {
    const response = await api.get('/vendor/dashboard');
    return response.data;
  },

  getAnalytics: async () => {
    const response = await api.get('/vendor/analytics');
    return response.data;
  },
};