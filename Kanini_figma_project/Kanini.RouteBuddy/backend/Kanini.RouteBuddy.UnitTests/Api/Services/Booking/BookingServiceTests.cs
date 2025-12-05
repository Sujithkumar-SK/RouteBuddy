using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Application.Services.Booking;
using Kanini.RouteBuddy.Data.Repositories.Booking;
using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kanini.RouteBuddy.UnitTests.Api.Services.Booking;

[TestFixture]
public class BookingServiceTests
{
    private Mock<IBookingRepository> _mockRepository;
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<BookingService>> _mockLogger;
    private BookingService _service;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _service = new BookingService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetAllBookingsAsync_ReturnsBookings()
    {
        // Arrange
        var bookings = new List<Domain.Entities.Booking> { new Domain.Entities.Booking { BookingId = 1 } };
        var bookingDtos = new List<AdminBookingDTO> { new AdminBookingDTO { BookingId = 1 } };
        
        _mockRepository.Setup(x => x.GetAllBookingsAsync()).ReturnsAsync(bookings);
        _mockMapper.Setup(x => x.Map<IEnumerable<AdminBookingDTO>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetAllBookingsAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().BookingId, Is.EqualTo(1));
    }

    [Test]
    public async Task FilterBookingsAsync_ReturnsFilteredBookings()
    {
        // Arrange
        var bookings = new List<Domain.Entities.Booking> { new Domain.Entities.Booking { BookingId = 1 } };
        var bookingDtos = new List<AdminBookingDTO> { new AdminBookingDTO { BookingId = 1 } };
        
        _mockRepository.Setup(x => x.FilterBookingsAsync("test", 1, null, null)).ReturnsAsync(bookings);
        _mockMapper.Setup(x => x.Map<IEnumerable<AdminBookingDTO>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.FilterBookingsAsync("test", 1, null, null);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetBookingByIdAsync_InvalidId_ReturnsNull()
    {
        // Act
        var result = await _service.GetBookingByIdAsync(0);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetBookingByIdAsync_ValidId_ReturnsBooking()
    {
        // Arrange
        var booking = new Domain.Entities.Booking { BookingId = 1 };
        var bookingDto = new AdminBookingDTO { BookingId = 1 };
        
        _mockRepository.Setup(x => x.GetBookingByIdAsync(1)).ReturnsAsync(booking);
        _mockMapper.Setup(x => x.Map<AdminBookingDTO>(booking)).Returns(bookingDto);

        // Act
        var result = await _service.GetBookingByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.BookingId, Is.EqualTo(1));
    }

    [Test]
    public async Task GetBookingByIdAsync_BookingNotFound_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetBookingByIdAsync(1)).ReturnsAsync((Domain.Entities.Booking)null);

        // Act
        var result = await _service.GetBookingByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetBookingStatusSummaryAsync_ReturnsSummary()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetBookingStatusSummaryAsync())
            .ReturnsAsync((10, 20, 5, 35, 15000m));

        // Act
        var result = await _service.GetBookingStatusSummaryAsync();

        // Assert
        Assert.That(result.PendingBookings, Is.EqualTo(10));
        Assert.That(result.ConfirmedBookings, Is.EqualTo(20));
        Assert.That(result.CancelledBookings, Is.EqualTo(5));
        Assert.That(result.TotalBookings, Is.EqualTo(35));
        Assert.That(result.TotalRevenue, Is.EqualTo(15000m));
    }
}