# RouteBuddy API Documentation

## Overview
RouteBuddy is a comprehensive bus booking system API that provides endpoints for bus search, seat booking, connecting routes, and smart routing capabilities.

**Base URL:** `https://localhost:7xxx/api`  
**API Version:** v1  
**Content-Type:** `application/json`
**Database:** SQL Server with Entity Framework Core
**Authentication:** Currently not implemented (planned for future releases)

---

## 🚌 Bus Controller Endpoints

### 1. Search Buses
**Endpoint:** `POST /api/bus/search`  
**Purpose:** Search for direct buses between source and destination on a specific date
**Implementation:** Uses stored procedure `sp_SearchBuses` for optimized database queries

#### Request Body
```json
{
  "source": "Chennai",
  "destination": "Bangalore", 
  "travelDate": "2025-12-25T00:00:00.000Z"
}
```

#### Validation Rules
- Source: Required, 2-100 characters
- Destination: Required, 2-100 characters  
- TravelDate: Required, must be valid DateTime

#### Response (200 OK)
```json
[
  {
    "scheduleId": 1,
    "busId": 1,
    "busName": "Express Deluxe",
    "busType": 1,
    "totalSeats": 40,
    "availableSeats": 38,
    "source": "Chennai",
    "destination": "Bangalore",
    "travelDate": "2025-12-25T00:00:00",
    "departureTime": "06:00:00",
    "arrivalTime": "12:00:00",
    "basePrice": 500.00,
    "amenities": 3,
    "vendorName": "Express Travels"
  }
]
```

#### BusType Enum Values
- 1=AC, 2=NonAC, 3=Sleeper, 4=SemiSleeper, 5=Volvo, 6=Luxury

#### Amenities Enum (Flags)
- 1=AC, 2=WiFi, 4=Charging, 8=Blanket, 16=Pillow, 32=Entertainment, 64=Snacks, 128=WashRoom

#### Error Responses
- **400 Bad Request:** Invalid request data or validation errors
- **500 Internal Server Error:** Unexpected server error

---

### 2. Get Seat Layout
**Endpoint:** `GET /api/bus/{scheduleId}/seats?travelDate={date}`  
**Purpose:** Retrieve seat layout and availability for a specific bus schedule
**Implementation:** Uses stored procedure `sp_GetBusSeatLayout` with real-time availability

#### Parameters
- `scheduleId` (path): Bus schedule ID (required, > 0)
- `travelDate` (query): Travel date in ISO format (required)

#### Example Request
```
GET /api/bus/1/seats?travelDate=2025-12-25T00:00:00.000Z
```

#### Response (200 OK)
```json
{
  "scheduleId": 1,
  "busName": "Express Deluxe",
  "totalSeats": 40,
  "availableSeats": 38,
  "seats": [
    {
      "seatNumber": "A1",
      "seatType": 1,
      "seatPosition": 1,
      "priceTier": 2,
      "isAvailable": true,
      "price": 550.00
    },
    {
      "seatNumber": "A2", 
      "seatType": 1,
      "seatPosition": 2,
      "priceTier": 1,
      "isAvailable": false,
      "price": 500.00
    }
  ]
}
```

#### Seat Enums
- **SeatType:** 1=Seater, 2=SleeperLower, 3=SleeperUpper, 4=SemiSleeper
- **SeatPosition:** 1=Window, 2=Aisle, 3=Middle  
- **PriceTier:** 1=Base, 2=Premium, 3=Luxury

---

### 3. Get Route Stops
**Endpoint:** `GET /api/bus/{scheduleId}/stops`  
**Purpose:** Get all boarding and dropping stops for a bus route

#### Parameters
- `scheduleId` (path): Bus schedule ID

#### Response (200 OK)
```json
[
  {
    "stopId": 1,
    "stopName": "Chennai Central",
    "landmark": "Railway Station",
    "orderNumber": 1,
    "arrivalTime": null,
    "departureTime": "06:00:00"
  },
  {
    "stopId": 3,
    "stopName": "Trichy", 
    "landmark": "Central Bus Stand",
    "orderNumber": 2,
    "arrivalTime": "09:00:00",
    "departureTime": "09:15:00"
  }
]
```

