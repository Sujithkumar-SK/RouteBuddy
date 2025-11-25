import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../features/auth/authSlice';
import busReducer from '../features/bus/busSlice';
import bookingReducer from '../features/booking/bookingSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    bus: busReducer,
    booking: bookingReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
