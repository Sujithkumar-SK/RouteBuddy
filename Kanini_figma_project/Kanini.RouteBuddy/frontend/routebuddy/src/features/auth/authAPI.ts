import api from '../../services/api';
import { API_ENDPOINTS } from '../../services/apiEndpoints';
import type {
  RegisterWithOtpRequest,
  ResendOtpRequest,
  VerifyRegistrationOtpRequest,
  LoginRequest,
} from './types';

export const authAPI = {
  registerWithOtp: async (data: RegisterWithOtpRequest) => {
    const response = await api.post(API_ENDPOINTS.REGISTER_WITH_OTP, data);
    return response.data;
  },

  resendRegistrationOtp: async (data: ResendOtpRequest) => {
    const response = await api.post(API_ENDPOINTS.RESEND_REGISTRATION_OTP, data);
    return response.data;
  },

  verifyRegistrationOtp: async (data: VerifyRegistrationOtpRequest) => {
    const response = await api.post(API_ENDPOINTS.VERIFY_REGISTRATION_OTP, data);
    return response.data;
  },

  login: async (data: LoginRequest) => {
    const response = await api.post(API_ENDPOINTS.LOGIN, data);
    return response.data;
  },

  logout: async () => {
    const response = await api.post(API_ENDPOINTS.LOGOUT);
    return response.data;
  },

  completeCustomerProfile: async (userId: number, data: any) => {
    const response = await api.post(`${API_ENDPOINTS.COMPLETE_CUSTOMER_PROFILE}/${userId}`, data);
    return response.data;
  },

  completeVendorProfile: async (userId: number, data: FormData) => {
    const response = await api.post(`${API_ENDPOINTS.COMPLETE_VENDOR_PROFILE}/${userId}`, data, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },
};
