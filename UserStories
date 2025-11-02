# 🚌 RouteBuddy – Payment, Cancellation & Refund User Stories
**Version:** 1.0  
**Author:** Intern 
**Date:** November 2025  
**Document Type:** User Stories & Acceptance Criteria (Azure DevOps Format)

---

## Epic 1: Payment Processing System

### Feature 1: Card Payment Processing
**US-PM-001: Process Card Payment**  
**Priority:** High  
**Story Points:** 13  
**Sprint:** 1  

**Description:**  
As a passenger, I want to pay for my bus booking using my debit card, so that I can secure my seat and complete my booking.

**Acceptance Criteria:**

**AC1: Valid card payment processing**  
**Scenario:** Process payment with valid card details  
Given: I have a pending booking with BookingId "12345" and Amount "₹500"  
When: I enter valid card details  
Then: Payment should be initiated with status "Pending" and TransactionId generated  

**AC2: Card validation**  
**Scenario:** Validate card details  
Given: I am entering card information  
When: I enter invalid CardNumber or expired date  
Then: I should see error "Card number must be 16 digits" or "Card has expired"  

**AC3: OTP requirement**  
**Scenario:** OTP verification for card payment  
Given: I have initiated card payment  
When: Payment is processed  
Then: I should receive OTP requirement with PaymentId and masked card number  

**AC4: Payment failure handling**  
**Scenario:** Handle payment failures  
Given: I have invalid card details  
When: Payment processing fails  
Then: Payment status should be "Failed" with appropriate error message  

**API Endpoint:**  
`POST /api/cardpayment/process`

---

**US-PM-002: Card Payment OTP Verification**  
**Priority:** High  
**Story Points:** 8  
**Sprint:** 1  

**Description:**  
As a passenger, I want to verify my card payment using OTP, so that my transaction is secure and authorized.

**Acceptance Criteria:**

**AC1: Successful OTP verification**  
**Scenario:** Verify OTP for card payment  
Given: I have PaymentId "123" with pending OTP verification  
When: I enter correct OTP "123456"  
Then: Payment status should change to "Success" and booking should be confirmed  

**AC2: Invalid OTP handling**  
**Scenario:** Handle invalid OTP  
Given: I have PaymentId "123" pending OTP verification  
When: I enter incorrect OTP  
Then: I should see error "Invalid OTP" and payment remains pending  

**AC3: OTP format validation**  
**Scenario:** Validate OTP format  
Given: I am entering OTP  
When: I enter OTP with less than 6 digits  
Then: I should see error "OTP must be 6 digits"  

**API Endpoint:**  
`POST /api/cardpayment/verify-otp`

---

### Feature 2: UPI Payment Processing
**US-PM-003: Process UPI Payment**  
**Priority:** High  
**Story Points:** 8  
**Sprint:** 1  

**Description:**  
As a passenger, I want to pay using UPI, so that I can make quick and secure payments.

**Acceptance Criteria:**

**AC1: Valid UPI payment**  
**Scenario:** Process UPI payment with valid UPI ID  
Given: I have BookingId "12345" and Amount "₹500"  
When: I enter UpiId "user@paytm" and optional CustomerPhoneNumber "9876543210"  
Then: Payment should be processed immediately with status "Success"  

**AC2: UPI ID validation**  
**Scenario:** Validate UPI ID format  
Given: I am entering UPI details  
When: I enter invalid UpiId "invalidupi"  
Then: I should see error "Invalid UPI ID format"  

**AC3: Phone number validation**  
**Scenario:** Validate customer phone number  
Given: I am entering UPI details  
When: I enter invalid phone "123456"  
Then: I should see error "Phone number must be 10 digits starting with 6-9"  

**API Endpoint:**  
`POST /api/payment/upi/process`

---

### Feature 3: Payment Validation
**US-PM-004: Real-time Payment Validation**  
**Priority:** Medium  
**Story Points:** 5  
**Sprint:** 2  

**Description:**  
As a passenger, I want to validate my payment details in real-time, so that I get immediate feedback on my input.

**Acceptance Criteria:**

**AC1: Card number validation**  
**Scenario:** Validate card number  
Given: I am entering card details  
When: I enter CardNumber "4111111111111111"  
Then: I should see card type "Visa" and validation status "Valid"  

**AC2: Expiry date validation**  
**Scenario:** Validate card expiry  
Given: I am entering expiry details  
When: I enter ExpiryMonth "12" and ExpiryYear "2025"  
Then: I should see validation status "Valid" for future date  

