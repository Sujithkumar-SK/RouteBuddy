import api from '../../services/api';

export interface AdminBooking {
  bookingId: number;
  pnrNo: string;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  totalSeats: number;
  totalAmount: number;
  travelDate: string;
  status: number;
  bookedAt: string;
  busName: string;
  route: string;
  paymentStatus: number;
}

export interface BookingStatusSummary {
  pendingBookings: number;
  confirmedBookings: number;
  cancelledBookings: number;
  totalBookings: number;
  totalRevenue: number;
  successfulPayments: number;
  pendingPayments: number;
}

export const adminBookingAPI = {
  getAllBookings: async (): Promise<AdminBooking[]> => {
    const response = await api.get('/admin/bookings');
    return response.data;
  },

  getBookingStatusSummary: async (): Promise<BookingStatusSummary> => {
    const response = await api.get('/admin/bookings/status-summary');
    return response.data;
  },

  filterBookings: async (searchName?: string, status?: number, fromDate?: string, toDate?: string): Promise<AdminBooking[]> => {
    const params = new URLSearchParams();
    if (searchName) params.append('searchName', searchName);
    if (status !== undefined) params.append('status', status.toString());
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    
    const response = await api.get(`/admin/bookings/filter?${params}`);
    return response.data;
  },

  getBookingById: async (bookingId: number): Promise<AdminBooking> => {
    const response = await api.get(`/admin/bookings/${bookingId}`);
    return response.data;
  },
};