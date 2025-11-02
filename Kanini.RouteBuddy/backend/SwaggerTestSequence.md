# Complete Swagger Testing Guide - Fresh Database

## 1. Search Buses
**POST** `/api/bus/search`
```json
{
  "source": "Chennai",
  "destination": "Bangalore",
  "travelDate": "2024-12-25"
}
```
**Expected:** Returns schedules with ID 1

## 2. Get Seat Layout
**GET** `/api/bus/1/seats?travelDate=2024-12-25`

**Expected:** Shows 40 seats, A1 and A2 already booked from seed data

## 3. Book Available Seats
**POST** `/api/bus/book`
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
**Expected:** Successful booking with PNR, busName, and route populated

## 4. Verify Booking
**GET** `/api/bus/1/seats?travelDate=2024-12-25`

**Expected:** A3 and A4 now show `"isBooked": true`, available seats decreased by 2

## 5. Test Future Date
**POST** `/api/bus/search`
```json
{
  "source": "Chennai", 
  "destination": "Bangalore",
  "travelDate": "2025-01-15"
}
```
**Expected:** Returns schedule ID 3

## 6. Book Future Date
**POST** `/api/bus/book`
```json
{
  "scheduleId": 3,
  "travelDate": "2025-01-15", 
  "seatNumbers": ["B1", "B2"],
  "passengers": [
    {
      "name": "Future User 1",
      "age": 28,
      "gender": 1
    },
    {
      "name": "Future User 2",
      "age": 32,
      "gender": 2
    }
  ],
  "customerId": 1
}
```

## Test Data Available:
- **Schedules:** 1 (Dec 25), 2 (Dec 30), 3 (Jan 15)
- **Customer ID:** 1
- **Pre-booked seats:** A1, A2 on schedule 1