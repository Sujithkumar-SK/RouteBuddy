namespace Kanini.RouteBuddy.Common;

public static class MagicStrings
{
    public static class StoredProcedures
    {
        public const string SearchBuses = "sp_SearchBuses";
        public const string SearchBusesFiltered = "sp_SearchBusesFiltered";
        public const string FindConnectingRoutes = "sp_FindConnectingRoutes";
        public const string BookConnectingRoute = "sp_BookConnectingRoute";
        public const string ConfirmConnectingBooking = "sp_ConfirmConnectingBooking";
        public const string GetBusSeatLayout = "sp_GetBusSeatLayout";
        public const string ValidateSeatsAvailability = "sp_ValidateSeatsAvailability";
        public const string ValidateSeatsAndStops = "sp_ValidateSeatsAndStops";
        public const string GetRouteStops = "sp_GetRouteStops";
        public const string ConfirmBooking = "sp_ConfirmBooking";
        public const string ExpirePendingBookings = "sp_ExpirePendingBookings";
        public const string GetBookingDetailsForEmail = "sp_GetBookingDetailsForEmail";
        public const string GetConnectingBookingDetailsForEmail =
            "sp_GetConnectingBookingDetailsForEmail";
        public const string GetAllSeatLayoutTemplates = "sp_GetAllSeatLayoutTemplates";
        public const string GetSeatLayoutTemplatesByBusType = "sp_GetSeatLayoutTemplatesByBusType";
        public const string GetSeatLayoutTemplateById = "sp_GetSeatLayoutTemplateById";
        public const string ApplyTemplateToLayout = "sp_ApplyTemplateToLayout";
        public const string CheckUserExistsByEmail = "sp_CheckUserExistsByEmail";
        public const string CheckUserExistsByPhone = "sp_CheckUserExistsByPhone";
        public const string GetUserById = "sp_GetUserById";
        public const string GetUserByEmail = "sp_GetUserByEmail";
        public const string GetUserByRefreshToken = "sp_GetUserByRefreshToken";
        public const string CreateUser = "sp_CreateUser";
        public const string CreateCustomer = "sp_CreateCustomer";
        public const string CreateRefreshToken = "sp_CreateRefreshToken";
        public const string RevokeRefreshToken = "sp_RevokeRefreshToken";
        public const string UpdateUserPassword = "sp_UpdateUserPassword";
        public const string GetCustomerProfileById = "sp_GetCustomerProfileById";
        public const string GetCustomerProfileByUserId = "sp_GetCustomerProfileByUserId";
        public const string UpdateCustomerProfile = "sp_UpdateCustomerProfile";
        public const string GetCustomerBookings = "sp_GetCustomerBookings";
        public const string GetCustomerById = "sp_GetCustomerById";
        public const string GetAllCustomersWithSummary = "sp_GetAllCustomersWithSummary";
        public const string FilterCustomersWithSummary = "sp_FilterCustomersWithSummary";
        public const string SoftDeleteCustomer = "sp_SoftDeleteCustomer";
        public const string GetBookingForCancellation = "sp_GetBookingForCancellation";
        public const string GetPlaceAutocomplete = "sp_GetPlaceAutocomplete";
        public const string ValidatePlaceExists = "sp_ValidatePlaceExists";
        public const string GetAllActiveRoutes = "sp_GetAllActiveRoutes";
        public const string GetAllActiveStops = "sp_GetAllActiveStops";
        public const string GetScheduleById = "sp_GetScheduleById";
        public const string GetSchedulesByVendor = "sp_GetSchedulesByVendor";
        public const string GetScheduleCountByVendor = "sp_GetScheduleCountByVendor";
        public const string CheckScheduleExists = "sp_CheckScheduleExists";
        public const string GetRouteById = "sp_GetRouteById";
        public const string GetAllRoutes = "sp_GetAllRoutes";
        public const string GetVendorRevenueAnalytics = "sp_GetVendorRevenueAnalytics";
        public const string GetVendorPerformanceMetrics = "sp_GetVendorPerformanceMetrics";
        public const string GetVendorFleetStatus = "sp_GetVendorFleetStatus";
        public const string GetVendorQuickStats = "sp_GetVendorQuickStats";
        public const string GetVendorRecentBookings = "sp_GetVendorRecentBookings";
        public const string GetVendorNotifications = "sp_GetVendorNotifications";
        public const string GetVendorAlerts = "sp_GetVendorAlerts";
        public const string GetVendorMaintenanceSchedule = "sp_GetVendorMaintenanceSchedule";
    }

