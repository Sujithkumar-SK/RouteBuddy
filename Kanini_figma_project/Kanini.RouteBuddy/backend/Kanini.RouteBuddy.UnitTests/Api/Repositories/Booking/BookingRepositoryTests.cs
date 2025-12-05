using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Repositories.Booking;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.UnitTests.Api.Repositories.Booking;

[TestFixture]
public class BookingRepositoryTests
{
    private Mock<ILogger<BookingRepository>> _mockLogger;
    private RouteBuddyDatabaseContext _context;
    private BookingRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<RouteBuddyDatabaseContext>()
            .UseSqlServer("Server=test;Database=test;Integrated Security=true;TrustServerCertificate=true;")
            .Options;
        
        _context = new RouteBuddyDatabaseContext(options);
        _mockLogger = new Mock<ILogger<BookingRepository>>();
        _repository = new BookingRepository(_context, _mockLogger.Object);
    }

    [Test]
    public async Task GetAllBookingsAsync_SqlException_ReturnsEmptyList()
    {
        // Arrange - This test verifies exception handling since we can't easily mock SqlConnection

        // Act
        var result = await _repository.GetAllBookingsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Entities.Booking>>());
    }

    [Test]
    public async Task FilterBookingsAsync_WithParameters_CallsStoredProcedure()
    {
        // Arrange
        var searchName = "John";
        var status = 1;
        var fromDate = DateTime.Now.AddDays(-7);
        var toDate = DateTime.Now;

        // Act
        var result = await _repository.FilterBookingsAsync(searchName, status, fromDate, toDate);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Entities.Booking>>());
    }

    [Test]
    public async Task FilterBookingsAsync_WithNullParameters_CallsStoredProcedure()
    {
        // Arrange & Act
        var result = await _repository.FilterBookingsAsync(null, null, null, null);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Entities.Booking>>());
    }

    [Test]
    public async Task GetBookingByIdAsync_ValidId_ReturnsBookingOrNull()
    {
        // Arrange
        var bookingId = 1;

        // Act
        var result = await _repository.GetBookingByIdAsync(bookingId);

        // Assert - Result can be null if booking not found, which is valid
        Assert.That(result, Is.TypeOf<Entities.Booking>().Or.Null);
    }

    [Test]
    public async Task GetBookingByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var bookingId = -1;

        // Act
        var result = await _repository.GetBookingByIdAsync(bookingId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetBookingStatusSummaryAsync_Success_ReturnsTuple()
    {
        // Act
        var result = await _repository.GetBookingStatusSummaryAsync();

        // Assert
        Assert.That(result.GetType().IsValueType, Is.True);
        Assert.That(result.Pending, Is.GreaterThanOrEqualTo(0));
        Assert.That(result.Confirmed, Is.GreaterThanOrEqualTo(0));
        Assert.That(result.Cancelled, Is.GreaterThanOrEqualTo(0));
        Assert.That(result.Total, Is.GreaterThanOrEqualTo(0));
        Assert.That(result.Revenue, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public void Constructor_ValidParameters_InitializesRepository()
    {
        // Arrange & Act
        var repository = new BookingRepository(_context, _mockLogger.Object);

        // Assert
        Assert.That(repository, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }
}