**AC3: CVV validation**  
**Scenario:** Validate CVV  
Given: I am entering CVV  
When: I enter CVV "123"  
Then: I should see validation status "Valid" for 3-digit CVV  

**API Endpoints:**  
`POST /api/cardpayment/validate-card`  
`POST /api/cardpayment/validate-expiry`  
`POST /api/cardpayment/validate-cvv`

---

## Epic 2: Payment Management

### Feature 4: Payment Status & History
**US-PM-005: Check Payment Status**  
**Priority:** Medium  
**Story Points:** 5  
**Sprint:** 2  

**Description:**  
As a passenger, I want to check my payment status using transaction ID, so that I can confirm my payment was successful.

**Acceptance Criteria:**

**AC1: Payment status retrieval**  
**Scenario:** Get payment status by transaction ID  
Given: I have TransactionId "TXN123456789"  
When: I check payment status  
Then: I should see payment details with current status, amount, and booking info  

**AC2: Invalid transaction ID**  
**Scenario:** Handle invalid transaction ID  
Given: I enter non-existent TransactionId "INVALID123"  
When: I check payment status  
Then: I should see error "Payment not found"  

**API Endpoint:**  
`GET /api/payment/{transactionId}/status`

---

## Epic 3: Refund & Cancellation Management

### Feature 5: Booking Cancellation
**US-PM-006: Cancel Booking with Refund**  
**Priority:** High  
**Story Points:** 13  
**Sprint:** 3  

**Description:**  
As a passenger, I want to cancel my booking and request refund, so that I can get my money back when I can't travel.

**Acceptance Criteria:**

**AC1: Successful cancellation**  
**Scenario:** Cancel booking with refund request  
Given: I have confirmed BookingId "12345" with payment  
When: I cancel with reason "Personal emergency"  
Then: Booking should be cancelled and refund initiated  

**AC2: Cancellation penalty calculation**  
**Scenario:** Calculate penalty before cancellation  
Given: I want to cancel BookingId "12345"  
When: I request penalty calculation  
Then: I should see penalty % and refund amount after deduction  

**AC3: Cancellation validation**  
**Scenario:** Validate cancellation eligibility  
Given: I have BookingId "12345" with departure in 1 hour  
When: I attempt to cancel  
Then: I should see restriction or message per cancellation rules  

**API Endpoints:**  
`POST /api/cancellation/cancel`  
`GET /api/cancellation/penalty/{bookingId}`

---

### Feature 6: Refund Processing
**US-PM-007: Process Refunds**  
**Priority:** High  
**Story Points:** 8  
**Sprint:** 3  

**Description:**  
As a passenger, I want to receive refunds for cancelled bookings, so that I get my money back according to policy.

**Acceptance Criteria:**

**AC1: Create refund request**  
**Scenario:** Create refund for cancelled booking  
Given: I have cancelled booking with PaymentId "123"  
When: Refund initiated with RefundAmount "₹450"  
Then: Refund record created with status "Pending"  

**AC2: Process pending refund**  
**Scenario:** Process approved refund  
Given: I have RefundId "456" with status "Pending"  
When: Admin processes refund  
Then: Refund status changes to "Processed" and credited  

**AC3: View pending refunds**  
**Scenario:** Admin views pending refunds  
Given: There are pending refunds  
When: Admin requests refund list  
Then: Paginated list of pending refunds displayed  

**API Endpoints:**  
`POST /api/refund/create`  
`POST /api/refund/process/{refundId}`  
`GET /api/refund/pending`

## Epic 4: Payment Security & Compliance

### Feature 8: Secure Payment Processing
**US-PM-009: Payment Data Security**  
**Priority:** High  
**Story Points:** 5  
**Sprint:** 1  

**Description:**  
As a passenger, I want my payment information to be secure, so that my financial data is protected.

**Acceptance Criteria:**

**AC1: Masked card display**  
**Scenario:** Display masked card information  
Given: I have processed card payment  
When: Payment response returned  
Then: Card number masked (show last 4 digits only)  

**AC2: Secure data transmission**  
**Scenario:** Encrypt sensitive payment data  
Given: I am submitting payment info  
When: Data transmitted to server  
Then: All sensitive fields encrypted  

**AC3: No sensitive data storage**  
**Scenario:** Prevent storage of card data  
Given: Payment is processed  
When: Record saved  
Then: CVV and full card number are never stored  

**AC4: Transaction logging**  
**Scenario:** Log payment transactions securely  
Given: Payment operations are performed  
When: System processes payments  
Then: All operations logged with audit trail  

---

✅ **End of Document**