    public static class LogMessages
    {
        public const string RecaptchaVerificationStarted = "Recaptcha verification started for Email: {Email}";
        public const string RecaptchaVerificationCompleted = "Recaptcha verification completed successfully for Email: {Email}";
        public const string RecaptchaVerificationFailed = "Recaptcha verification failed for Email: {Email}: {Error}";
        public const string BusSearchStarted =
            "Bus search started for Source: {Source}, Destination: {Destination}, Date: {TravelDate}";
        public const string BusSearchCompleted = "Bus search completed. Found {Count} buses";
        public const string BusSearchFailed = "Bus search failed: {Error}";
        public const string ValidationFailed = "Validation failed for request";
        public const string SeatLayoutStarted =
            "Seat layout request started for ScheduleId: {ScheduleId}, Date: {TravelDate}";
        public const string SeatLayoutCompleted = "Seat layout completed. Found {Count} seats";
        public const string SeatLayoutFailed = "Seat layout failed: {Error}";
        public const string BookingStarted =
            "Booking started for ScheduleId: {ScheduleId}, Seats: {SeatCount}, Customer: {CustomerId}";
        public const string BookingCompleted =
            "Booking completed successfully. PNR: {PNR}, BookingId: {BookingId}";
        public const string BookingFailed = "Booking failed: {Error}";
        public const string SeatValidationStarted =
            "Seat availability validation started for {SeatCount} seats";
        public const string SeatValidationCompleted =
            "Seat validation completed. All seats available";
        public const string SeatValidationFailed = "Seat validation failed: {Error}";
        public const string RouteStopsStarted =
            "Route stops request started for ScheduleId: {ScheduleId}";
        public const string RouteStopsCompleted = "Route stops completed. Found {Count} stops";
        public const string RouteStopsFailed = "Route stops failed: {Error}";
        public const string BookingConfirmationStarted =
            "Booking confirmation started for BookingId: {BookingId}";
        public const string BookingConfirmationCompleted =
            "Booking confirmed successfully for BookingId: {BookingId}";
        public const string BookingConfirmationFailed = "Booking confirmation failed: {Error}";
        public const string BookingExpiryStarted = "Auto-expiry process started";
        public const string BookingExpiryCompleted =
            "Auto-expiry completed. Expired {Count} bookings";
        public const string BookingExpiryFailed = "Auto-expiry failed: {Error}";
        public const string FilteredBusSearchStarted =
            "Filtered bus search started for Source: {Source}, Destination: {Destination}, Date: {TravelDate}";
        public const string FilteredBusSearchCompleted =
            "Filtered bus search completed. Found {Count} buses";
        public const string FilteredBusSearchFailed = "Filtered bus search failed: {Error}";
        public const string ConnectingRoutesSearchStarted =
            "Connecting routes search started for Source: {Source}, Destination: {Destination}, Date: {TravelDate}";
        public const string ConnectingRoutesSearchCompleted =
            "Connecting routes search completed. Found {Count} routes";
        public const string ConnectingRoutesSearchFailed =
            "Connecting routes search failed: {Error}";
        public const string ConnectingBookingStarted =
            "Connecting booking started for Customer: {CustomerId}, Segments: {SegmentCount}, Date: {TravelDate}";
        public const string ConnectingBookingCompleted =
            "Connecting booking completed. PNR: {PNR}, BookingId: {BookingId}";
        public const string ConnectingBookingFailed = "Connecting booking failed: {Error}";
        public const string ConnectingBookingConfirmationStarted =
            "Connecting booking confirmation started for BookingId: {BookingId}";
        public const string ConnectingBookingConfirmationCompleted =
            "Connecting booking confirmed successfully for BookingId: {BookingId}";
        public const string ConnectingBookingConfirmationFailed =
            "Connecting booking confirmation failed: {Error}";
        public const string EmailSendingStarted =
            "Email sending started for BookingId: {BookingId}";
        public const string EmailSendingCompleted =
            "Email sent successfully for BookingId: {BookingId}";
        public const string EmailSendingFailed =
            "Email sending failed for BookingId: {BookingId}: {Error}";
        public const string ConnectingEmailSendingStarted =
            "Connecting email sending started for BookingId: {BookingId}";
        public const string ConnectingEmailSendingCompleted =
            "Connecting email sent successfully for BookingId: {BookingId}";
        public const string ConnectingEmailSendingFailed =
            "Connecting email sending failed for BookingId: {BookingId}: {Error}";
        public const string SmtpConnectionFailed = "SMTP connection failed: {Error}";
        public const string PdfGenerationStarted =
            "PDF generation started for BookingId: {BookingId}";
        public const string PdfGenerationCompleted =
            "PDF generated successfully for BookingId: {BookingId}";
        public const string PdfGenerationFailed =
            "PDF generation failed for BookingId: {BookingId}: {Error}";
        public const string BookingEmailDataRetrievalStarted =
            "Booking email data retrieval started for BookingId: {BookingId}";
        public const string BookingEmailDataRetrievalCompleted =
            "Booking email data retrieved successfully for BookingId: {BookingId}, Passengers: {PassengerCount}";
        public const string BookingEmailDataRetrievalFailed =
            "Booking email data retrieval failed for BookingId: {BookingId}: {Error}";
        public const string SmartEmailSendingStarted =
            "Smart email sending started for BookingId: {BookingId}";
        public const string SmartEmailSendingCompleted =
            "Smart email sent successfully for BookingId: {BookingId}";
        public const string SmartEmailSendingFailed =
            "Smart email sending failed for BookingId: {BookingId}: {Error}";
        public const string SmartSmtpConnectionFailed = "Smart SMTP connection failed: {Error}";
        public const string SmartPdfGenerationStarted =
            "Smart PDF generation started for BookingId: {BookingId}";
        public const string SmartPdfGenerationCompleted =
            "Smart PDF generated successfully for BookingId: {BookingId}";
        public const string SmartPdfGenerationFailed =
            "Smart PDF generation failed for BookingId: {BookingId}: {Error}";
        public const string SmartBookingEmailDataRetrievalStarted =
            "Smart booking email data retrieval started for BookingId: {BookingId}";
        public const string SmartBookingEmailDataRetrievalCompleted =
            "Smart booking email data retrieved successfully for BookingId: {BookingId}, Passengers: {PassengerCount}";
        public const string SmartBookingEmailDataRetrievalFailed =
            "Smart booking email data retrieval failed for BookingId: {BookingId}: {Error}";
        public const string SmartEmailTemplateLoadFailed =
            "Smart email template load failed: {Error}";
        public const string SeatLayoutTemplateGetAllStarted =
            "Getting all seat layout templates started";
        public const string SeatLayoutTemplateGetAllCompleted =
            "Getting all seat layout templates completed. Found {Count} templates";
        public const string SeatLayoutTemplateGetAllFailed =
            "Getting all seat layout templates failed: {Error}";
        public const string SeatLayoutTemplateGetByIdStarted =
            "Getting seat layout template by ID started for TemplateId: {TemplateId}";
        public const string SeatLayoutTemplateGetByIdCompleted =
            "Getting seat layout template by ID completed for TemplateId: {TemplateId}";
        public const string SeatLayoutTemplateGetByIdFailed =
            "Getting seat layout template by ID failed: {Error}";
        public const string SeatLayoutTemplateCreationStarted =
            "Seat layout template creation started for Template: {TemplateName}";
        public const string SeatLayoutTemplateCreationCompleted =
            "Seat layout template created successfully with ID: {TemplateId}";
        public const string SeatLayoutTemplateCreationFailed =
            "Seat layout template creation failed: {Error}";
        public const string SeatLayoutTemplateUpdateStarted =
            "Seat layout template update started for TemplateId: {TemplateId}";
        public const string SeatLayoutTemplateUpdateCompleted =
            "Seat layout template updated successfully for TemplateId: {TemplateId}";
        public const string SeatLayoutTemplateUpdateFailed =
            "Seat layout template update failed: {Error}";
        public const string SeatLayoutTemplateDeactivationStarted =
            "Seat layout template deactivation started for TemplateId: {TemplateId}";
        public const string SeatLayoutTemplateDeactivationCompleted =
            "Seat layout template deactivated successfully for TemplateId: {TemplateId}";
        public const string SeatLayoutTemplateDeactivationFailed =
            "Seat layout template deactivation failed: {Error}";
        public const string SeatLayoutTemplatesByBusTypeStarted =
            "Getting seat layout templates by bus type started for BusType: {BusType}";
        public const string SeatLayoutTemplatesByBusTypeCompleted =
            "Getting seat layout templates by bus type completed. Found {Count} templates";
        public const string SeatLayoutTemplatesByBusTypeFailed =
            "Getting seat layout templates by bus type failed: {Error}";
        public const string BusTemplateApplicationStarted =
            "Bus template application started for BusId: {BusId}, TemplateId: {TemplateId}";
        public const string BusTemplateApplicationCompleted =
            "Bus template applied successfully for BusId: {BusId}";
        public const string BusTemplateApplicationFailed =
            "Bus template application failed: {Error}";
        public const string RegistrationOtpStarted = "Registration OTP generation started for Email: {Email}";
        public const string RegistrationOtpCompleted = "Registration OTP sent successfully to Email: {Email}";
        public const string RegistrationOtpFailed = "Registration OTP failed for Email: {Email}: {Error}";
        public const string RegistrationVerificationStarted = "Registration OTP verification started for Email: {Email}";
        public const string RegistrationVerificationCompleted = "Registration completed for Email: {Email}, UserId: {UserId}";
        public const string RegistrationVerificationFailed = "Registration verification failed for Email: {Email}: {Error}";
        public const string LoginAttemptStarted = "Login attempt started for Email: {Email}";
        public const string LoginAttemptCompleted = "Login successful for Email: {Email}, UserId: {UserId}";
        public const string LoginAttemptFailed = "Login failed for Email: {Email}: {Error}";
        public const string TokenRefreshStarted = "Token refresh started for RefreshToken";
        public const string TokenRefreshCompleted = "Token refreshed successfully for UserId: {UserId}";
        public const string TokenRefreshFailedLog = "Token refresh failed: {Error}";
        public const string LogoutStarted = "Logout started for UserId: {UserId}";
        public const string LogoutCompleted = "Logout completed for UserId: {UserId}";
        public const string LogoutFailedLog = "Logout failed for UserId: {UserId}: {Error}";
        public const string PasswordChangeStarted = "Password change started for UserId: {UserId}";
        public const string PasswordChangeCompleted = "Password changed successfully for UserId: {UserId}";
        public const string PasswordChangeFailedLog = "Password change failed for UserId: {UserId}: {Error}";
        public const string ForgotPasswordStarted = "Forgot password OTP generation started for Email: {Email}";
        public const string ForgotPasswordCompleted = "Forgot password OTP sent successfully to Email: {Email}";
        public const string ForgotPasswordFailed = "Forgot password failed for Email: {Email}: {Error}";
        public const string ForgotPasswordVerificationStarted = "Forgot password OTP verification started for Email: {Email}";
        public const string ForgotPasswordVerificationCompleted = "Forgot password OTP verified for Email: {Email}";
        public const string ForgotPasswordVerificationFailed = "Forgot password OTP verification failed for Email: {Email}: {Error}";
        public const string PasswordResetStarted = "Password reset started for Email: {Email}";
        public const string PasswordResetCompleted = "Password reset completed for Email: {Email}";
        public const string PasswordResetFailedLog = "Password reset failed for Email: {Email}: {Error}";
        public const string PaymentInitiationStarted = "Payment initiation started for BookingId: {BookingId}";
        public const string PaymentInitiationCompleted = "Payment initiated successfully for PaymentId: {PaymentId}";
        public const string PaymentInitiationFailed = "Payment initiation failed: {Error}";
        public const string PaymentVerificationStarted = "Payment verification started for PaymentId: {PaymentId}";
        public const string PaymentVerificationCompleted = "Payment verified successfully for PaymentId: {PaymentId}";
        public const string PaymentVerificationFailed = "Payment verification failed: {Error}";
        public const string PaymentCreationStarted = "Payment creation started for BookingId: {BookingId}";
        public const string PaymentCreationCompleted = "Payment created successfully with PaymentId: {PaymentId}";
        public const string PaymentCreationFailed = "Payment creation failed: {Error}";
        public const string PaymentUpdateStarted = "Payment update started for PaymentId: {PaymentId}";
        public const string PaymentUpdateCompleted = "Payment updated successfully for PaymentId: {PaymentId}";
        public const string PaymentUpdateFailed = "Payment update failed: {Error}";
        public const string PaymentRetrievalFailed = "Payment retrieval failed: {Error}";
        public const string GetPaymentsStarted = "Get payments started for BookingId: {BookingId}";
        public const string GetPaymentsCompleted = "Get payments completed for BookingId: {BookingId}";
        public const string GetPaymentsFailed = "Get payments failed: {Error}";
        public const string InvalidPaymentId = "Invalid payment ID: {PaymentId}";
        public const string InvalidBookingId = "Invalid booking ID: {BookingId}";
        public const string CustomerProfileRetrievalStarted = "Customer profile retrieval started for CustomerId: {CustomerId}";
        public const string CustomerProfileRetrievalCompleted = "Customer profile retrieved successfully for CustomerId: {CustomerId}";
        public const string CustomerProfileRetrievalFailed = "Customer profile retrieval failed for CustomerId: {CustomerId}: {Error}";
        public const string CustomerProfileUpdateStarted = "Customer profile update started for CustomerId: {CustomerId}";
        public const string CustomerProfileUpdateCompleted = "Customer profile updated successfully for CustomerId: {CustomerId}";
        public const string CustomerProfileUpdateFailed = "Customer profile update failed for CustomerId: {CustomerId}: {Error}";
        public const string CustomerBookingsRetrievalStarted = "Customer bookings retrieval started for CustomerId: {CustomerId}";
        public const string CustomerBookingsRetrievalCompleted = "Customer bookings retrieved successfully for CustomerId: {CustomerId}, Count: {Count}";
        public const string CustomerBookingsRetrievalFailed = "Customer bookings retrieval failed for CustomerId: {CustomerId}: {Error}";
        public const string BookingCancellationDataRetrievalStarted = "Booking cancellation data retrieval started for BookingId: {BookingId}, CustomerId: {CustomerId}";
        public const string BookingCancellationDataRetrievalCompleted = "Booking cancellation data retrieved successfully for BookingId: {BookingId}";
        public const string BookingCancellationDataRetrievalFailed = "Booking cancellation data retrieval failed for BookingId: {BookingId}, CustomerId: {CustomerId}: {Error}";
        public const string BookingNotFoundForCancellation = "Booking not found for cancellation - BookingId: {BookingId}, CustomerId: {CustomerId}";
        public const string BookingCancellationStarted = "Booking cancellation started for BookingId: {BookingId}";
        public const string BookingCancellationCompleted = "Booking cancelled successfully for BookingId: {BookingId}";
        public const string BookingCancellationFailed = "Booking cancellation failed for BookingId: {BookingId}: {Error}";
        public const string BookingNotFound = "Booking not found for BookingId: {BookingId}";
        public const string PlaceAutocompleteStarted = "Place autocomplete started for Query: {Query}";
        public const string PlaceAutocompleteCompleted = "Place autocomplete completed. Found {Count} places";
        public const string PlaceAutocompleteFailed = "Place autocomplete failed: {Error}";
        public const string PlaceValidationStarted = "Place validation started for Source: {Source}, Destination: {Destination}";
        public const string PlaceValidationCompleted = "Place validation completed successfully";
        public const string PlaceValidationFailed = "Place validation failed: {Error}";
        public const string VendorIdRetrievalFailed = "Vendor ID retrieval failed: {Error}";
    }

