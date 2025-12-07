using System.Security.Claims;
using Kanini.RouteBuddy.Api.Controllers;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Services;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kanini.RouteBuddy.UnitTests.Api.Controllers.Payment;

[TestFixture]
public class PaymentControllerTests
{
    private Mock<IPaymentService> _mockPaymentService;
    private Mock<ILogger<PaymentController>> _mockLogger;
    private PaymentController _controller;

    [SetUp]
    public void Setup()
    {
        _mockPaymentService = new Mock<IPaymentService>();
        _mockLogger = new Mock<ILogger<PaymentController>>();
        _controller = new PaymentController(_mockPaymentService.Object, _mockLogger.Object);
        
        SetupHttpContext();
    }

    private void SetupHttpContext()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [Test]
    public async Task InitiatePayment_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new PaymentInitiateRequestDto { BookingId = 1 };
        var paymentResponse = new PaymentInitiateResponseDto();
        _mockPaymentService.Setup(x => x.InitiatePaymentAsync(request, 1, "1"))
            .ReturnsAsync(Result.Success(paymentResponse));

        // Act
        var response = await _controller.InitiatePayment(request);

        // Assert
        Assert.That(response, Is.InstanceOf<OkObjectResult>());
        var okResult = response as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(paymentResponse));
    }

    [Test]
    public async Task InitiatePayment_InvalidCustomerId_ReturnsBadRequest()
    {
        // Arrange
        var request = new PaymentInitiateRequestDto { BookingId = 1 };
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;

        // Act
        var response = await _controller.InitiatePayment(request);

        // Assert
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task InitiatePayment_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = new PaymentInitiateRequestDto { BookingId = 1 };
        _mockPaymentService.Setup(x => x.InitiatePaymentAsync(request, 1, "1"))
            .ReturnsAsync(Result.Failure<PaymentInitiateResponseDto>(Error.Failure("Payment.Failed", "Payment initiation failed")));

        // Act
        var response = await _controller.InitiatePayment(request);

        // Assert
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task InitiatePayment_Exception_ReturnsInternalServerError()
    {
        // Arrange
        var request = new PaymentInitiateRequestDto { BookingId = 1 };
        
        _mockPaymentService.Setup(x => x.InitiatePaymentAsync(request, 1, "1"))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var response = await _controller.InitiatePayment(request);

        // Assert
        Assert.That(response, Is.InstanceOf<ObjectResult>());
        var objectResult = response as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task VerifyPayment_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new PaymentVerifyRequestDto { RazorpayPaymentId = "pay_123" };
        var payment = new PaymentResponseDto();
        _mockPaymentService.Setup(x => x.VerifyPaymentAsync(request, "1"))
            .ReturnsAsync(Result.Success(payment));

        // Act
        var response = await _controller.VerifyPayment(request);

        // Assert
        Assert.That(response, Is.InstanceOf<OkObjectResult>());
        var okResult = response as OkObjectResult;
        Assert.That(okResult.Value, Is.Not.Null);
    }

    [Test]
    public async Task VerifyPayment_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = new PaymentVerifyRequestDto { RazorpayPaymentId = "pay_123" };
        _mockPaymentService.Setup(x => x.VerifyPaymentAsync(request, "1"))
            .ReturnsAsync(Result.Failure<PaymentResponseDto>(Error.Failure("Payment.VerificationFailed", "Payment verification failed")));

        // Act
        var response = await _controller.VerifyPayment(request);

        // Assert
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task VerifyPayment_Exception_ReturnsInternalServerError()
    {
        // Arrange
        var request = new PaymentVerifyRequestDto { RazorpayPaymentId = "pay_123" };
        
        _mockPaymentService.Setup(x => x.VerifyPaymentAsync(request, "1"))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var response = await _controller.VerifyPayment(request);

        // Assert
        Assert.That(response, Is.InstanceOf<ObjectResult>());
        var objectResult = response as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task GetPaymentsByBookingId_ValidId_ReturnsOk()
    {
        // Arrange
        var bookingId = 1;
        var payments = new List<PaymentResponseDto>();
        _mockPaymentService.Setup(x => x.GetPaymentsByBookingIdAsync(bookingId))
            .ReturnsAsync(Result.Success(payments));

        // Act
        var response = await _controller.GetPaymentsByBookingId(bookingId);

        // Assert
        Assert.That(response, Is.InstanceOf<OkObjectResult>());
        var okResult = response as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(payments));
    }

    [Test]
    public async Task GetPaymentsByBookingId_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var bookingId = 0;

        // Act
        var response = await _controller.GetPaymentsByBookingId(bookingId);

        // Assert
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task GetPaymentsByBookingId_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var bookingId = 1;
        _mockPaymentService.Setup(x => x.GetPaymentsByBookingIdAsync(bookingId))
            .ReturnsAsync(Result.Failure<List<PaymentResponseDto>>(Error.NotFound("Payment.NotFound", "Payments not found")));

        // Act
        var response = await _controller.GetPaymentsByBookingId(bookingId);

        // Assert
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task GetPaymentsByBookingId_Exception_ReturnsInternalServerError()
    {
        // Arrange
        var bookingId = 1;
        
        _mockPaymentService.Setup(x => x.GetPaymentsByBookingIdAsync(bookingId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var response = await _controller.GetPaymentsByBookingId(bookingId);

        // Assert
        Assert.That(response, Is.InstanceOf<ObjectResult>());
        var objectResult = response as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }
}