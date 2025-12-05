import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../features/auth/authSlice';
import customerProfileReducer from '../features/auth/customerProfileSlice';
import customerBookingsReducer from '../features/auth/customerBookingsSlice';
import busReducer from '../features/bus/busSlice';
import bookingReducer from '../features/booking/bookingSlice';
import vendorReducer from '../features/vendor/vendorSlice';
import adminReducer from '../features/admin/adminSlice';
import adminBusReducer from '../features/admin/adminBusSlice';
import adminBookingReducer from '../features/admin/adminBookingSlice';
import adminRouteReducer from '../features/admin/adminRouteSlice';
import busFleetReducer from '../features/vendor/busFleetSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    customerProfile: customerProfileReducer,
    customerBookings: customerBookingsReducer,
    bus: busReducer,
    booking: bookingReducer,
    vendor: vendorReducer,
    admin: adminReducer,
    adminBus: adminBusReducer,
    adminBooking: adminBookingReducer,
    adminRoute: adminRouteReducer,
    busFleet: busFleetReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