    public static class ErrorMessages
    {
        public const string RecaptchaVerificationFailed = "Recaptcha verification failed. Please try again.";
        public const string RecaptchaTokenRequired = "Recaptcha token is required.";
        public const string SourceRequired = "Source is required";
        public const string DestinationRequired = "Destination is required";
        public const string TravelDateRequired = "Travel date is required";
        public const string TravelDateInvalid = "Travel date cannot be in the past";
        public const string ScheduleIdRequired = "Schedule ID is required";
        public const string ScheduleNotFound = "Bus schedule not found";
        public const string DatabaseError = "Database operation failed";
        public const string UnexpectedError = "An unexpected error occurred";
        public const string SeatsRequired = "At least one seat must be selected";
        public const string PassengersRequired = "Passenger details are required";
        public const string SeatPassengerMismatch =
            "Number of seats must match number of passengers";
        public const string SeatsNotAvailable = "One or more selected seats are not available";
        public const string BookingFailed = "Booking could not be completed";
        public const string InvalidSeatNumbers = "Invalid seat numbers provided";
        public const string CustomerNotFound = "Customer not found";
        public const string BoardingStopRequired = "Boarding stop is required";
        public const string DroppingStopRequired = "Dropping stop is required";
        public const string InvalidBoardingStop = "Invalid boarding stop selected";
        public const string InvalidDroppingStop = "Invalid dropping stop selected";
        public const string SameStopError = "Boarding and dropping stops cannot be the same";
        public const string InvalidStopOrder = "Dropping stop must be after boarding stop in route";
        public const string BookingNotFound = "Booking not found";
        public const string BookingAlreadyConfirmed = "Booking is already confirmed";
        public const string BookingExpired = "Booking reservation has expired";
        public const string PaymentReferenceRequired = "Payment reference ID is required";
        public const string InvalidTimeRange = "Departure time 'from' cannot be greater than 'to'";
        public const string InvalidPriceRange =
            "Minimum price cannot be greater than maximum price";
        public const string InvalidSortOption = "Invalid sort option provided";
        public const string InvalidBusTypeFilter = "Invalid bus type filter provided";
        public const string InvalidAmenitiesFilter = "Invalid amenities filter provided";
        public const string NoConnectingRoutesFound =
            "No connecting routes available for this search";
        public const string InvalidToggleOption =
            "Invalid toggle option. Must be 'cheapest' or 'fastest'";
        public const string ConnectingRoutesNotFound = "No complete connecting routes found";
        public const string ConnectingBookingFailed =
            "Connecting route booking could not be completed";
        public const string SegmentValidationFailed = "One or more segments have validation errors";
        public const string PassengerMismatchAcrossSegments =
            "Passenger count must be same across all segments";
        public const string ConnectingBookingNotFound = "Connecting booking not found";
        public const string PartialBookingFailure = "Some segments could not be booked";
        public const string EmailSendingFailed = "Email could not be sent";
        public const string SmtpConnectionFailed = "SMTP server connection failed";
        public const string PdfGenerationFailed = "PDF ticket generation failed";
        public const string BookingDataNotFound = "Booking data not found for email";
        public const string InvalidEmailAddress = "Invalid email address provided";
        public const string EmailTemplateNotFound = "Email template not found";
        public const string BookingEmailDataRetrievalFailed =
            "Failed to retrieve booking data for email";
        public const string ConnectingBookingDataNotFound =
            "Connecting booking data not found for email";
        public const string SmartEmailSendingFailed = "Smart email could not be sent";
        public const string SmartSmtpConnectionFailed = "Smart SMTP server connection failed";
        public const string SmartPdfGenerationFailed = "Smart PDF ticket generation failed";
        public const string SeatLayoutTemplateNotFound = "Seat layout template not found";
        public const string SeatLayoutTemplateNameRequired = "Template name is required";
        public const string SeatLayoutTemplateNameTooShort = "Template name must be at least 3 characters";
        public const string SeatLayoutTemplateNameTooLong = "Template name cannot exceed 100 characters";
        public const string SeatLayoutTemplateNameExists = "Template name already exists";
        public const string SeatLayoutTemplateTotalSeatsRequired = "Total seats is required";
        public const string SeatLayoutTemplateTotalSeatsInvalid = "Total seats must be between 1 and 100";
        public const string SeatLayoutTemplateBusTypeRequired = "Bus type is required";
        public const string SeatLayoutTemplateSeatDetailsRequired = "Seat details are required";
        public const string SeatLayoutTemplateSeatDetailsMismatch = "Number of seat details must match total seats";
        public const string SeatLayoutTemplateDuplicateSeatNumber = "Duplicate seat numbers are not allowed";
        public const string SeatLayoutTemplateInvalidSeatNumber = "Invalid seat number format";
        public const string SeatLayoutTemplateInvalidRowColumn = "Invalid row or column number";
        public const string EmailRequired = "Email is required.";
        public const string EmailAlreadyExists = "This email is already registered.";
        public const string PhoneAlreadyExists = "This phone number is already registered.";
        public const string EmailNotVerified = "Please verify your email before registration.";
        public const string InvalidOrExpiredOtp = "Invalid or expired verification code.";
        public const string EmailNotFound = "Email not found.";
        public const string ResendNotAllowed = "Please wait 120 seconds before requesting a new verification code.";
        public const string InternalServerError = "An unexpected error occurred. Please try again later.";
        public const string InvalidCredentials = "Invalid email or password.";
        public const string AccountInactive = "Your account is inactive.";
        public const string InvalidRefreshToken = "Invalid or expired refresh token.";
        public const string UserNotFound = "User not found.";
        public const string InvalidCurrentPassword = "Current password is incorrect.";
        public const string RegistrationFailed = "Registration failed. Please try again.";
        public const string LoginFailed = "Login failed. Please try again.";
        public const string TokenRefreshFailed = "Token refresh failed.";
        public const string LogoutFailed = "Logout failed.";
        public const string PasswordChangeFailed = "Password change failed.";
        public const string OtpSendFailed = "Failed to send OTP.";
        public const string OtpVerificationFailed = "OTP verification failed.";
        public const string PasswordResetFailed = "Password reset failed.";
        public const string PaymentFailed = "Payment processing failed";
        public const string PaymentNotFound = "Payment not found";
        public const string InvalidCustomerId = "Invalid customer ID";
        public const string InvalidBookingId = "Invalid booking ID";
        public const string InvalidPaymentId = "Invalid payment ID";
        public const string InvalidBookingStatus = "Invalid booking status";
        public const string CustomerProfileNotFound = "Customer profile not found";
        public const string CustomerProfileUpdateFailed = "Customer profile update failed";
        public const string InvalidCustomerData = "Invalid customer data provided";
        public const string PaymentAlreadyProcessed = "Payment has already been processed";
        public const string BookingCannotBeCancelled = "Booking cannot be cancelled";
        public const string BookingAlreadyCancelled = "Booking is already cancelled";
        public const string CancellationNotAllowed = "Cancellation not allowed within 2 hours of travel";
        public const string CancellationReasonRequired = "Cancellation reason is required";
        public const string InvalidCancellationReason = "Cancellation reason must be between 10 and 250 characters";
        public const string InvalidPlace = "Invalid place name. Please select from suggestions";
        public const string PlaceNotFound = "Place not found in our database";
        public const string QueryTooShort = "Please enter at least 1 character to search places";
    }

