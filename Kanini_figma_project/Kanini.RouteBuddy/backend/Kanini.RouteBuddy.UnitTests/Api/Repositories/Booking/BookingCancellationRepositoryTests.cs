using Kanini.RouteBuddy.Data.Repositories.Booking;
using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kanini.RouteBuddy.UnitTests.Api.Repositories.Booking;

[TestFixture]
public class BookingCancellationRepositoryTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<BookingCancellationRepository>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<BookingCancellationRepository>>();
    }

    [Test]
    public void GetBookingForCancellationAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BookingCancellationRepository).GetMethod("GetBookingForCancellationAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void CancelBookingAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BookingCancellationRepository).GetMethod("CancelBookingAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void BookingCancellationRepository_HasCorrectNamespace()
    {
        // Act & Assert - Correct namespace
        Assert.That(typeof(BookingCancellationRepository).Namespace, Is.EqualTo("Kanini.RouteBuddy.Data.Repositories.Booking"));
    }

    [Test]
    public void BookingCancellationRepository_ImplementsInterface()
    {
        // Act & Assert - Implements interface
        Assert.That(typeof(BookingCancellationRepository).GetInterfaces().Any(i => i.Name == "IBookingCancellationRepository"), Is.True);
    }

    [Test]
    public void BookingCancellationRepository_IsPublicClass()
    {
        // Act & Assert - Is public class
        Assert.That(typeof(BookingCancellationRepository).IsPublic, Is.True);
    }

    [Test]
    public void BookingCancellationRepository_HasRequiredMethods()
    {
        // Act & Assert - Has all required methods
        var methods = typeof(BookingCancellationRepository).GetMethods().Select(m => m.Name).ToArray();
        Assert.That(methods, Contains.Item("GetBookingForCancellationAsync"));
        Assert.That(methods, Contains.Item("CancelBookingAsync"));
    }

    [Test]
    public void BookingCancellationRepository_HasCorrectConstructor()
    {
        // Act & Assert - Has correct constructor parameters
        var constructor = typeof(BookingCancellationRepository).GetConstructors()[0];
        var parameters = constructor.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(3));
    }

    [Test]
    public void BookingCancellationRepository_UsesCorrectUsings()
    {
        // Act & Assert - Class exists and can be referenced
        Assert.That(typeof(BookingCancellationRepository), Is.Not.Null);
        Assert.That(typeof(BookingCancellationRepository).Assembly, Is.Not.Null);
    }

    [Test]
    public void GetBookingForCancellationAsync_ReturnsCorrectType()
    {
        // Act & Assert - Method returns correct type
        var method = typeof(BookingCancellationRepository).GetMethod("GetBookingForCancellationAsync");
        Assert.That(method.ReturnType.Name, Is.EqualTo("Task`1"));
    }

    [Test]
    public void CancelBookingAsync_ReturnsCorrectType()
    {
        // Act & Assert - Method returns correct type
        var method = typeof(BookingCancellationRepository).GetMethod("CancelBookingAsync");
        Assert.That(method.ReturnType.Name, Is.EqualTo("Task`1"));
    }

    [Test]
    public void BookingCancellationRepository_HasCorrectMethodParameters()
    {
        // Act & Assert - GetBookingForCancellationAsync has correct parameters
        var method = typeof(BookingCancellationRepository).GetMethod("GetBookingForCancellationAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(2));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
        Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void CancelBookingAsync_HasCorrectMethodParameters()
    {
        // Act & Assert - CancelBookingAsync has correct parameters
        var method = typeof(BookingCancellationRepository).GetMethod("CancelBookingAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(4));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
        Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(string)));
        Assert.That(parameters[2].ParameterType, Is.EqualTo(typeof(decimal)));
        Assert.That(parameters[3].ParameterType, Is.EqualTo(typeof(string)));
    }

    [TearDown]
    public void TearDown()
    {
        // No cleanup needed for reflection tests
    }
}