---

### 4. Book Seats
**Endpoint:** `POST /api/bus/book`  
**Purpose:** Book seats on a bus (creates pending booking for 10 minutes)
**Implementation:** Uses stored procedures `sp_ValidateSeatsAndStops` and creates booking with auto-expiry

#### Request Body
```json
{
  "scheduleId": 1,
  "customerId": 1,
  "boardingStopId": 1,
  "droppingStopId": 3,
  "travelDate": "2025-12-25T00:00:00.000Z",
  "seatNumbers": ["A1", "A2"],
  "passengers": [
    {
      "name": "John Doe",
      "age": 30,
      "gender": 1
    },
    {
      "name": "Jane Doe", 
      "age": 28,
      "gender": 2
    }
  ]
}
```

#### Validation Rules
- ScheduleId: Required, > 0
- CustomerId: Required, > 0
- BoardingStopId: Required, > 0
- DroppingStopId: Required, > 0
- SeatNumbers: Required, minimum 1 seat
- Passengers: Required, must match seat count
- TravelDate: Required, valid DateTime

#### Gender Enum
- 1=Male, 2=Female, 3=Other

#### Response (200 OK)
```json
{
  "bookingId": 123,
  "pnr": "PNR789012",
  "totalAmount": 1100.00,
  "expiresAt": "2025-12-25T10:15:00Z",
  "status": "Pending"
}
```

#### Business Rules
- Booking expires in 10 minutes if not confirmed
- Seat numbers must match passenger count
- Boarding stop must come before dropping stop in route

---

### 5. Confirm Booking
**Endpoint:** `POST /api/bus/book/{bookingId}/confirm`  
**Purpose:** Confirm a pending booking with payment details
**Implementation:** Uses stored procedure `sp_ConfirmBooking` and triggers email notification

#### Request Body
```json
{
  "bookingId": 123,
  "paymentReferenceId": "PAY_123456789",
  "paymentMethod": 2
}
```

#### Validation Rules
- BookingId: Must match URL parameter
- PaymentReferenceId: Required string
- PaymentMethod: Required enum value

#### Response (200 OK)
```json
{
  "message": "Booking confirmed successfully"
}
```

#### Payment Methods Enum
- 1=Mock, 2=UPI, 3=Card, 4=NetBanking

#### Post-Confirmation Actions
- Booking status updated to Confirmed
- Payment record created
- Email confirmation sent asynchronously
- PDF ticket generated

---

### 6. Search Buses with Filters
**Endpoint:** `POST /api/bus/search/filtered`  
**Purpose:** Advanced bus search with filters and sorting options
**Implementation:** Uses stored procedure `sp_SearchBusesFiltered` with comprehensive filtering

#### Request Body
```json
{
  "source": "Chennai",
  "destination": "Bangalore",
  "travelDate": "2025-12-25T00:00:00.000Z",
  "departureTimeFrom": "06:00:00",
  "departureTimeTo": "18:00:00", 
  "minPrice": 300.00,
  "maxPrice": 800.00,
  "busTypes": [1, 3],
  "amenities": [1, 2],
  "sortBy": "price"
}
```

#### Filter Options (All Optional)
- **DepartureTimeFrom/To:** Time range filtering
- **MinPrice/MaxPrice:** Price range filtering
- **BusTypes:** Array of bus type enums
- **Amenities:** Array of amenity flags
- **SortBy:** "price", "departure", "duration", "rating"

#### Filter Options
- **BusTypes:** 1=AC, 2=NonAC, 3=Sleeper, 4=SemiSleeper, 5=Volvo, 6=Luxury
- **Amenities:** 1=AC, 2=WiFi, 4=Charging, 8=Blanket, 16=Pillow, etc.
- **SortBy:** "price", "departure", "duration", "rating"

---

## 🧠 Smart Engine Controller Endpoints