    public static class ConfigKeys
    {
        public const string RecaptchaSiteKey = "RecaptchaSettings:SiteKey";
        public const string RecaptchaSecretKey = "RecaptchaSettings:SecretKey";
        public const string SmtpServer = "EmailSettings:SmtpServer";
        public const string SmtpPort = "EmailSettings:SmtpPort";
        public const string EmailSenderName = "EmailSettings:SenderName";
        public const string EmailSenderEmail = "EmailSettings:SenderEmail";
        public const string EmailUsername = "EmailSettings:Username";
        public const string EmailPassword = "EmailSettings:AppPassword";
        public const string EnableSsl = "EmailSettings:EnableSsl";
        public const string DatabaseConnectionString = "DatabaseConnectionString";
        public const string FromEmail = "EmailSettings:FromEmail";
    }

    public static class ErrorCodes
    {
        public const string RecaptchaFailed = "RECAPTCHA_FAILED";
        public const string EmailSendingFailed = "EMAIL_SENDING_FAILED";
        public const string SmtpConnectionFailed = "SMTP_CONNECTION_FAILED";
        public const string PdfGenerationFailed = "PDF_GENERATION_FAILED";
        public const string BookingDataNotFound = "BOOKING_DATA_NOT_FOUND";
        public const string InvalidEmailAddress = "INVALID_EMAIL_ADDRESS";
        public const string BookingEmailDataRetrievalFailed = "BOOKING_EMAIL_DATA_RETRIEVAL_FAILED";
        public const string ConnectingBookingDataNotFound = "CONNECTING_BOOKING_DATA_NOT_FOUND";
        public const string SmartEmailSendingFailed = "SMART_EMAIL_SENDING_FAILED";
        public const string SmartSmtpConnectionFailed = "SMART_SMTP_CONNECTION_FAILED";
        public const string SmartPdfGenerationFailed = "SMART_PDF_GENERATION_FAILED";
        public const string PaymentFailed = "PAYMENT_FAILED";
        public const string PaymentNotFound = "PAYMENT_NOT_FOUND";
        public const string InvalidCustomerId = "INVALID_CUSTOMER_ID";
        public const string InvalidBookingId = "INVALID_BOOKING_ID";
        public const string InvalidPaymentId = "INVALID_PAYMENT_ID";
        public const string InvalidBookingStatus = "INVALID_BOOKING_STATUS";
    }

