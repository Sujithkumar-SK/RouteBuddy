import api from '../../services/api';

export interface CustomerBooking {
  bookingId: number;
  pnrNo: string;
  totalSeats: number;
  totalAmount: number;
  travelDate: string;
  status: number;
  bookedAt: string;
  busName: string;
  route: string;
  vendorName: string;
  departureTime: string;
  arrivalTime: string;
  paymentMethod: number;
  isPaymentCompleted: boolean;
}

export interface CancelBookingRequest {
  bookingId: number;
  reason: string;
}

export interface CancelBookingResponse {
  isSuccess: boolean;
  message: string;
  refundAmount: number;
  penaltyAmount: number;
  refundMethod: string;
  processedAt: string;
}

export const customerBookingsAPI = {
  getBookings: async (status?: number, fromDate?: string, toDate?: string): Promise<CustomerBooking[]> => {
    const params = new URLSearchParams();
    if (status !== undefined) params.append('status', status.toString());
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    
    const response = await api.get(`/customer/profile/my-bookings?${params}`);
    return response.data;
  },

  downloadTicket: async (customerId: number, bookingId: number): Promise<Blob> => {
    const response = await api.get(`/customer/profile/${customerId}/bookings/${bookingId}/ticket`, {
      responseType: 'blob'
    });
    return response.data;
  },

  cancelBooking: async (customerId: number, request: CancelBookingRequest): Promise<CancelBookingResponse> => {
    const response = await api.post(`/customer/${customerId}/bookings/cancel`, request);
    return response.data;
  },
};