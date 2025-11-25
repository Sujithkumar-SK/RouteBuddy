import { Routes, Route, Navigate } from 'react-router-dom';
import { useAppSelector } from '../hooks/useAppDispatch';
import { ROUTES } from '../utils/constants';

// Auth
import Signup from '../features/auth/Signup';
import OTPVerification from '../features/auth/OTPVerification';
import Login from '../features/auth/Login';
import CustomerProfile from '../features/auth/CustomerProfile';
import VendorProfile from '../features/auth/VendorProfile';

// Pages
import HomePage from '../pages/HomePage';
import CustomerDashboard from '../pages/CustomerDashboard';
import VendorDashboard from '../pages/VendorDashboard';
import SearchResultsPage from '../pages/SearchResultsPage';
import SeatSelectionPage from '../pages/SeatSelectionPage';
import PaymentPage from '../pages/PaymentPage';
import PaymentSuccessPage from '../pages/PaymentSuccessPage';

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated } = useAppSelector((state) => state.auth);
  return isAuthenticated ? <>{children}</> : <Navigate to={ROUTES.LOGIN} />;
};

const AppRoutes = () => {
  return (
    <Routes>
      {/* Public Routes */}
      <Route path={ROUTES.HOME} element={<HomePage />} />
      <Route path={ROUTES.LOGIN} element={<Login />} />
      <Route path={ROUTES.SIGNUP} element={<Signup />} />
      <Route path={ROUTES.VERIFY_OTP} element={<OTPVerification />} />
      <Route path={ROUTES.CUSTOMER_PROFILE} element={<CustomerProfile />} />
      <Route path={ROUTES.VENDOR_PROFILE} element={<VendorProfile />} />
      <Route path={ROUTES.SEARCH_RESULTS} element={<SearchResultsPage />} />
      <Route path="/seat-selection/:scheduleId" element={<SeatSelectionPage />} />
      <Route path="/payment/:bookingId" element={<PaymentPage />} />
      <Route path="/payment-success" element={<PaymentSuccessPage />} />

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

      {/* Fallback */}
      <Route path="*" element={<Navigate to={ROUTES.LOGIN} />} />
    </Routes>
  );
};

export default AppRoutes;