    public static class SuccessMessages
    {
        public const string EmailSentSuccessfully = "Email sent successfully";
        public const string PdfGeneratedSuccessfully = "PDF generated successfully";
        public const string BookingEmailDataRetrieved = "Booking email data retrieved successfully";
        public const string SmartEmailSentSuccessfully = "Smart email sent successfully";
        public const string SmartPdfGeneratedSuccessfully = "Smart PDF generated successfully";
        public const string SmartBookingEmailDataRetrieved =
            "Smart booking email data retrieved successfully";
        public const string OtpSent = "Verification code sent successfully to your email.";
        public const string EmailVerified = "Email verified successfully.";
        public const string OtpResent = "Verification code resent successfully.";
        public const string PasswordResetMailSent = "Password reset code sent to your email.";
        public const string PasswordUpdated = "Password updated successfully.";
        public const string RegistrationCompleted = "Registration completed successfully.";
        public const string LoginSuccessful = "Login successful.";
        public const string TokenRefreshed = "Token refreshed successfully.";
        public const string LogoutSuccessful = "Logout successful.";
        public const string PasswordChanged = "Password changed successfully.";
        public const string OtpVerified = "OTP verified successfully.";
        public const string PasswordReset = "Password reset successfully.";
        public const string PaymentVerifiedSuccessfully = "Payment verified successfully";
        public const string PaymentInitiatedSuccessfully = "Payment initiated successfully";
        public const string BookingConfirmedAfterPayment = "Booking confirmed after successful payment";
        public const string CustomerProfileRetrievedSuccessfully = "Customer profile retrieved successfully";
        public const string CustomerProfileUpdatedSuccessfully = "Customer profile updated successfully";
        public const string BookingCancelledSuccessfully = "Booking cancelled successfully";
        public const string RefundProcessedSuccessfully = "Refund processed successfully";
    }

