using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Services;
using Kanini.RouteBuddy.Application.Services.Buses;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kanini.RouteBuddy.UnitTests.Api.Services.Payment;

[TestFixture]
public class PaymentServiceTests
{
    private Mock<IPaymentRepository> _mockPaymentRepository;
    private Mock<IBus_Search_Book_Service> _mockBusService;
    private Mock<IEmailService> _mockEmailService;
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<PaymentService>> _mockLogger;
    private Mock<IConfiguration> _mockConfiguration;
    private PaymentService _service;

    [SetUp]
    public void Setup()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockBusService = new Mock<IBus_Search_Book_Service>();
        _mockEmailService = new Mock<IEmailService>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<PaymentService>>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(x => x["RazorpaySettings:KeyId"]).Returns("test_key_id");
        _mockConfiguration.Setup(x => x["RazorpaySettings:KeySecret"]).Returns("test_key_secret");

        _service = new PaymentService(_mockPaymentRepository.Object, _mockBusService.Object,
            _mockEmailService.Object, _mockMapper.Object, _mockLogger.Object, _mockConfiguration.Object);
    }

    [Test]
    public async Task InitiatePaymentAsync_BookingNotPending_ReturnsFailure()
    {
        // Arrange
        var request = new PaymentInitiateRequestDto { BookingId = 1, PaymentMethod = PaymentMethod.UPI };
        var booking = new Domain.Entities.Booking { BookingId = 1, Status = BookingStatus.Confirmed, TotalAmount = 500 };
        
        _mockBusService.Setup(x => x.GetBookingDetailsAsync(1)).ReturnsAsync(Result.Success(booking));

        // Act
        var result = await _service.InitiatePaymentAsync(request, 1, "test_user");

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task InitiatePaymentAsync_BookingNotFound_ReturnsFailure()
    {
        // Arrange
        var request = new PaymentInitiateRequestDto { BookingId = 999, PaymentMethod = PaymentMethod.UPI };
        
        _mockBusService.Setup(x => x.GetBookingDetailsAsync(999))
            .ReturnsAsync(Result.Failure<Domain.Entities.Booking>(Error.NotFound("Booking.NotFound", "Booking not found")));

        // Act
        var result = await _service.InitiatePaymentAsync(request, 1, "test_user");

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task VerifyPaymentAsync_PaymentNotFound_ReturnsFailure()
    {
        // Arrange
        var request = new PaymentVerifyRequestDto 
        { 
            BookingId = 1, 
            RazorpayOrderId = "order_123",
            RazorpayPaymentId = "pay_123",
            RazorpaySignature = "sig_123"
        };
        
        _mockPaymentRepository.Setup(x => x.GetPaymentByTransactionIdAsync("order_123"))
            .ReturnsAsync(Result.Failure<Domain.Entities.Payment>(Error.NotFound("Payment.NotFound", "Payment not found")));

        // Act
        var result = await _service.VerifyPaymentAsync(request, "test_user");

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task VerifyPaymentAsync_BookingIdMismatch_ReturnsFailure()
    {
        // Arrange
        var request = new PaymentVerifyRequestDto 
        { 
            BookingId = 1, 
            RazorpayOrderId = "order_123",
            RazorpayPaymentId = "pay_123",
            RazorpaySignature = "sig_123"
        };
        var payment = new Domain.Entities.Payment { PaymentId = 1, BookingId = 2 };
        
        _mockPaymentRepository.Setup(x => x.GetPaymentByTransactionIdAsync("order_123"))
            .ReturnsAsync(Result.Success(payment));

        // Act
        var result = await _service.VerifyPaymentAsync(request, "test_user");

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task GetPaymentsByBookingIdAsync_ValidBookingId_ReturnsPayments()
    {
        // Arrange
        var bookingId = 1;
        var payments = new List<Domain.Entities.Payment>
        {
            new Domain.Entities.Payment { PaymentId = 1, BookingId = 1, Amount = 500 },
            new Domain.Entities.Payment { PaymentId = 2, BookingId = 1, Amount = 100 }
        };
        var paymentDtos = new List<PaymentResponseDto>
        {
            new PaymentResponseDto { PaymentId = 1, BookingId = 1, Amount = 500 },
            new PaymentResponseDto { PaymentId = 2, BookingId = 1, Amount = 100 }
        };

        _mockPaymentRepository.Setup(x => x.GetPaymentsByBookingIdAsync(bookingId))
            .ReturnsAsync(Result.Success(payments));
        _mockMapper.Setup(x => x.Map<List<PaymentResponseDto>>(payments)).Returns(paymentDtos);

        // Act
        var result = await _service.GetPaymentsByBookingIdAsync(bookingId);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Count, Is.EqualTo(2));
        Assert.That(result.Value[0].PaymentId, Is.EqualTo(1));
        Assert.That(result.Value[1].PaymentId, Is.EqualTo(2));
    }

    [Test]
    public async Task GetPaymentsByBookingIdAsync_NoPayments_ReturnsFailure()
    {
        // Arrange
        var bookingId = 999;
        _mockPaymentRepository.Setup(x => x.GetPaymentsByBookingIdAsync(bookingId))
            .ReturnsAsync(Result.Failure<List<Domain.Entities.Payment>>(Error.NotFound("Payments.NotFound", "No payments found")));

        // Act
        var result = await _service.GetPaymentsByBookingIdAsync(bookingId);

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task GetPaymentsByBookingIdAsync_RepositoryException_ReturnsFailure()
    {
        // Arrange
        var bookingId = 1;
        _mockPaymentRepository.Setup(x => x.GetPaymentsByBookingIdAsync(bookingId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.GetPaymentsByBookingIdAsync(bookingId);

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task InitiatePaymentAsync_RepositoryException_ReturnsFailure()
    {
        // Arrange
        var request = new PaymentInitiateRequestDto { BookingId = 1, PaymentMethod = PaymentMethod.UPI };
        
        _mockBusService.Setup(x => x.GetBookingDetailsAsync(1))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.InitiatePaymentAsync(request, 1, "test_user");

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task VerifyPaymentAsync_RepositoryException_ReturnsFailure()
    {
        // Arrange
        var request = new PaymentVerifyRequestDto 
        { 
            BookingId = 1, 
            RazorpayOrderId = "order_123",
            RazorpayPaymentId = "pay_123",
            RazorpaySignature = "sig_123"
        };
        
        _mockPaymentRepository.Setup(x => x.GetPaymentByTransactionIdAsync("order_123"))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.VerifyPaymentAsync(request, "test_user");

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }
}