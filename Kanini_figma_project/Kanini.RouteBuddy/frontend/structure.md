frontend/
├── public/
│   ├── index.html
│   ├── favicon.ico
│   └── assets/
│       └── images/
├── src/
│   ├── app/
│   │   ├── store.js                    # Redux store configuration
│   │   └── rootReducer.js              # Combine all reducers
│   │
│   ├── features/                       # Feature-based slices
│   │   ├── auth/
│   │   │   ├── authSlice.js           # Redux slice
│   │   │   ├── authAPI.js             # API calls
│   │   │   ├── Login.jsx
│   │   │   ├── Register.jsx
│   │   │   ├── ForgotPassword.jsx
│   │   │   └── OTPVerification.jsx
│   │   │
│   │   ├── bus/
│   │   │   ├── busSlice.js
│   │   │   ├── busAPI.js
│   │   │   ├── BusSearch.jsx
│   │   │   ├── BusResults.jsx
│   │   │   ├── SeatLayout.jsx
│   │   │   └── BusFilters.jsx
│   │   │
│   │   ├── booking/
│   │   │   ├── bookingSlice.js
│   │   │   ├── bookingAPI.js
│   │   │   ├── BookingForm.jsx
│   │   │   ├── BookingConfirmation.jsx
│   │   │   ├── PaymentPage.jsx
│   │   │   └── BookingTimer.jsx
│   │   │
│   │   ├── smartEngine/
│   │   │   ├── smartEngineSlice.js
│   │   │   ├── smartEngineAPI.js
│   │   │   ├── ConnectingRoutes.jsx
│   │   │   └── ConnectingBooking.jsx
│   │   │
│   │   ├── vendor/
│   │   │   ├── vendorSlice.js
│   │   │   ├── vendorAPI.js
│   │   │   ├── VendorDashboard.jsx
│   │   │   ├── BusManagement.jsx
│   │   │   └── ScheduleManagement.jsx
│   │   │
│   │   └── admin/
│   │       ├── adminSlice.js
│   │       ├── adminAPI.js
│   │       ├── AdminDashboard.jsx
│   │       ├── VendorApproval.jsx
│   │       └── BusApproval.jsx
│   │
│   ├── components/                     # Reusable components
│   │   ├── common/
│   │   │   ├── Button.jsx
│   │   │   ├── Input.jsx
│   │   │   ├── Card.jsx
│   │   │   ├── Modal.jsx
│   │   │   ├── Loader.jsx
│   │   │   └── ErrorBoundary.jsx
│   │   │
│   │   ├── layout/
│   │   │   ├── Header.jsx
│   │   │   ├── Footer.jsx
│   │   │   ├── Sidebar.jsx
│   │   │   └── Layout.jsx
│   │   │
│   │   └── ui/
│   │       ├── BusCard.jsx
│   │       ├── SeatButton.jsx
│   │       ├── RouteTimeline.jsx
│   │       └── PriceDisplay.jsx
│   │
│   ├── pages/                          # Page components
│   │   ├── HomePage.jsx
│   │   ├── SearchPage.jsx
│   │   ├── BookingPage.jsx
│   │   ├── MyBookingsPage.jsx
│   │   ├── ProfilePage.jsx
│   │   └── NotFoundPage.jsx
│   │
│   ├── routes/
│   │   ├── AppRoutes.jsx              # All routes
│   │   ├── PrivateRoute.jsx           # Protected routes
│   │   └── RoleBasedRoute.jsx         # Role-based access
│   │
│   ├── services/
│   │   ├── api.js                     # Axios instance
│   │   ├── apiEndpoints.js            # API URLs
│   │   └── interceptors.js            # Request/Response interceptors
│   │
│   ├── utils/
│   │   ├── constants.js               # Enums, constants
│   │   ├── helpers.js                 # Utility functions
│   │   ├── validators.js              # Form validations
│   │   └── formatters.js              # Date, price formatters
│   │
│   ├── hooks/
│   │   ├── useAuth.js
│   │   ├── useBooking.js
│   │   ├── useTimer.js
│   │   └── useDebounce.js
│   │
│   ├── theme/
│   │   ├── theme.js                   # MUI theme config
│   │   ├── colors.js
│   │   └── typography.js
│   │
│   ├── styles/
│   │   └── global.css
│   │
│   ├── App.jsx
│   ├── index.js
│   └── setupTests.js
│
├── .env
├── .env.example
├── .gitignore
├── package.json
└── README.md


Phase 1: Search Interface (Like RedBus)
┌─────────────────────────────────────────┐
│  🚌 ROUTEBUDDY - Book Your Journey     │
├─────────────────────────────────────────┤
│  From: [Mumbai ▼]                       │
│  To:   [Pune ▼]                         │
│  Date: [📅 15 Jan 2024]                 │
│        [🔍 SEARCH BUSES]                │
└─────────────────────────────────────────┘

Copy
Phase 2: Bus Listing (Search Results)
┌─────────────────────────────────────────┐
│  Filters:                               │
│  □ AC  □ Sleeper  □ Volvo              │
│  □ WiFi  □ Charging  □ Meals           │
│  Price: ₹500 ━━━━━━━━━ ₹2000          │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│  🚌 Shivneri Travels - Volvo AC        │
│  ⭐⭐⭐⭐ 4.5 | 35 seats available      │
│  🕐 06:00 AM → 09:30 AM (3h 30m)       │
│  💺 Seater | 📶 WiFi | 🔌 Charging     │
│  ₹850 onwards        [VIEW SEATS →]    │
└─────────────────────────────────────────┘

Copy
Phase 3: Seat Selection
┌─────────────────────────────────────────┐
│  Select Seats (Max 6)                   │
│  ┌─────────────────┐                    │
│  │ 🪟 [1] [2]  [3] [4] 🪟             │
│  │    [5] [6]  [7] [8]                │
│  │ 🪟 [9] [10] [11][12] 🪟            │
│  └─────────────────┘                    │
│  ✅ Available  ❌ Booked  🟦 Selected  │
│  Selected: 5, 6 | Total: ₹1700         │
└─────────────────────────────────────────┘

Copy
Phase 4: Boarding/Dropping Points
┌─────────────────────────────────────────┐
│  Boarding Point:                        │
│  ○ Dadar (06:00 AM)                    │
│  ● Thane (06:30 AM) ✓                  │
│                                         │
│  Dropping Point:                        │
│  ○ Pune Station (09:30 AM)             │
│  ● Hinjewadi (10:00 AM) ✓              │
└─────────────────────────────────────────┘

Copy
Phase 5: Passenger Details
┌─────────────────────────────────────────┐
│  Passenger 1 (Seat 5)                   │
│  Name: [____________]                   │
│  Age:  [__]  Gender: ○M ●F ○Other      │
│                                         │
│  Passenger 2 (Seat 6)                   │
│  Name: [____________]                   │
│  Age:  [__]  Gender: ●M ○F ○Other      │
└─────────────────────────────────────────┘

Copy
Phase 6: Payment & Confirmation
┌─────────────────────────────────────────┐
│  Booking Summary                        │
│  Bus: Shivneri Travels                  │
│  Route: Mumbai → Pune                   │
│  Date: 15 Jan 2024                      │
│  Seats: 5, 6                            │
│  Total: ₹1700                           │
│  [💳 PROCEED TO PAYMENT]                │
└────────────────────────────────