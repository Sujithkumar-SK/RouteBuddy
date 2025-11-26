import axios from 'axios';
import { authService } from '../Services/authService';
import { cookieUtils } from './cookies';

axios.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = cookieUtils.getRefreshToken();
        
        if (refreshToken) {
          const response = await authService.refreshToken(refreshToken);
          
          cookieUtils.setToken(response.accessToken);
          cookieUtils.setRefreshToken(response.refreshToken);
          
          originalRequest.headers.Authorization = `Bearer ${response.accessToken}`;
          return axios(originalRequest);
        }
      } catch (refreshError) {
        cookieUtils.removeToken();
        cookieUtils.removeRefreshToken();
        cookieUtils.removeUser();
        window.location.href = '/login';
      }
    }

    return Promise.reject(error);
  }
);
