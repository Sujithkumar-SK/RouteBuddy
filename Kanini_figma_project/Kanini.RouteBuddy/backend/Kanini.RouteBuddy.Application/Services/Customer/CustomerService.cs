using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Data.Repositories.Customer;
using Kanini.RouteBuddy.Data.Repositories.Email;
using Kanini.RouteBuddy.Data.Repositories.Booking;
using Kanini.RouteBuddy.Application.Services.Pdf;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Domain.Enums;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IBookingCancellationRepository _cancellationRepository;
        private readonly IPdfService _pdfService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerRepository customerRepository, 
            IEmailRepository emailRepository,
            IBookingCancellationRepository cancellationRepository,
            IPdfService pdfService,
            IMapper mapper, 
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _emailRepository = emailRepository;
            _cancellationRepository = cancellationRepository;
            _pdfService = pdfService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AdminCustomerDTO>> GetAllCustomersAsync()
        {
            _logger.LogInformation("CustomerService GetAllCustomersAsync started");
            _logger.LogInformation("Calling repository GetAllCustomersAsync");
            var customers = await _customerRepository.GetAllCustomersAsync();
            _logger.LogInformation("Repository call completed");
            _logger.LogInformation("Starting AutoMapper mapping");
            var result = _mapper.Map<IEnumerable<AdminCustomerDTO>>(customers);
            _logger.LogInformation("AutoMapper mapping completed");
            _logger.LogInformation("CustomerService GetAllCustomersAsync completed");
            return result;
        }

        public async Task<IEnumerable<AdminCustomerDTO>> FilterCustomersAsync(string? searchName, bool? isActive, int? minAge, int? maxAge)
        {
            _logger.LogInformation("CustomerService FilterCustomersAsync started");
            _logger.LogInformation("Calling repository FilterCustomersAsync");
            var customers = await _customerRepository.FilterCustomersAsync(searchName, isActive, minAge, maxAge);
            _logger.LogInformation("Repository call completed");
            _logger.LogInformation("Starting AutoMapper mapping");
            var result = _mapper.Map<IEnumerable<AdminCustomerDTO>>(customers);
            _logger.LogInformation("AutoMapper mapping completed");
            _logger.LogInformation("CustomerService FilterCustomersAsync completed");
            return result;
        }

        public async Task<AdminCustomerDTO?> GetCustomerByIdAsync(int customerId)
        {
            _logger.LogInformation("CustomerService GetCustomerByIdAsync started");

            if (customerId < 1 || customerId > 999999)
            {
                _logger.LogWarning("Invalid customer ID {CustomerId}", customerId);
                return null;
            }

            _logger.LogInformation("Validation passed");
            _logger.LogInformation("Calling repository GetCustomerByIdAsync");
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            _logger.LogInformation("Repository call completed");

            if (customer == null)
            {
                _logger.LogInformation("Customer not found, returning null");
                return null;
            }

            _logger.LogInformation("Starting AutoMapper mapping");
            var result = _mapper.Map<AdminCustomerDTO>(customer);
            _logger.LogInformation("CustomerService GetCustomerByIdAsync completed");
            return result;
        }

        public async Task<bool> SoftDeleteCustomerAsync(int customerId)
        {
            _logger.LogInformation("CustomerService SoftDeleteCustomerAsync started");

            if (customerId < 1 || customerId > 999999)
            {
                _logger.LogWarning("Invalid customer ID {CustomerId}", customerId);
                return false;
            }

            _logger.LogInformation("Validation passed");
            _logger.LogInformation("Calling repository SoftDeleteCustomerAsync");
            var result = await _customerRepository.SoftDeleteCustomerAsync(customerId);
            _logger.LogInformation("Repository call completed");

            if (result)
            {
                _logger.LogInformation("Customer deleted {CustomerId}", customerId);
                return true;
            }

            _logger.LogWarning("Customer delete failed {CustomerId}", customerId);
            return false;
        }

        public async Task<CustomerProfileDto?> GetCustomerProfileAsync(int customerId)
        {
            try
            {
                _logger.LogInformation(MagicStrings.LogMessages.CustomerProfileRetrievalStarted, customerId);

                if (customerId < 1 || customerId > 999999)
                {
                    _logger.LogWarning("Invalid customer ID {CustomerId}", customerId);
                    return null;
                }

                var customer = await _customerRepository.GetCustomerProfileByIdAsync(customerId);
                if (customer == null)
                {
                    _logger.LogInformation("Customer profile not found for ID {CustomerId}", customerId);
                    return null;
                }

                var result = _mapper.Map<CustomerProfileDto>(customer);
                _logger.LogInformation(MagicStrings.LogMessages.CustomerProfileRetrievalCompleted, customerId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MagicStrings.LogMessages.CustomerProfileRetrievalFailed, customerId, ex.Message);
                throw;
            }
        }

        public async Task<CustomerProfileDto?> GetCustomerProfileByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Customer profile retrieval by UserId started: {UserId}", userId);

                if (userId < 1 || userId > 999999)
                {
                    _logger.LogWarning("Invalid user ID {UserId}", userId);
                    return null;
                }

                var customer = await _customerRepository.GetCustomerProfileByUserIdAsync(userId);
                if (customer == null)
                {
                    _logger.LogInformation("Customer profile not found for UserId {UserId}", userId);
                    return null;
                }

                var result = _mapper.Map<CustomerProfileDto>(customer);
                _logger.LogInformation("Customer profile retrieval by UserId completed: {UserId}", userId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer profile retrieval by UserId failed: {UserId}: {Error}", userId, ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateCustomerProfileAsync(int customerId, UpdateCustomerProfileDto updateDto)
        {
            try
            {
                _logger.LogInformation(MagicStrings.LogMessages.CustomerProfileUpdateStarted, customerId);

                if (customerId < 1 || customerId > 999999)
                {
                    _logger.LogWarning("Invalid customer ID {CustomerId}", customerId);
                    return false;
                }

                // Age validation
                var age = DateTime.Today.Year - updateDto.DateOfBirth.Year;
                if (updateDto.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
                
                if (age < 18 || age > 100)
                {
                    _logger.LogWarning("Invalid age for customer {CustomerId}: {Age}", customerId, age);
                    return false;
                }

                var result = await _customerRepository.UpdateCustomerProfileAsync(
                    customerId,
                    updateDto.FirstName,
                    updateDto.MiddleName,
                    updateDto.LastName,
                    updateDto.DateOfBirth,
                    (int)updateDto.Gender,
                    updateDto.Phone);

                if (result)
                {
                    _logger.LogInformation(MagicStrings.LogMessages.CustomerProfileUpdateCompleted, customerId);
                    return true;
                }

                _logger.LogWarning("Customer profile update failed for ID {CustomerId}", customerId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MagicStrings.LogMessages.CustomerProfileUpdateFailed, customerId, ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateCustomerProfilePictureAsync(int customerId, IFormFile profilePicture)
        {
            try
            {
                _logger.LogInformation("Customer profile picture update started for CustomerId: {CustomerId}", customerId);

                if (customerId < 1 || customerId > 999999)
                {
                    _logger.LogWarning("Invalid customer ID {CustomerId}", customerId);
                    return false;
                }

                // Validate file
                if (profilePicture == null || profilePicture.Length == 0)
                {
                    _logger.LogWarning("Invalid profile picture file for CustomerId: {CustomerId}", customerId);
                    return false;
                }

                // Validate file size (max 5MB)
                if (profilePicture.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning("Profile picture file too large for CustomerId: {CustomerId}, Size: {Size}", customerId, profilePicture.Length);
                    return false;
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(profilePicture.ContentType.ToLower()))
                {
                    _logger.LogWarning("Invalid file type for CustomerId: {CustomerId}, Type: {Type}", customerId, profilePicture.ContentType);
                    return false;
                }

                // Convert to byte array
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await profilePicture.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }

                var result = await _customerRepository.UpdateCustomerProfilePictureAsync(customerId, imageBytes);

                if (result)
                {
                    _logger.LogInformation("Customer profile picture updated successfully for CustomerId: {CustomerId}", customerId);
                    return true;
                }

                _logger.LogWarning("Customer profile picture update failed for CustomerId: {CustomerId}", customerId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer profile picture update failed for CustomerId: {CustomerId}: {Error}", customerId, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<CustomerBookingDto>> GetCustomerBookingsAsync(int customerId, BookingStatus? status, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                _logger.LogInformation("Getting bookings for customer: {CustomerId}", customerId);

                if (customerId < 1 || customerId > 999999)
                {
                    _logger.LogWarning("Invalid customer ID {CustomerId}", customerId);
                    return Enumerable.Empty<CustomerBookingDto>();
                }

                var bookingsWithDetails = await _customerRepository.GetCustomerBookingsWithDetailsAsync(customerId, status, fromDate, toDate);
                
                // Map from BookingWithDetails to CustomerBookingDto
                var result = bookingsWithDetails.Select(bwd => new CustomerBookingDto
                {
                    BookingId = bwd.Booking.BookingId,
                    PNRNo = bwd.Booking.PNRNo,
                    TotalSeats = bwd.Booking.TotalSeats,
                    TotalAmount = bwd.Booking.TotalAmount,
                    TravelDate = bwd.Booking.TravelDate,
                    Status = bwd.Booking.Status,
                    BookedAt = bwd.Booking.BookedAt,
                    BusName = bwd.BusName,
                    Route = bwd.Route,
                    VendorName = bwd.VendorName,
                    DepartureTime = bwd.DepartureTime,
                    ArrivalTime = bwd.ArrivalTime,
                    PaymentMethod = bwd.PaymentMethod,
                    IsPaymentCompleted = bwd.IsPaymentCompleted
                }).ToList();
                
                _logger.LogInformation("Retrieved {Count} bookings for customer: {CustomerId}", result.Count, customerId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<byte[]?> GenerateTicketAsync(int customerId, int bookingId)
        {
            try
            {
                _logger.LogInformation("Generating ticket for customer: {CustomerId}, booking: {BookingId}", customerId, bookingId);

                // Validate customer owns this booking
                var customerBookings = await _customerRepository.GetCustomerBookingsAsync(customerId, null, null, null);
                var booking = customerBookings.FirstOrDefault(b => b.BookingId == bookingId);
                
                if (booking == null)
                {
                    _logger.LogWarning("Booking {BookingId} not found for customer {CustomerId}", bookingId, customerId);
                    return null;
                }

                // Only generate tickets for confirmed bookings
                if (booking.Status != BookingStatus.Confirmed)
                {
                    _logger.LogWarning("Cannot generate ticket for booking {BookingId} with status {Status}", bookingId, booking.Status);
                    return null;
                }

                // Get booking details for PDF generation
                var bookingData = await _emailRepository.GetBookingDetailsForEmailAsync(bookingId);
                if (bookingData == null)
                {
                    _logger.LogWarning("Booking details not found for booking {BookingId}", bookingId);
                    return null;
                }

                // Generate PDF ticket
                var pdfResult = await _pdfService.GenerateBookingTicketAsync(bookingData);
                if (pdfResult.IsFailure)
                {
                    _logger.LogError("PDF generation failed for booking {BookingId}: {Error}", bookingId, pdfResult.Error);
                    return null;
                }

                _logger.LogInformation("Ticket generated successfully for customer: {CustomerId}, booking: {BookingId}", customerId, bookingId);
                return pdfResult.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating ticket for customer: {CustomerId}, booking: {BookingId}", customerId, bookingId);
                throw;
            }
        }

        public async Task<CancelBookingResponseDto> CancelBookingAsync(int customerId, CancelBookingRequestDto request)
        {
            try
            {
                _logger.LogInformation(MagicStrings.LogMessages.BookingCancellationStarted, request.BookingId);

                // Validate customer and booking
                var bookingData = await _cancellationRepository.GetBookingForCancellationAsync(request.BookingId, customerId);
                if (bookingData == null)
                {
                    _logger.LogWarning(MagicStrings.LogMessages.BookingNotFoundForCancellation, request.BookingId, customerId);
                    return new CancelBookingResponseDto
                    {
                        IsSuccess = false,
                        Message = MagicStrings.ErrorMessages.BookingNotFound
                    };
                }

                // Check if booking is already cancelled
                if (bookingData.BookingStatus == (int)BookingStatus.Cancelled)
                {
                    return new CancelBookingResponseDto
                    {
                        IsSuccess = false,
                        Message = MagicStrings.ErrorMessages.BookingAlreadyCancelled
                    };
                }

                // Check cancellation policy (2 hours before travel)
                if (bookingData.HoursUntilTravel <= 2)
                {
                    return new CancelBookingResponseDto
                    {
                        IsSuccess = false,
                        Message = MagicStrings.ErrorMessages.CancellationNotAllowed
                    };
                }

                // Calculate penalty and refund
                var (penaltyAmount, refundAmount) = CalculateCancellationAmounts(bookingData.TotalAmount, bookingData.HoursUntilTravel);

                // Cancel the booking
                var cancellationResult = await _cancellationRepository.CancelBookingAsync(
                    request.BookingId, 
                    request.Reason, 
                    penaltyAmount, 
                    $"Customer_{customerId}");

                if (!cancellationResult)
                {
                    return new CancelBookingResponseDto
                    {
                        IsSuccess = false,
                        Message = MagicStrings.ErrorMessages.BookingCannotBeCancelled
                    };
                }

                _logger.LogInformation(MagicStrings.LogMessages.BookingCancellationCompleted, request.BookingId);

                return new CancelBookingResponseDto
                {
                    IsSuccess = true,
                    Message = MagicStrings.SuccessMessages.BookingCancelledSuccessfully,
                    RefundAmount = refundAmount,
                    PenaltyAmount = penaltyAmount,
                    RefundMethod = GetRefundMethod(bookingData.PaymentMethod),
                    ProcessedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MagicStrings.LogMessages.BookingCancellationFailed, request.BookingId, ex.Message);
                throw;
            }
        }

        private static (decimal penaltyAmount, decimal refundAmount) CalculateCancellationAmounts(decimal totalAmount, int hoursUntilTravel)
        {
            decimal penaltyPercentage = hoursUntilTravel switch
            {
                >= 24 => 0.10m,  // 10% penalty if cancelled 24+ hours before
                >= 12 => 0.25m,  // 25% penalty if cancelled 12-24 hours before
                >= 6 => 0.50m,   // 50% penalty if cancelled 6-12 hours before
                >= 2 => 0.75m,   // 75% penalty if cancelled 2-6 hours before
                _ => 1.00m        // 100% penalty (no refund) if less than 2 hours
            };

            var penaltyAmount = totalAmount * penaltyPercentage;
            var refundAmount = totalAmount - penaltyAmount;

            return (penaltyAmount, refundAmount);
        }

        private static string GetRefundMethod(int paymentMethod)
        {
            return paymentMethod switch
            {
                2 => "UPI Refund (3-5 business days)",
                3 => "Card Refund (5-7 business days)",
                4 => "Bank Transfer (3-5 business days)",
                _ => "Original Payment Method (3-7 business days)"
            };
        }
    }
}