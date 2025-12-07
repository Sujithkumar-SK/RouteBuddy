using NUnit.Framework;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Kanini.RouteBuddy.Application.Services.Customer;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Data.Repositories.Customer;
using Kanini.RouteBuddy.Data.Repositories.Email;
using Kanini.RouteBuddy.Data.Repositories.Booking;
using Kanini.RouteBuddy.Application.Services.Pdf;
using Kanini.RouteBuddy.Domain.Enums;
using Kanini.RouteBuddy.Common.Utility;
using CustomerEntity = Kanini.RouteBuddy.Domain.Entities.Customer;

namespace Kanini.RouteBuddy.UnitTests.Api.Services.Customer;

[TestFixture]
public class CustomerServiceTests
{
    private Mock<ICustomerRepository> _mockCustomerRepository;
    private Mock<IEmailRepository> _mockEmailRepository;
    private Mock<IBookingCancellationRepository> _mockCancellationRepository;
    private Mock<IPdfService> _mockPdfService;
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<CustomerService>> _mockLogger;
    private CustomerService _customerService;

    [SetUp]
    public void Setup()
    {
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockEmailRepository = new Mock<IEmailRepository>();
        _mockCancellationRepository = new Mock<IBookingCancellationRepository>();
        _mockPdfService = new Mock<IPdfService>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<CustomerService>>();
        
        _customerService = new CustomerService(
            _mockCustomerRepository.Object,
            _mockEmailRepository.Object,
            _mockCancellationRepository.Object,
            _mockPdfService.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }

    [Test]
    public async Task GetAllCustomersAsync_WithValidData_ReturnsCustomers()
    {
        // Arrange
        var customers = new List<CustomerEntity> { new CustomerEntity() };
        var customerDtos = new List<AdminCustomerDTO> { new AdminCustomerDTO() };

        _mockCustomerRepository.Setup(x => x.GetAllCustomersAsync()).ReturnsAsync(customers);
        _mockMapper.Setup(x => x.Map<IEnumerable<AdminCustomerDTO>>(customers)).Returns(customerDtos);

        // Act
        var result = await _customerService.GetAllCustomersAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task FilterCustomersAsync_WithValidParameters_ReturnsFilteredCustomers()
    {
        // Arrange
        var searchName = "John";
        var isActive = true;
        var minAge = 18;
        var maxAge = 65;
        var customers = new List<CustomerEntity> { new CustomerEntity() };
        var customerDtos = new List<AdminCustomerDTO> { new AdminCustomerDTO() };

        _mockCustomerRepository.Setup(x => x.FilterCustomersAsync(searchName, isActive, minAge, maxAge))
            .ReturnsAsync(customers);
        _mockMapper.Setup(x => x.Map<IEnumerable<AdminCustomerDTO>>(customers)).Returns(customerDtos);

        // Act
        var result = await _customerService.FilterCustomersAsync(searchName, isActive, minAge, maxAge);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetCustomerByIdAsync_WithValidId_ReturnsCustomer()
    {
        // Arrange
        var customerId = 1;
        var customer = new CustomerEntity();
        var customerDto = new AdminCustomerDTO();

        _mockCustomerRepository.Setup(x => x.GetCustomerByIdAsync(customerId)).ReturnsAsync(customer);
        _mockMapper.Setup(x => x.Map<AdminCustomerDTO>(customer)).Returns(customerDto);

        // Act
        var result = await _customerService.GetCustomerByIdAsync(customerId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetCustomerByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var customerId = 0;

        // Act
        var result = await _customerService.GetCustomerByIdAsync(customerId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetCustomerByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var customerId = 999;
        _mockCustomerRepository.Setup(x => x.GetCustomerByIdAsync(customerId)).ReturnsAsync((CustomerEntity?)null);

        // Act
        var result = await _customerService.GetCustomerByIdAsync(customerId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task SoftDeleteCustomerAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var customerId = 1;
        _mockCustomerRepository.Setup(x => x.SoftDeleteCustomerAsync(customerId)).ReturnsAsync(true);

        // Act
        var result = await _customerService.SoftDeleteCustomerAsync(customerId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task SoftDeleteCustomerAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var customerId = 0;

        // Act
        var result = await _customerService.SoftDeleteCustomerAsync(customerId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetCustomerProfileAsync_WithValidId_ReturnsProfile()
    {
        // Arrange
        var customerId = 1;
        var customer = new CustomerEntity();
        var profileDto = new CustomerProfileDto();

        _mockCustomerRepository.Setup(x => x.GetCustomerProfileByIdAsync(customerId)).ReturnsAsync(customer);
        _mockMapper.Setup(x => x.Map<CustomerProfileDto>(customer)).Returns(profileDto);

        // Act
        var result = await _customerService.GetCustomerProfileAsync(customerId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetCustomerProfileByUserIdAsync_WithValidUserId_ReturnsProfile()
    {
        // Arrange
        var userId = 1;
        var customer = new CustomerEntity();
        var profileDto = new CustomerProfileDto();

        _mockCustomerRepository.Setup(x => x.GetCustomerProfileByUserIdAsync(userId)).ReturnsAsync(customer);
        _mockMapper.Setup(x => x.Map<CustomerProfileDto>(customer)).Returns(profileDto);

        // Act
        var result = await _customerService.GetCustomerProfileByUserIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task UpdateCustomerProfileAsync_WithValidData_ReturnsTrue()
    {
        // Arrange
        var customerId = 1;
        var updateDto = new UpdateCustomerProfileDto
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.Now.AddYears(-25),
            Gender = Gender.Male,
            Phone = "1234567890"
        };

        _mockCustomerRepository.Setup(x => x.UpdateCustomerProfileAsync(
            customerId, 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<DateTime>(), 
            It.IsAny<int>(), 
            It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _customerService.UpdateCustomerProfileAsync(customerId, updateDto);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task UpdateCustomerProfileAsync_WithInvalidAge_ReturnsFalse()
    {
        // Arrange
        var customerId = 1;
        var updateDto = new UpdateCustomerProfileDto
        {
            DateOfBirth = DateTime.Now.AddYears(-10), // Too young
            Gender = Gender.Male
        };

        // Act
        var result = await _customerService.UpdateCustomerProfileAsync(customerId, updateDto);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateCustomerProfilePictureAsync_WithValidFile_ReturnsTrue()
    {
        // Arrange
        var customerId = 1;
        var mockFile = new Mock<IFormFile>();
        var content = "fake image content";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        mockFile.Setup(f => f.Length).Returns(ms.Length);
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream stream, CancellationToken token) => ms.CopyToAsync(stream, token));

        _mockCustomerRepository.Setup(x => x.UpdateCustomerProfilePictureAsync(customerId, It.IsAny<byte[]>()))
            .ReturnsAsync(true);

        // Act
        var result = await _customerService.UpdateCustomerProfilePictureAsync(customerId, mockFile.Object);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CancelBookingAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var customerId = 1;
        var request = new CancelBookingRequestDto { BookingId = 1, Reason = "Change of plans" };
        var bookingData = new BookingCancellationData
        {
            BookingStatus = (int)BookingStatus.Confirmed,
            HoursUntilTravel = 25,
            TotalAmount = 1000,
            PaymentMethod = 2
        };

        _mockCancellationRepository.Setup(x => x.GetBookingForCancellationAsync(request.BookingId, customerId))
            .ReturnsAsync(bookingData);
        _mockCancellationRepository.Setup(x => x.CancelBookingAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _customerService.CancelBookingAsync(customerId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.RefundAmount, Is.GreaterThan(0));
    }

    [Test]
    public async Task CancelBookingAsync_WithNonExistentBooking_ReturnsFailureResponse()
    {
        // Arrange
        var customerId = 1;
        var request = new CancelBookingRequestDto { BookingId = 999, Reason = "Change of plans" };

        _mockCancellationRepository.Setup(x => x.GetBookingForCancellationAsync(request.BookingId, customerId))
            .ReturnsAsync((BookingCancellationData?)null);

        // Act
        var result = await _customerService.CancelBookingAsync(customerId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public async Task GetAllCustomersAsync_WithException_ThrowsException()
    {
        // Arrange
        _mockCustomerRepository.Setup(x => x.GetAllCustomersAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _customerService.GetAllCustomersAsync());
    }

    [Test]
    public async Task GetCustomerProfileAsync_WithException_ThrowsException()
    {
        // Arrange
        var customerId = 1;
        _mockCustomerRepository.Setup(x => x.GetCustomerProfileByIdAsync(customerId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _customerService.GetCustomerProfileAsync(customerId));
    }
}