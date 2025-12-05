using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Services.Buses;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Razorpay.Api;
using PaymentEntity = Kanini.RouteBuddy.Domain.Entities.Payment;

namespace Kanini.RouteBuddy.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBus_Search_Book_Service _busService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentService> _logger;
    private readonly RazorpayClient _razorpayClient;
    private readonly string _razorpayKey;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IBus_Search_Book_Service busService,
        IEmailService emailService,
        IMapper mapper,
        ILogger<PaymentService> logger,
        IConfiguration configuration)
    {
        _paymentRepository = paymentRepository;
        _busService = busService;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;

        _razorpayKey = configuration["RazorpaySettings:KeyId"]!;
        var razorpaySecret = configuration["RazorpaySettings:KeySecret"]!;
        _razorpayClient = new RazorpayClient(_razorpayKey, razorpaySecret);
    }

    public async Task<Result<PaymentInitiateResponseDto>> InitiatePaymentAsync(
        PaymentInitiateRequestDto request, 
        int customerId, 
        string createdBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.PaymentInitiationStarted, request.BookingId);

            // Get booking details to validate and get amount
            var bookingResult = await _busService.GetBookingDetailsAsync(request.BookingId);
            if (bookingResult.IsFailure)
            {
                return Result.Failure<PaymentInitiateResponseDto>(bookingResult.Error);
            }

            var booking = bookingResult.Value;
            
            // Validate booking belongs to customer and is pending
            if (booking.Status != BookingStatus.Pending)
            {
                return Result.Failure<PaymentInitiateResponseDto>(
                    Error.Failure(MagicStrings.ErrorCodes.InvalidBookingStatus, 
                    "Booking is not in pending status"));
            }

            // Create Razorpay order
            var razorpayOrder = new Dictionary<string, object>
            {
                { "amount", (int)(booking.TotalAmount * 100) }, // Convert to paise
                { "currency", "INR" },
                { "receipt", $"booking_{booking.BookingId}_{DateTime.UtcNow:yyyyMMddHHmmss}" }
            };

            var createdRazorpayOrder = _razorpayClient.Order.Create(razorpayOrder);

            // Create payment record
            var payment = new PaymentEntity
            {
                BookingId = request.BookingId,
                Amount = booking.TotalAmount,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                TransactionId = createdRazorpayOrder["id"].ToString()!,
                PaymentDate = DateTime.UtcNow,
                IsActive = false,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow
            };

            var paymentResult = await _paymentRepository.CreatePaymentAsync(payment);
            if (paymentResult.IsFailure)
            {
                return Result.Failure<PaymentInitiateResponseDto>(paymentResult.Error);
            }

            var response = new PaymentInitiateResponseDto
            {
                RazorpayOrderId = createdRazorpayOrder["id"].ToString()!,
                Amount = booking.TotalAmount,
                Key = _razorpayKey,
                Description = $"Bus Booking Payment - PNR: {booking.PNRNo}",
                PrefillName = "Customer",
                PrefillEmail = "customer@routebuddy.com",
                PrefillContact = "9999999999",
                BookingId = request.BookingId
            };

            _logger.LogInformation(MagicStrings.LogMessages.PaymentInitiationCompleted, paymentResult.Value.PaymentId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentInitiationFailed, ex.Message);
            return Result.Failure<PaymentInitiateResponseDto>(
                Error.Failure("Payment.UnexpectedError", 
                MagicStrings.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<PaymentResponseDto>> VerifyPaymentAsync(
        PaymentVerifyRequestDto request, 
        string updatedBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.PaymentVerificationStarted, request.RazorpayPaymentId);

            // Verify payment signature
            var attributes = new Dictionary<string, string>
            {
                { "razorpay_order_id", request.RazorpayOrderId },
                { "razorpay_payment_id", request.RazorpayPaymentId },
                { "razorpay_signature", request.RazorpaySignature }
            };

            Utils.verifyPaymentSignature(attributes);

            // Get payment by transaction ID (Razorpay order ID)
            _logger.LogInformation("Searching for payment with TransactionId: {TransactionId}", request.RazorpayOrderId);
            var paymentResult = await _paymentRepository.GetPaymentByTransactionIdAsync(request.RazorpayOrderId);
            if (paymentResult.IsFailure)
            {
                _logger.LogError("Payment not found for TransactionId: {TransactionId}, Error: {Error}", request.RazorpayOrderId, paymentResult.Error.Description);
                return Result.Failure<PaymentResponseDto>(paymentResult.Error);
            }
            _logger.LogInformation("Found payment with PaymentId: {PaymentId} for TransactionId: {TransactionId}", paymentResult.Value.PaymentId, request.RazorpayOrderId);

            var payment = paymentResult.Value;

            // Validate booking ID matches
            if (payment.BookingId != request.BookingId)
            {
                return Result.Failure<PaymentResponseDto>(
                    Error.Failure(MagicStrings.ErrorCodes.InvalidBookingId, 
                    "Payment booking ID mismatch"));
            }

            // Update payment status to success
            var updateResult = await _paymentRepository.UpdatePaymentStatusAsync(
                payment.PaymentId, 
                (int)PaymentStatus.Success, 
                request.RazorpayPaymentId, 
                updatedBy);

            if (updateResult.IsFailure)
            {
                return Result.Failure<PaymentResponseDto>(updateResult.Error);
            }

            // Confirm booking after successful payment
            var confirmationDto = new BookingConfirmationDto
            {
                BookingId = request.BookingId,
                PaymentReferenceId = request.RazorpayPaymentId,
                PaymentMethod = payment.PaymentMethod,
                IsPaymentSuccessful = true
            };

            var confirmResult = await _busService.ConfirmBookingAsync(confirmationDto);
            if (confirmResult.IsFailure)
            {
                _logger.LogError("Failed to confirm booking after payment: {Error}", confirmResult.Error?.Description);
                // Payment was successful but booking confirmation failed - needs manual intervention
            }
            else
            {
                _logger.LogInformation("Booking {BookingId} confirmed after successful payment", request.BookingId);
                
                // Send confirmation email asynchronously
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendBookingConfirmationAsync(request.BookingId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send booking confirmation email for BookingId: {BookingId}", request.BookingId);
                    }
                });
            }

            var responseDto = _mapper.Map<PaymentResponseDto>(updateResult.Value);
            
            _logger.LogInformation(MagicStrings.LogMessages.PaymentVerificationCompleted, request.RazorpayPaymentId);
            return Result.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentVerificationFailed, ex.Message);
            return Result.Failure<PaymentResponseDto>(
                Error.Failure(MagicStrings.ErrorCodes.PaymentFailed, 
                MagicStrings.ErrorMessages.PaymentFailed));
        }
    }

    public async Task<Result<List<PaymentResponseDto>>> GetPaymentsByBookingIdAsync(int bookingId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsStarted, bookingId);

            var result = await _paymentRepository.GetPaymentsByBookingIdAsync(bookingId);
            if (result.IsFailure)
            {
                return Result.Failure<List<PaymentResponseDto>>(result.Error);
            }

            var paymentDtos = _mapper.Map<List<PaymentResponseDto>>(result.Value);

            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsCompleted, bookingId);
            return Result.Success(paymentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
            return Result.Failure<List<PaymentResponseDto>>(
                Error.Failure("Payment.UnexpectedError", 
                MagicStrings.ErrorMessages.UnexpectedError));
        }
    }
}