    public static class EmailAttachments
    {
        public const string TicketFileName = "RouteBuddy-Ticket.pdf";
        public const string PdfContentType = "application/pdf";
        public const string SmartTicketFileName = "RouteBuddy-ConnectingRoute-Ticket.pdf";
    }

    public static class EmailTemplates
    {
        public const string BookingConfirmationSubject =
            "RouteBuddy - Booking Confirmation (PNR: {0})";
        public const string ConnectingBookingConfirmationSubject =
            "RouteBuddy - Connecting Route Booking Confirmation (PNR: {0})";
        public const string SmartBookingConfirmationSubject =
            "RouteBuddy Smart Engine - Multi-Segment Journey Confirmed (PNR: {0})";
        public const string VendorRejectionSubject = "RouteBuddy - Vendor Application Rejected";
        public const string BusNotificationSubject = "RouteBuddy - Bus Status Update";
        public const string VendorRejectionTemplate = "Dear {0},\n\nWe regret to inform you that your vendor application has been rejected.\n\nReason: {1}\n\nBest regards,\nRouteBuddy Team";
        public const string BusNotificationTemplate = "Dear {0},\n\n{1}\n\nBest regards,\nRouteBuddy Team";
        
        public static string GetVerificationCodeEmail(string customerName, string otp)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Email Verification - RouteBuddy</h2>
                    <p>Dear {customerName},</p>
                    <p>Your verification code is: <strong>{otp}</strong></p>
                    <p>This code will expire in 10 minutes.</p>
                    <p>Best regards,<br/>RouteBuddy Team</p>
                </body>
                </html>";
        }

        public static string GetPasswordResetEmail(string customerName, string otp)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Password Reset - RouteBuddy</h2>
                    <p>Dear {customerName},</p>
                    <p>Your password reset code is: <strong>{otp}</strong></p>
                    <p>This code will expire in 10 minutes.</p>
                    <p>Best regards,<br/>RouteBuddy Team</p>
                </body>
                </html>";
        }
    }
}
