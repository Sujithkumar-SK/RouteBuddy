# RouteBuddy - Epic Features & User Stories Documentation

**Project:** RouteBuddy Bus Booking System  
**Version:** 1.0  
**Date:** December 2024  
**Author:** Senior Developer  
**Document Type:** Epic Features & User Stories for Azure DevOps

---

## Table of Contents

1. [Epic 1: Core Bus Booking System](#epic-1-core-bus-booking-system)
2. [Epic 2: Smart Engine - Connecting Routes](#epic-2-smart-engine---connecting-routes)
3. [Epic 3: Communication & Notifications](#epic-3-communication--notifications)
4. [Epic 4: System Administration](#epic-4-system-administration)
5. [Technical Acceptance Criteria](#technical-acceptance-criteria)
6. [Definition of Done](#definition-of-done)

---

## Epic 1: Core Bus Booking System

### Feature 1: Bus Search & Discovery

#### US-BS-001: Search Buses by Route and Date
**Priority:** High  
**Story Points:** 8  
**Sprint:** 1

**Description:**
As a passenger, I want to search for buses between source and destination on a specific travel date, so that I can find available buses for my journey.

**Acceptance Criteria:**

**AC1: Valid bus search**
- **Scenario:** Search with valid route and date
- **Given:** I enter "Chennai" as Source, "Bangalore" as Destination, and "2025-01-15" as Travel Date
- **When:** I click "Search Buses"
- **Then:** I should see a list of available buses with details (bus name, departure time, price, available seats)

**AC2: Date validation**
- **Scenario:** Search with past date
- **Given:** I enter a travel date in the past
- **When:** I click "Search Buses"
- **Then:** I should see error: "Travel date cannot be in the past"

**AC3: No buses available**
- **Scenario:** Search with no available buses
- **Given:** I search for a route with no buses on selected date
- **When:** I click "Search Buses"
- **Then:** I should see message: "No buses available for this route on selected date"

**API Endpoint:** `POST /api/bus/search`

---

#### US-BS-002: Advanced Bus Search with Filters
**Priority:** Medium  
**Story Points:** 13  
**Sprint:** 2

**Description:**
As a passenger, I want to filter bus search results by price, departure time, bus type, and amenities, so that I can find buses matching my preferences.

**Acceptance Criteria:**

**AC1: Price range filter**
- **Scenario:** Filter by price range
- **Given:** I have search results displayed
- **When:** I set price range from ₹300 to ₹800
- **Then:** Only buses within this price range should be displayed

**AC2: Time range filter**
- **Scenario:** Filter by departure time
- **Given:** I have search results displayed
- **When:** I set departure time from 06:00 to 18:00
- **Then:** Only buses departing within this time range should be displayed

**AC3: Bus type filter**
- **Scenario:** Filter by bus type
- **Given:** I have search results displayed
- **When:** I select "AC" and "Sleeper" bus types
- **Then:** Only AC and Sleeper buses should be displayed

**AC4: Amenities filter**
- **Scenario:** Filter by amenities
- **Given:** I have search results displayed
- **When:** I select "WiFi" and "Charging" amenities
- **Then:** Only buses with WiFi and Charging facilities should be displayed

**AC5: Sort functionality**
- **Scenario:** Sort search results
- **Given:** I have filtered search results
- **When:** I select sort by "Price", "Departure Time", "Duration", or "Rating"
- **Then:** Results should be sorted accordingly

**API Endpoint:** `POST /api/bus/search/filtered`

---

### Feature 2: Seat Selection & Layout

#### US-SL-001: View Bus Seat Layout
**Priority:** High  
**Story Points:** 8  
**Sprint:** 1

**Description:**
As a passenger, I want to view the seat layout of a selected bus, so that I can choose my preferred seats.

**Acceptance Criteria:**

**AC1: Display seat layout**
- **Scenario:** View seat layout for selected bus
- **Given:** I select a bus from search results
- **When:** I click "Select Seats"
- **Then:** I should see the bus seat layout with available/booked seat indicators

**AC2: Seat availability status**
- **Scenario:** Check seat availability
- **Given:** I am viewing the seat layout
- **Then:** Available seats should be shown in green, booked seats in red, and selected seats in blue

**AC3: Seat pricing tiers**
- **Scenario:** View seat prices
- **Given:** I am viewing the seat layout
- **When:** I hover over any seat
- **Then:** I should see the seat price based on its tier (Base/Premium/Luxury)

**AC4: Seat information display**
- **Scenario:** View seat details
- **Given:** I am viewing the seat layout
- **Then:** Each seat should show seat number, type (Window/Aisle/Middle), and seat category (Seater/Sleeper)

**API Endpoint:** `GET /api/bus/{scheduleId}/seats?travelDate={date}`

---

#### US-SL-002: Select and Book Seats
**Priority:** High  
**Story Points:** 13  
**Sprint:** 1

**Description:**
As a passenger, I want to select seats and provide passenger details, so that I can reserve my seats on the bus.

**Acceptance Criteria:**

**AC1: Seat selection**
- **Scenario:** Select available seats
- **Given:** I am viewing the seat layout
- **When:** I click on available seats
- **Then:** Selected seats should be highlighted and added to my selection

**AC2: Seat deselection**
- **Scenario:** Deselect seats
- **Given:** I have selected seats
- **When:** I click on selected seats again
- **Then:** Seats should be deselected and removed from my selection

**AC3: Passenger details entry**
- **Scenario:** Enter passenger information
- **Given:** I have selected seats
- **When:** I proceed to passenger details
- **Then:** I should enter name, age, and gender for each selected seat

**AC4: Passenger validation**
- **Scenario:** Validate passenger details
- **Given:** I am entering passenger information
- **Then:** Name should be 2-50 characters, age 1-120, gender mandatory

**AC5: Boarding and dropping stops**
- **Scenario:** Select pickup and drop points
- **Given:** I have entered passenger details
- **When:** I select boarding and dropping stops
- **Then:** I should see available stops with timings for the selected route

**API Endpoint:** `GET /api/bus/{scheduleId}/stops`

---

### Feature 3: Booking Management

#### US-BM-001: Create Booking Reservation
**Priority:** High  
**Story Points:** 21  
**Sprint:** 2

**Description:**
As a passenger, I want to create a booking reservation, so that my selected seats are temporarily held while I complete payment.

**Acceptance Criteria:**

**AC1: Successful booking creation**
- **Scenario:** Create booking with valid details
- **Given:** I have selected seats and entered passenger details
- **When:** I click "Book Now"
- **Then:** A booking should be created with PNR, 10-minute expiry timer, and total amount

**AC2: Seat validation**
- **Scenario:** Validate seat availability during booking
- **Given:** I attempt to book seats
- **When:** Selected seats become unavailable after my selection
- **Then:** I should see error: "Selected seats are no longer available"

**AC3: Stop validation**
- **Scenario:** Validate boarding and dropping stops
- **Given:** I attempt to book with invalid stops
- **When:** Boarding stop comes after dropping stop in route
- **Then:** I should see error: "Dropping stop must be after boarding stop in route"

**AC4: Booking expiry timer**
- **Scenario:** Display countdown timer
- **Given:** I have a pending booking
- **When:** Booking is created
- **Then:** I should see a 10-minute countdown timer

**AC5: Booking expiry**
- **Scenario:** Booking expires after 10 minutes
- **Given:** I have a pending booking
- **When:** 10 minutes pass without confirmation
- **Then:** The booking should expire and seats should be released

**API Endpoint:** `POST /api/bus/book`

---

#### US-BM-002: Confirm Booking with Payment
**Priority:** High  
**Story Points:** 13  
**Sprint:** 2

**Description:**
As a passenger, I want to confirm my booking by making payment, so that my seats are permanently reserved.

**Acceptance Criteria:**

**AC1: Payment processing**
- **Scenario:** Successful payment confirmation
- **Given:** I have a pending booking
- **When:** I provide payment details and confirm
- **Then:** Booking status should change to "Confirmed" and payment record should be created

**AC2: Payment methods**
- **Scenario:** Multiple payment options
- **Given:** I am on payment page
- **Then:** I should see options for UPI, Card, Net Banking, and Mock payment

**AC3: Payment validation**
- **Scenario:** Validate payment details
- **Given:** I am entering payment information
- **When:** Payment reference ID is missing or invalid
- **Then:** I should see appropriate validation errors

**AC4: Booking confirmation response**
- **Scenario:** Successful confirmation response
- **Given:** Payment is processed successfully
- **When:** Booking is confirmed
- **Then:** I should see confirmation message with booking details

**AC5: Booking confirmation email**
- **Scenario:** Email notification after confirmation
- **Given:** I have successfully confirmed my booking
- **When:** Payment is processed
- **Then:** I should receive a confirmation email with PDF ticket attachment

**API Endpoint:** `POST /api/bus/book/{bookingId}/confirm`

---

## Epic 2: Smart Engine - Connecting Routes

### Feature 4: Multi-Segment Journey Planning

#### US-CR-001: Find Connecting Routes
**Priority:** Medium  
**Story Points:** 21  
**Sprint:** 3

**Description:**
As a passenger, I want to find connecting bus routes when no direct buses are available, so that I can still reach my destination through multiple segments.

**Acceptance Criteria:**

**AC1: Connecting routes search**
- **Scenario:** Search for connecting routes
- **Given:** No direct buses available from "Chennai" to "Mumbai"
- **When:** I search for connecting routes
- **Then:** I should see multi-segment journey options (e.g., Chennai→Bangalore→Mumbai)

**AC2: Route optimization**
- **Scenario:** Choose optimization preference
- **Given:** Multiple connecting routes are available
- **When:** I select "Cheapest" or "Fastest" option
- **Then:** Routes should be sorted by total price or total duration respectively

**AC3: Buffer time validation**
- **Scenario:** Ensure adequate connection time
- **Given:** Connecting routes are displayed
- **Then:** Each connection should have minimum 1-hour buffer time between segments

**AC4: Route details display**
- **Scenario:** View connecting route information
- **Given:** Connecting routes are found
- **Then:** Each route should show total price, total duration, number of segments, and individual segment details

**AC5: No connecting routes**
- **Scenario:** No connecting routes available
- **Given:** I search for connecting routes
- **When:** No valid connections are found
- **Then:** I should see message: "No connecting routes available for this search"

**API Endpoint:** `POST /api/smartengine/connecting-routes`

---

#### US-CR-002: Book Multi-Segment Journey
**Priority:** Medium  
**Story Points:** 34  
**Sprint:** 3

**Description:**
As a passenger, I want to book seats across multiple connecting bus segments, so that I can complete my entire journey with one booking.

**Acceptance Criteria:**

**AC1: Multi-segment booking**
- **Scenario:** Book connecting route
- **Given:** I select a connecting route with 2 segments
- **When:** I provide passenger details and book
- **Then:** Seats should be reserved on both buses with single PNR

**AC2: Passenger consistency**
- **Scenario:** Same passengers across segments
- **Given:** I am booking a multi-segment journey
- **When:** I enter passenger details
- **Then:** Same passengers should be booked on all segments

**AC3: Seat selection per segment**
- **Scenario:** Select seats for each segment
- **Given:** I am booking a connecting route
- **When:** I select seats
- **Then:** I should be able to choose different seats for each segment

**AC4: Atomic booking transaction**
- **Scenario:** All-or-nothing booking
- **Given:** I attempt to book a 2-segment journey
- **When:** One segment fails to book
- **Then:** The entire booking should fail and no seats should be reserved

**AC5: Booking validation**
- **Scenario:** Validate multi-segment booking
- **Given:** I attempt to book connecting route
- **When:** Validation occurs
- **Then:** System should validate seat availability, stop sequences, and passenger details for all segments

**API Endpoint:** `POST /api/smartengine/book-connecting-route`

---

#### US-CR-003: Confirm Multi-Segment Booking
**Priority:** Medium  
**Story Points:** 13  
**Sprint:** 3

**Description:**
As a passenger, I want to confirm my multi-segment booking with payment, so that all my connecting route segments are confirmed together.

**Acceptance Criteria:**

**AC1: Multi-segment confirmation**
- **Scenario:** Confirm connecting route booking
- **Given:** I have a pending connecting route booking
- **When:** I confirm with payment
- **Then:** All segments should be confirmed atomically

**AC2: Payment for total amount**
- **Scenario:** Single payment for entire journey
- **Given:** I am confirming a multi-segment booking
- **When:** I make payment
- **Then:** Payment should be for the total amount of all segments combined

**AC3: Connecting route confirmation email**
- **Scenario:** Specialized email for connecting routes
- **Given:** I confirm a connecting route booking
- **When:** Confirmation is processed
- **Then:** I should receive specialized connecting route email with multi-segment details

**AC4: Confirmation failure handling**
- **Scenario:** Handle partial confirmation failure
- **Given:** I attempt to confirm multi-segment booking
- **When:** Confirmation fails for any segment
- **Then:** Entire confirmation should fail and booking should remain pending

**API Endpoint:** `POST /api/smartengine/book-connecting-route/{bookingId}/confirm`

---

## Epic 3: Communication & Notifications

### Feature 5: Email Notifications

#### US-EN-001: Standard Booking Confirmation Email
**Priority:** High  
**Story Points:** 8  
**Sprint:** 2

**Description:**
As a passenger, I want to receive a booking confirmation email, so that I have proof of my reservation and travel details.

**Acceptance Criteria:**

**AC1: Email content**
- **Scenario:** Receive booking confirmation email
- **Given:** I have confirmed a booking
- **When:** Email is sent
- **Then:** Email should contain PNR, journey details, passenger info, and PDF ticket attachment

**AC2: Email delivery**
- **Scenario:** Asynchronous email sending
- **Given:** I confirm a booking
- **When:** Confirmation is processed
- **Then:** Email should be sent in background without blocking the API response

**AC3: Email template**
- **Scenario:** Professional email format
- **Given:** I receive booking confirmation email
- **Then:** Email should use HTML template with RouteBuddy branding and clear formatting

**AC4: Email failure handling**
- **Scenario:** Handle email sending failure
- **Given:** Email sending fails
- **When:** Error occurs
- **Then:** System should log error but not fail the booking confirmation

**Implementation:** EmailService with HTML template

---

#### US-EN-002: Smart Connecting Route Email
**Priority:** Medium  
**Story Points:** 13  
**Sprint:** 3

**Description:**
As a passenger, I want to receive specialized email for connecting route bookings, so that I have clear information about my multi-segment journey.

**Acceptance Criteria:**

**AC1: Multi-segment email format**
- **Scenario:** Receive connecting route email
- **Given:** I have confirmed a connecting route booking
- **When:** Email is sent
- **Then:** Email should show overall journey, individual segments, connection details, and multi-segment PDF

**AC2: Journey instructions**
- **Scenario:** Clear travel guidance
- **Given:** I receive connecting route email
- **Then:** Email should include specific instructions for connections and buffer times

**AC3: Segment breakdown**
- **Scenario:** Detailed segment information
- **Given:** I receive connecting route email
- **Then:** Each segment should show bus details, timings, stops, and seat information

**AC4: Connection guidance**
- **Scenario:** Connection instructions
- **Given:** I receive connecting route email
- **Then:** Email should highlight connection points and recommended arrival times

**Implementation:** SmartEmailService with specialized template

---

### Feature 6: PDF Ticket Generation

#### US-PT-001: Standard PDF Ticket
**Priority:** High  
**Story Points:** 8  
**Sprint:** 2

**Description:**
As a passenger, I want to receive a PDF ticket, so that I can show it during boarding and have offline access to my booking details.

**Acceptance Criteria:**

**AC1: PDF content**
- **Scenario:** Generate standard PDF ticket
- **Given:** I have a confirmed booking
- **When:** PDF is generated
- **Then:** PDF should contain PNR, journey details, passenger info, seat numbers, and terms & conditions

**AC2: PDF formatting**
- **Scenario:** Professional PDF layout
- **Given:** PDF ticket is generated
- **Then:** PDF should have clear sections, proper formatting, and RouteBuddy branding

**AC3: PDF attachment**
- **Scenario:** Email attachment
- **Given:** Booking confirmation email is sent
- **When:** PDF is attached
- **Then:** PDF should be attached with filename "RouteBuddy-Ticket.pdf"

**Implementation:** PdfService for single-segment tickets

---

#### US-PT-002: Multi-Segment PDF Ticket
**Priority:** Medium  
**Story Points:** 13  
**Sprint:** 3

**Description:**
As a passenger, I want to receive a comprehensive PDF for connecting routes, so that I have all segment details in one document.

**Acceptance Criteria:**

**AC1: Multi-segment PDF format**
- **Scenario:** Generate connecting route PDF
- **Given:** I have a confirmed connecting route booking
- **When:** PDF is generated
- **Then:** PDF should contain overall journey summary, individual segment details, and connection instructions

**AC2: Segment tables**
- **Scenario:** Clear segment breakdown
- **Given:** Multi-segment PDF is generated
- **Then:** Each segment should be displayed in a separate table with complete details

**AC3: Connection information**
- **Scenario:** Connection guidance in PDF
- **Given:** Multi-segment PDF is generated
- **Then:** PDF should include connection points, buffer times, and travel instructions

**Implementation:** SmartPdfService for multi-segment tickets

---

## Epic 4: System Administration

### Feature 7: Automated Booking Management

#### US-AB-001: Auto-Expiry System
**Priority:** High  
**Story Points:** 8  
**Sprint:** 2

**Description:**
As a system, I want to automatically expire pending bookings after 10 minutes, so that seats are released back to inventory.

**Acceptance Criteria:**

**AC1: Background expiry service**
- **Scenario:** Automatic booking expiry
- **Given:** Bookings are pending for more than 10 minutes
- **When:** Background service runs every 2 minutes
- **Then:** Expired bookings should be marked as expired and seats released

**AC2: Expiry logging**
- **Scenario:** Track expiry operations
- **Given:** Bookings are being expired
- **When:** Expiry process runs
- **Then:** System should log the number of bookings expired

**AC3: Seat availability update**
- **Scenario:** Release expired seats
- **Given:** Bookings are expired
- **When:** Expiry process completes
- **Then:** Seats should be available for new bookings immediately

**AC4: Service reliability**
- **Scenario:** Continuous operation
- **Given:** Background service is running
- **When:** Service encounters errors
- **Then:** Service should log errors and continue processing other bookings

**Implementation:** BookingExpiryService with stored procedure

---

## Technical Acceptance Criteria (All Features)

### Performance Requirements
- **Response Time:** API response time < 2 seconds for search operations
- **Database Optimization:** All queries optimized using stored procedures
- **Async Processing:** Email sending should not block API responses (async processing)
- **Concurrent Users:** System should handle 100+ concurrent users

### Security Requirements
- **Input Validation:** Comprehensive validation on all API endpoints
- **SQL Injection Prevention:** Parameterized queries and stored procedures
- **Error Handling:** Proper error handling without exposing sensitive information
- **Data Sanitization:** All user inputs sanitized before processing

### Logging Requirements
- **Structured Logging:** All operations logged with structured format
- **File-based Logging:** Daily log file rotation
- **Error Tracking:** Detailed error logging with correlation IDs
- **Performance Monitoring:** Log response times and database query performance

### Data Integrity Requirements
- **Atomic Transactions:** All booking operations use database transactions
- **Foreign Key Constraints:** Proper database constraints maintained
- **Multi-layer Validation:** Validation at API, Service, and Database layers
- **Concurrency Handling:** Proper handling of concurrent seat bookings

### Scalability Requirements
- **Clean Architecture:** Separation of concerns with layered architecture
- **Dependency Injection:** Proper DI container usage
- **Repository Pattern:** Data access abstraction
- **Service Layer:** Business logic separation

---

## Definition of Done (All User Stories)

### ✅ Development Complete
- [ ] Code implemented following clean architecture principles
- [ ] Unit tests written with minimum 80% coverage
- [ ] Integration tests for all API endpoints
- [ ] Code reviewed and approved by senior developer
- [ ] Static code analysis passed
- [ ] No critical or high severity security vulnerabilities

### ✅ Quality Assurance
- [ ] All acceptance criteria validated through testing
- [ ] Performance benchmarks met (< 2 second response time)
- [ ] Security testing completed
- [ ] Error scenarios tested and handled gracefully
- [ ] Cross-browser compatibility verified (for frontend)
- [ ] Mobile responsiveness verified (for frontend)

### ✅ Documentation
- [ ] API documentation updated in Swagger
- [ ] Database schema changes documented
- [ ] Deployment guide updated with new requirements
- [ ] User guide sections completed
- [ ] Technical documentation updated
- [ ] Code comments added for complex logic

### ✅ Deployment Ready
- [ ] Configuration management setup for all environments
- [ ] Environment-specific settings configured
- [ ] Database migrations created and tested
- [ ] Monitoring and alerting configured
- [ ] Log aggregation setup completed
- [ ] Performance monitoring enabled

### ✅ Business Validation
- [ ] Product Owner acceptance obtained
- [ ] User acceptance testing completed
- [ ] Business rules validated
- [ ] Edge cases identified and handled
- [ ] Rollback plan prepared
- [ ] Go-live checklist completed

---

## Sprint Planning Summary

### Sprint 1 (Core Functionality)
- US-BS-001: Search Buses by Route and Date (8 points)
- US-SL-001: View Bus Seat Layout (8 points)
- US-SL-002: Select and Book Seats (13 points)
- **Total: 29 points**

### Sprint 2 (Booking & Notifications)
- US-BS-002: Advanced Bus Search with Filters (13 points)
- US-BM-001: Create Booking Reservation (21 points)
- US-BM-002: Confirm Booking with Payment (13 points)
- US-EN-001: Standard Booking Confirmation Email (8 points)
- US-PT-001: Standard PDF Ticket (8 points)
- US-AB-001: Auto-Expiry System (8 points)
- **Total: 71 points**

### Sprint 3 (Smart Engine)
- US-CR-001: Find Connecting Routes (21 points)
- US-CR-002: Book Multi-Segment Journey (34 points)
- US-CR-003: Confirm Multi-Segment Booking (13 points)
- US-EN-002: Smart Connecting Route Email (13 points)
- US-PT-002: Multi-Segment PDF Ticket (13 points)
- **Total: 94 points**

---

**Document End**

*This document serves as the comprehensive user story specification for the RouteBuddy bus booking system, designed for import into Azure DevOps for project management and sprint planning.*