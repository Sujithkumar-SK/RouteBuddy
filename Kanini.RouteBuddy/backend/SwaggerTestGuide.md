# Swagger Testing Guide for RouteBuddy API

## Prerequisites
1. Run the application: `dotnet run` in the API project
2. Navigate to: `https://localhost:7xxx/swagger` (check your launch settings)
3. Ensure database is seeded with test data

## Test Sequence

### 1. Search Buses
**Endpoint:** `POST /api/bus/search`

**Request Body:**
```json
{
  "source": "Chennai",
  "destination": "Bangalore",
  "travelDate": "2024-12-25"
}
```

**Expected Response:** List of available buses with scheduleId = 1

### 2. Get Seat Layout
**Endpoint:** `GET /api/bus/{scheduleId}/seats`

**Parameters:**
- scheduleId: `1`
- travelDate: `2024-12-25`

**Expected Response:** Seat layout with 40 seats, some may be booked

### 3. Book Seats
**Endpoint:** `POST /api/bus/book`

**Request Body:**
```json
{
  "scheduleId": 1,
  "travelDate": "2024-12-25",
  "seatNumbers": ["A3", "A4"],
  "passengers": [
    {
      "name": "Test User 1",
      "age": 25,
      "gender": 1
    },
    {
      "name": "Test User 2",
      "age": 30,
      "gender": 2
    }
  ],
  "customerId": 1
}
```

**Expected Response:** Booking confirmation with PNR

### 4. Verify Booking
Re-run the seat layout API to confirm seats A3 and A4 are now booked.

## Test Data Values
- **Sources/Destinations:** Chennai, Bangalore, Mumbai, Pune
- **Travel Dates:** 2024-12-25 (seeded), or future dates
- **Customer ID:** 1 (seeded)
- **Schedule ID:** 1 (seeded)
- **Available Seats:** A1-J4 (40 seats total)

## Error Testing
- Invalid dates (past dates)
- Non-existent scheduleId
- Already booked seats
- Invalid customer ID