### 1. Find Connecting Routes
**Endpoint:** `POST /api/smartengine/connecting-routes`  
**Purpose:** Find connecting bus routes when no direct buses available
**Implementation:** Uses stored procedure `sp_FindConnectingRoutes` with intelligent route optimization

#### Request Body
```json
{
  "source": "Chennai",
  "destination": "Mumbai", 
  "travelDate": "2025-12-25T00:00:00.000Z",
  "toggle": "cheapest"
}
```

#### Validation Rules
- Source: Required, 2-100 characters
- Destination: Required, 2-100 characters
- TravelDate: Required, valid DateTime
- Toggle: Optional, defaults to "cheapest"

#### Response (200 OK)
```json
[
  {
    "routeId": "CONN_001",
    "totalPrice": 850.00,
    "totalDuration": 720,
    "segments": [
      {
        "scheduleId": 1,
        "busName": "Express Deluxe",
        "source": "Chennai",
        "destination": "Bangalore", 
        "departureTime": "06:00:00",
        "arrivalTime": "12:00:00",
        "price": 500.00,
        "availableSeats": 20
      },
      {
        "scheduleId": 5,
        "busName": "City Express",
        "source": "Bangalore", 
        "destination": "Mumbai",
        "departureTime": "14:00:00",
        "arrivalTime": "22:00:00", 
        "price": 350.00,
        "availableSeats": 15
      }
    ]
  }
]
```

#### Toggle Options
- **"cheapest":** Sort by total price (ascending)
- **"fastest":** Sort by total duration (ascending)

#### Business Rules
- Minimum 1-hour buffer between connecting buses
- Both segments must have available seats
- Same travel date for all segments

---

### 2. Book Connecting Route
**Endpoint:** `POST /api/smartengine/book-connecting-route`  
**Purpose:** Book seats across multiple connecting bus segments
**Implementation:** Uses stored procedure `sp_BookConnectingRoute` with atomic transaction handling

#### Request Body
```json
{
  "customerId": 1,
  "travelDate": "2025-12-25T00:00:00.000Z",
  "segments": [
    {
      "scheduleId": 1,
      "boardingStopId": 1,
      "droppingStopId": 2,
      "seatNumbers": ["A1", "A2"]
    },
    {
      "scheduleId": 5, 
      "boardingStopId": 3,
      "droppingStopId": 4,
      "seatNumbers": ["B1", "B2"]
    }
  ],
  "passengers": [
    {
      "name": "John Doe",
      "age": 30, 
      "gender": 1
    },
    {
      "name": "Jane Doe",
      "age": 28,
      "gender": 2  
    }
  ]
}
```

#### Validation Rules
- CustomerId: Required, > 0
- TravelDate: Required, valid DateTime
- Segments: Required, minimum 1 segment
- Passengers: Required, must match total seat count across all segments
- Each segment must have valid scheduleId, stop IDs, and seat numbers

#### Response (200 OK)
```json
{
  "bookingId": 456,
  "pnr": "CONN789012", 
  "totalAmount": 850.00,
  "expiresAt": "2025-12-25T10:15:00Z",
  "segmentCount": 2,
  "status": "Pending"
}
```

---

### 3. Confirm Connecting Booking
**Endpoint:** `POST /api/smartengine/book-connecting-route/{bookingId}/confirm`  
**Purpose:** Confirm a pending connecting route booking
**Implementation:** Confirms all segments atomically and triggers smart email notifications

#### Request Body
```json
{
  "bookingId": 456,
  "paymentReferenceId": "PAY_987654321", 
  "paymentMethod": 2
}
```

#### Response (200 OK)
```json
{
  "message": "Connecting route booking confirmed successfully"
}
```

#### Post-Confirmation Actions
- All booking segments confirmed atomically
- Payment record created for total amount
- Smart connecting booking email sent
- PDF tickets generated for each segment

---

## 📋 Common Response Patterns

### Success Response Structure
All successful responses follow consistent patterns with appropriate HTTP status codes (200, 201).

### Error Response Structure
```json
{
  "error": {
    "code": "VALIDATION_FAILED",
    "description": "Source is required",
    "type": "Failure"
  }
}
```

