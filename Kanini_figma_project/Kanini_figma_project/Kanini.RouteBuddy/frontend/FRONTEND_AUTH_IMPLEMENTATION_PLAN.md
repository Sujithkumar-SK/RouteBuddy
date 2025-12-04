# Frontend Authentication Implementation Plan

## 📋 Requirements Analysis

### User Flow:
1. **Signup** → Email OTP Verification → Role-based Profile Completion
   - Customer → Customer Profile Page
   - Vendor → Vendor Profile Page
2. **Login** → Dashboard (role-based)

### Features Needed:
- ✅ Unified Signup (Customer + Vendor)
- ✅ Email OTP Verification
- ✅ Customer Profile Completion
- ✅ Vendor Profile Completion
- ✅ Login with JWT + Refresh Token
- ✅ Role-based Routing

---

## 🏗️ Architecture

### Tech Stack:
- React 19 + TypeScript
- Redux Toolkit (state management)
- Material-UI (components)
- React Router (routing)
- Axios (API calls)

### Folder Structure:
```
src/
├── features/auth/
│   ├── authSlice.ts              # Redux slice
│   ├── authAPI.ts                # API calls
│   ├── Signup.tsx                # Unified signup
│   ├── OTPVerification.tsx       # Email OTP
│   ├── Login.tsx                 # Login page
│   ├── CustomerProfile.tsx       # Customer details
│   └── VendorProfile.tsx         # Vendor details
├── services/
│   ├── api.ts                    # Axios instance
│   └── apiEndpoints.ts           # API URLs
├── utils/
│   ├── constants.ts              # Enums
│   └── validators.ts             # Validations
└── routes/
    └── AppRoutes.tsx             # All routes
```

---

## 📝 Implementation Steps

### Step 1: Install Dependencies
```bash
npm install @reduxjs/toolkit react-redux
npm install @mui/material @mui/icons-material @emotion/react @emotion/styled
npm install react-router-dom
npm install axios
npm install react-hook-form yup @hookform/resolvers
```

### Step 2: Setup Redux Store
- Configure store
- Create auth slice

### Step 3: Setup API Service
- Axios instance with interceptors
- Refresh token logic
- API endpoints

### Step 4: Create Auth Components
- Signup (unified for Customer/Vendor)
- OTP Verification
- Login
- Customer Profile
- Vendor Profile

### Step 5: Setup Routing
- Public routes
- Protected routes
- Role-based routes

---

## 🔄 User Flows

### Signup Flow:
```
1. User visits /signup
2. Selects role (Customer/Vendor)
3. Enters: Email, Password, Phone
4. Clicks "Send OTP"
5. Backend sends OTP to email
6. User enters OTP on /verify-otp
7. Backend verifies OTP
8. If Customer → Redirect to /customer-profile
9. If Vendor → Redirect to /vendor-profile
10. User completes profile
11. Redirect to dashboard
```

### Login Flow:
```
1. User visits /login
2. Enters: Email, Password
3. Backend returns: accessToken, refreshToken, role
4. Store tokens securely
5. If Customer → Redirect to /customer-dashboard
6. If Vendor → Redirect to /vendor-dashboard
7. If Admin → Redirect to /admin-dashboard
```

---

## 🔐 Security Implementation

### Token Storage:
- **Access Token**: Memory only (not localStorage)
- **Refresh Token**: localStorage (httpOnly cookie better but needs backend change)

### Auto-Refresh Logic:
```typescript
// Axios interceptor
response.interceptor(error => {
  if (error.response.status === 401) {
    // Call /api/token/refresh
    // Retry original request
  }
})
```

---

## 📊 State Management

### Auth Slice State:
```typescript
{
  user: {
    userId: number,
    email: string,
    role: 'Customer' | 'Vendor' | 'Admin',
    isEmailVerified: boolean
  },
  tokens: {
    accessToken: string,
    refreshToken: string,
    accessTokenExpiresAt: string,
    refreshTokenExpiresAt: string
  },
  loading: boolean,
  error: string | null
}
```

---

## 🎨 UI Components Needed

### Material-UI Components:
- TextField (email, password, phone)
- Button (primary, secondary)
- Select (role selection)
- Alert (error/success messages)
- CircularProgress (loading)
- Stepper (signup progress)
- Card (form containers)

---

## ✅ Validation Rules

### Signup:
- Email: Required, valid email format
- Password: Required, min 8 chars, 1 uppercase, 1 number, 1 special char
- Phone: Required, 10 digits, starts with 6-9
- Role: Required (Customer/Vendor)

### OTP:
- OTP: Required, 6 digits

### Customer Profile:
- FirstName: Required, 2-50 chars
- LastName: Required, 2-50 chars
- DateOfBirth: Required, age 18+
- Gender: Required (Male/Female/Other)

### Vendor Profile:
- AgencyName: Required, 3-100 chars
- OwnerName: Required, 2-100 chars
- BusinessLicenseNumber: Required
- OfficeAddress: Required
- FleetSize: Required, number > 0
- TaxRegistrationNumber: Required

---

## 🚀 Ready to Implement!

**Status:** Planning Complete
**Next:** Install dependencies and start coding
