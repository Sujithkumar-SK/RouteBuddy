import api from '../../services/api';

export interface AdminBus {
  busId: number;
  busName: string;
  busType: number;
  totalSeats: number;
  registrationNo: string;
  status: number;
  amenities: number;
  driverName?: string;
  driverContact?: string;
  isActive: boolean;
  vendorId: number;
  vendorName: string;
  createdAt: string;
}

export const adminBusAPI = {
  getAllBuses: async () => {
    const response = await api.get('/admin/buses');
    return response.data;
  },

  getPendingBuses: async () => {
    const response = await api.get('/admin/buses/pending');
    return response.data;
  },
};