### Common Error Codes
- **VALIDATION_FAILED:** Request validation errors
- **NOT_FOUND:** Resource not found (schedule, booking, etc.)
- **SEATS_NOT_AVAILABLE:** Selected seats are not available
- **BOOKING_EXPIRED:** Booking reservation has expired
- **DATABASE_ERROR:** Database operation failed
- **INVALID_ROUTE_STOPS:** Boarding/dropping stop validation failed
- **INSUFFICIENT_SEATS:** Not enough available seats
- **PAYMENT_FAILED:** Payment processing error

### HTTP Status Codes Used
- **200 OK:** Successful operation
- **400 Bad Request:** Validation errors or business rule violations
- **404 Not Found:** Resource not found
- **500 Internal Server Error:** Unexpected server errors

---

## 🔄 Business Workflows

### Standard Booking Flow
1. **Search Buses** → Get available buses using `sp_SearchBuses`
2. **Get Seat Layout** → Show seat map using `sp_GetBusSeatLayout`  
3. **Book Seats** → Create pending booking with `sp_ValidateSeatsAndStops`
4. **Confirm Booking** → Complete payment using `sp_ConfirmBooking`
5. **Email Notification** → Automatic booking confirmation email

### Connecting Route Flow  
1. **Find Connecting Routes** → Get multi-segment options using `sp_FindConnectingRoutes`
2. **Book Connecting Route** → Reserve seats across segments using `sp_BookConnectingRoute`
3. **Confirm Connecting Booking** → Complete payment and confirm all segments
4. **Smart Email** → Specialized connecting route confirmation email

### Auto-Expiry System
- Pending bookings automatically expire after 10 minutes
- Background service `BookingExpiryService` runs every 2 minutes
- Uses stored procedure `sp_ExpirePendingBookings` for cleanup
- Expired bookings release reserved seats back to inventory

### Email & PDF Services
- **EmailService:** Standard booking confirmations
- **SmartEmailService:** Connecting route confirmations
- **PdfService:** Individual ticket generation
- **SmartPdfService:** Multi-segment ticket generation
- All email services use HTML templates with booking details

---

## 🎯 Frontend Integration Tips

### State Management
- Track booking expiry timers for pending bookings (10-minute countdown)
- Cache seat layouts to avoid repeated API calls
- Implement optimistic UI updates for better UX
- Store search filters for better user experience

### Error Handling
- Display user-friendly messages for common errors
- Implement retry logic for network failures
- Show loading states during API calls
- Handle validation errors from ModelState

### Performance Optimization
- Debounce search inputs to reduce API calls
- Implement pagination for large result sets
- Use skeleton loaders for better perceived performance
- Cache route stops data for repeated bookings

### Real-time Features
- Implement booking countdown timers (10 minutes)
- Show real-time seat selection conflicts
- Consider polling for seat availability updates
- Display connecting route buffer times clearly

### Data Formatting
- All DateTime fields use ISO format with timezone
- Enum values are integers (refer to documentation for mappings)
- Price fields are decimal with 2 decimal places
- Time fields use HH:mm:ss format

---

## 🛠️ Development Information

### Database Setup
1. Run `Execute_This_SQL_First.sql` for initial setup
2. Apply Entity Framework migrations
3. Seed data is automatically applied via `RouteBuddyDatabaseContext`

### Testing
- Use `SwaggerTestGuide.md` for API testing instructions
- Follow `SwaggerTestSequence.md` for end-to-end testing
- Unit tests available in `Kanini.RouteBuddy.UnitTests` project

### Logging
- File-based logging using Serilog
- Log files stored in `Logs/` directory
- Structured logging with magic strings for consistency

### Email Configuration
- SMTP settings in `appsettings.json`
- HTML templates: `BookingConfirmationEmail.html`, `ConnectingBookingConfirmationEmail.html`
- Asynchronous email sending to avoid blocking API responses

## 📞 Support Information

For technical support or API questions, contact the development team.

**Last Updated:** December 2024  
**API Version:** 1.0  
**Framework:** .NET 8  
**Database:** SQL Server with Entity Framework Core 9.0.8