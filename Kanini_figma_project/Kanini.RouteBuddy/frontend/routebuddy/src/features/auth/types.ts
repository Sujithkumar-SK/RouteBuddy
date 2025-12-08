export interface RegisterWithOtpRequest {
  email: string;
  password: string;
  phone: string;
  role: number; // 1=Customer, 2=Vendor
  recaptchaToken: string;
}

export interface ResendOtpRequest {
  email: string;
  password: string;
  phone: string;
  role: number;
}

export interface CompleteCustomerProfileRequest {
  firstName: string;
  middleName?: string;
  lastName: string;
  dateOfBirth: string;
  gender: number;
}

export interface CompleteVendorProfileRequest {
  agencyName: string;
  ownerName: string;
  businessLicenseNumber: string;
  officeAddress: string;
  fleetSize: number;
  taxRegistrationNumber?: string;
}

export interface VerifyRegistrationOtpRequest {
  email: string;
  otp: string;
  otpToken: string;
  role: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  userId: number;
  email: string;
  role: string;
  accessToken: string;
  refreshToken: string;
  accessTokenExpiry: string;
  refreshTokenExpiry: string;
  message: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface OtpResponse {
  email: string;
  otpToken: string;
  expiresAt: string;
  message: string;
}

export interface RegistrationResponse {
  userId: number;
  email: string;
  role: string;
  customerId?: number;
  requiresVendorProfile: boolean;
  message: string;
}

export interface ProfileCompletionResponse {
  message: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface VerifyForgotPasswordOtpRequest {
  email: string;
  otp: string;
  otpToken: string;
}

export interface ResetPasswordRequest {
  email: string;
  newPassword: string;
}
