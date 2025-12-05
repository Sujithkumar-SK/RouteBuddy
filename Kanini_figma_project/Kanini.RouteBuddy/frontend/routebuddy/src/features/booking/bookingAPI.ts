import api from '../../services/api';
import type { SeatLayoutResponse, RouteStop, BookingRequest, BookingResponse } from './types';

export const bookingAPI = {
  getSeatLayout: async (scheduleId: number, travelDate: string): Promise<SeatLayoutResponse> => {
    const response = await api.get(`/Bus_Search_Book_/${scheduleId}/seats?travelDate=${travelDate}`);
    return response.data;
  },

  getRouteStops: async (scheduleId: number): Promise<RouteStop[]> => {
    const response = await api.get(`/Bus_Search_Book_/${scheduleId}/stops`);
    return response.data;
  },

  bookSeats: async (data: BookingRequest): Promise<BookingResponse> => {
    const response = await api.post('/Bus_Search_Book_/book', data);
    return response.data;
  },

  confirmBooking: async (bookingId: number, paymentData: any): Promise<{ message: string }> => {
    const response = await api.post(`/Bus_Search_Book_/book/${bookingId}/confirm`, paymentData);
    return response.data;
  },

  // Payment endpoints
  initiatePayment: async (data: any): Promise<any> => {
    const response = await api.post('/payment/initiate', data);
    return response.data;
  },

  verifyPayment: async (data: any): Promise<any> => {
    const response = await api.post('/payment/verify', data);
    return response.data;
  },

  getPaymentHistory: async (bookingId: number): Promise<any[]> => {
    const response = await api.get(`/payment/booking/${bookingId}`);
    return response.data;
  },
};