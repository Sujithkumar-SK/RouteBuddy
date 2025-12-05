export const UserRole = {
  Customer: 1,
  Vendor: 2,
  Admin: 3,
} as const;

export const Gender = {
  Male: 1,
  Female: 2,
  Other: 3,
} as const;

export const BusType = {
  AC: 1,
  NonAC: 2,
  Sleeper: 3,
  SemiSleeper: 4,
  Volvo: 5,
  Luxury: 6,
} as const;

export const BusAmenities = {
  None: 0,
  AC: 1,
  WiFi: 2,
  Charging: 4,
  Blanket: 8,
  Pillow: 16,
  Entertainment: 32,
  Snacks: 64,
  WashRoom: 128,
} as const;

export const API_BASE_URL = 'http://localhost:5172/api';
export const ROUTES = {
  HOME: '/',
  LOGIN: '/login',
  SIGNUP: '/signup',
  VERIFY_OTP: '/verify-otp',
  FORGOT_PASSWORD: '/forgot-password',
  VERIFY_FORGOT_OTP: '/verify-forgot-otp',
  RESET_PASSWORD: '/reset-password',
  CUSTOMER_PROFILE: '/customer-profile',
  VENDOR_PROFILE: '/vendor-profile',
  CUSTOMER_DASHBOARD: '/customer/dashboard',
  VENDOR_DASHBOARD: '/vendor/dashboard',
  ADMIN_DASHBOARD: '/admin/dashboard',
  SEARCH_RESULTS: '/search-results',
  PROFILE: '/profile',
  MY_BOOKINGS: '/my-bookings',
  // Vendor routes
  VENDOR_FLEET: '/vendor/fleet',
  VENDOR_SCHEDULES: '/vendor/schedules',
  VENDOR_ANALYTICS: '/vendor/analytics',
  // Admin routes
  ADMIN_VENDORS: '/admin/vendors',
  ADMIN_BUSES: '/admin/buses',
  ADMIN_BOOKINGS: '/admin/bookings',
  ADMIN_ROUTES: '/admin/routes',
  ADMIN_USERS: '/admin/users',
  ADMIN_REPORTS: '/admin/reports',
  ADMIN_SETTINGS: '/admin/settings',
};

export const getBusTypeName = (type: number): string => {
  const typeMap = {
    [BusType.AC]: 'AC',
    [BusType.NonAC]: 'Non-AC',
    [BusType.Sleeper]: 'Sleeper',
    [BusType.SemiSleeper]: 'Semi-Sleeper',
    [BusType.Volvo]: 'Volvo',
    [BusType.Luxury]: 'Luxury',
  };
  return typeMap[type] || 'Unknown';
};

export const getAmenityNames = (amenities: number): string[] => {
  const names: string[] = [];
  if (amenities & BusAmenities.AC) names.push('AC');
  if (amenities & BusAmenities.WiFi) names.push('WiFi');
  if (amenities & BusAmenities.Charging) names.push('Charging');
  if (amenities & BusAmenities.Blanket) names.push('Blanket');
  if (amenities & BusAmenities.Pillow) names.push('Pillow');
  if (amenities & BusAmenities.Entertainment) names.push('Entertainment');
  if (amenities & BusAmenities.Snacks) names.push('Snacks');
  if (amenities & BusAmenities.WashRoom) names.push('Washroom');
  return names;
};
