export const API_ENDPOINTS = {
  // Auth
  REGISTER_WITH_OTP: '/auth/register-with-otp',
  RESEND_REGISTRATION_OTP: '/auth/resend-registration-otp',
  VERIFY_REGISTRATION_OTP: '/auth/verify-registration-otp',
  LOGIN: '/auth/login',
  REFRESH_TOKEN: '/auth/refresh',
  LOGOUT: '/auth/logout',
  CHANGE_PASSWORD: '/auth/change-password',
  FORGOT_PASSWORD: '/auth/forgot-password',
  VERIFY_FORGOT_PASSWORD_OTP: '/auth/verify-forgot-password-otp',
  RESET_PASSWORD: '/auth/reset-password',
  COMPLETE_CUSTOMER_PROFILE: '/auth/complete-customer-profile',
  COMPLETE_VENDOR_PROFILE: '/auth/complete-vendor-profile',
  
  // Customer Profile
  GET_MY_PROFILE: '/customer/profile/my-profile',
  UPDATE_MY_PROFILE: '/customer/profile/my-profile',
  
  // Routes & Stops
  ROUTES: '/route',
  STOPS: '/stop',
};

