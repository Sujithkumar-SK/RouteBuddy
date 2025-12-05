import { Routes, Route, Navigate } from 'react-router-dom';
import { useEffect } from 'react';
import { useAppSelector, useAppDispatch } from '../hooks/useAppDispatch';
import { checkAuthState } from '../features/auth/authSlice';
import { ROUTES } from '../utils/constants';

// Auth
import Signup from '../features/auth/Signup';
import OTPVerification from '../features/auth/OTPVerification';
import Login from '../features/auth/Login';
import ForgotPassword from '../features/auth/ForgotPassword';
import VerifyForgotOtp from '../features/auth/VerifyForgotOtp';
import ResetPassword from '../features/auth/ResetPassword';
import CustomerProfile from '../features/auth/CustomerProfile';
import VendorProfile from '../features/auth/VendorProfile';

// Pages
import HomePage from '../pages/HomePage';
import CustomerDashboard from '../pages/CustomerDashboard';
import VendorDashboard from '../pages/VendorDashboard';
import AdminDashboard from '../pages/AdminDashboard';
import AdminVendorsPage from '../pages/AdminVendorsPage';
import AdminBusesPage from '../pages/AdminBusesPage';
import AdminBookingsPage from '../pages/AdminBookingsPage';
import AdminRoutesPage from '../pages/AdminRoutesPage';
import AdminUsersPage from '../pages/AdminUsersPage';
import SearchResultsPage from '../pages/SearchResultsPage';
import SeatSelectionPage from '../pages/SeatSelectionPage';
import PaymentPage from '../pages/PaymentPage';
import PaymentSuccessPage from '../pages/PaymentSuccessPage';
import ProfilePage from '../pages/ProfilePage';
import MyBookingsPage from '../pages/MyBookingsPage';
import BusFleetManagementPage from '../pages/BusFleetManagementPage';
import VendorAnalyticsPage from '../pages/VendorAnalyticsPage';
import VendorSchedulesPage from '../pages/VendorSchedulesPage';
import EditBusFormPage from '../features/vendor/components/EditBusFormPage';
import BusPhotoManagerPage from '../features/vendor/components/BusPhotoManagerPage';


const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated, user } = useAppSelector((state) => state.auth);
  
  // Check if we have both authentication flag and user data
  const isActuallyAuthenticated = isAuthenticated && user && user.userId;
  
  return isActuallyAuthenticated ? <>{children}</> : <Navigate to={ROUTES.LOGIN} replace />;
};

const AdminProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated, user } = useAppSelector((state) => state.auth);
  
  // Check if user is authenticated and has admin role
  const isActuallyAuthenticated = isAuthenticated && user && user.userId;
  const isAdmin = user?.role === 'Admin';
  
  if (!isActuallyAuthenticated) {
    return <Navigate to={ROUTES.LOGIN} replace />;
  }
  
  if (!isAdmin) {
    return <Navigate to={ROUTES.HOME} replace />;
  }
  
  return <>{children}</>;
};

const AppRoutes = () => {
  const dispatch = useAppDispatch();
  
  useEffect(() => {
    // Check auth state on app initialization
    dispatch(checkAuthState());
  }, [dispatch]);
  
  return (
    <Routes>
      {/* Public Routes */}
      <Route path={ROUTES.HOME} element={<HomePage />} />
      <Route path={ROUTES.LOGIN} element={<Login />} />
      <Route path={ROUTES.SIGNUP} element={<Signup />} />
      <Route path={ROUTES.VERIFY_OTP} element={<OTPVerification />} />
      <Route path={ROUTES.FORGOT_PASSWORD} element={<ForgotPassword />} />
      <Route path={ROUTES.VERIFY_FORGOT_OTP} element={<VerifyForgotOtp />} />
      <Route path={ROUTES.RESET_PASSWORD} element={<ResetPassword />} />
      <Route path={ROUTES.CUSTOMER_PROFILE} element={<CustomerProfile />} />
      <Route path={ROUTES.VENDOR_PROFILE} element={<VendorProfile />} />
      <Route path={ROUTES.SEARCH_RESULTS} element={<SearchResultsPage />} />
      <Route path="/seat-selection/:scheduleId" element={<SeatSelectionPage />} />
      <Route path="/payment/:bookingId" element={<PaymentPage />} />
      <Route path="/payment-success" element={<PaymentSuccessPage />} />
      <Route
        path={ROUTES.PROFILE}
        element={
          <ProtectedRoute>
            <ProfilePage />
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.MY_BOOKINGS}
        element={
          <ProtectedRoute>
            <MyBookingsPage />
          </ProtectedRoute>
        }
      />

      {/* Protected Routes */}
      <Route
        path={ROUTES.CUSTOMER_DASHBOARD}
        element={
          <ProtectedRoute>
            <CustomerDashboard />
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.VENDOR_DASHBOARD}
        element={
          <ProtectedRoute>
            <VendorDashboard />
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.ADMIN_DASHBOARD}
        element={
          <AdminProtectedRoute>
            <AdminDashboard />
          </AdminProtectedRoute>
        }
      />
      <Route
        path={ROUTES.ADMIN_VENDORS}
        element={
          <AdminProtectedRoute>
            <AdminVendorsPage />
          </AdminProtectedRoute>
        }
      />
      <Route
        path={ROUTES.ADMIN_BUSES}
        element={
          <AdminProtectedRoute>
            <AdminBusesPage />
          </AdminProtectedRoute>
        }
      />
      <Route
        path={ROUTES.ADMIN_BOOKINGS}
        element={
          <AdminProtectedRoute>
            <AdminBookingsPage />
          </AdminProtectedRoute>
        }
      />
      <Route
        path={ROUTES.ADMIN_ROUTES}
        element={
          <AdminProtectedRoute>
            <AdminRoutesPage />
          </AdminProtectedRoute>
        }
      />
      <Route
        path={ROUTES.ADMIN_USERS}
        element={
          <AdminProtectedRoute>
            <AdminUsersPage />
          </AdminProtectedRoute>
        }
      />
      <Route
        path={ROUTES.VENDOR_FLEET}
        element={
          <ProtectedRoute>
            <BusFleetManagementPage />
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.VENDOR_SCHEDULES}
        element={
          <ProtectedRoute>
            <VendorSchedulesPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/vendor/fleet/:tab/:busId?"
        element={
          <ProtectedRoute>
            <BusFleetManagementPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/vendor/fleet/edit/:busId"
        element={
          <ProtectedRoute>
            <EditBusFormPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/vendor/fleet/photos/:busId"
        element={
          <ProtectedRoute>
            <BusPhotoManagerPage />
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.VENDOR_ANALYTICS}
        element={
          <ProtectedRoute>
            <VendorAnalyticsPage />
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.ADMIN_REPORTS}
        element={
          <AdminProtectedRoute>
            <AdminDashboard />
          </AdminProtectedRoute>
        }
      />
      <Route
        path={ROUTES.ADMIN_SETTINGS}
        element={
          <AdminProtectedRoute>
            <AdminDashboard />
          </AdminProtectedRoute>
        }
      />

      {/* Fallback */}
      <Route path="*" element={<Navigate to={ROUTES.LOGIN} />} />
    </Routes>
  );
};

export default AppRoutes;
