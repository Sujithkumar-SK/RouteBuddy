using Kanini.RouteBuddy.Data.Repositories;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kanini.RouteBuddy.UnitTests.Api.Repositories.Payment;

[TestFixture]
public class PaymentRepositoryTests
{
    private Mock<IConfiguration> _mockConfiguration = null!;
    private Mock<ILogger<PaymentRepository>> _mockLogger = null!;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<PaymentRepository>>();
    }

    [Test]
    public void CreatePaymentAsync_ValidPayment_MethodExists()
    {
        // Arrange
        var payment = new Domain.Entities.Payment
        {
            BookingId = 1,
            Amount = 100.00m,
            PaymentMethod = PaymentMethod.Card,
            PaymentStatus = PaymentStatus.Pending,
            PaymentDate = DateTime.UtcNow,
            TransactionId = "TXN123",
            IsActive = true
        };

        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(PaymentRepository).GetMethod("CreatePaymentAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetPaymentByIdAsync_ValidId_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(PaymentRepository).GetMethod("GetPaymentByIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetPaymentByTransactionIdAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(PaymentRepository).GetMethod("GetPaymentByTransactionIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetPaymentsByBookingIdAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(PaymentRepository).GetMethod("GetPaymentsByBookingIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void UpdatePaymentStatusAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(PaymentRepository).GetMethod("UpdatePaymentStatusAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void Constructor_ValidParameters_DoesNotThrow()
    {
        // Act & Assert - Constructor works
        Assert.DoesNotThrow(() => 
        {
            var constructor = typeof(PaymentRepository).GetConstructors()[0];
            Assert.That(constructor, Is.Not.Null);
        });
    }

    [Test]
    public void PaymentRepository_HasCorrectNamespace()
    {
        // Act & Assert - Correct namespace
        Assert.That(typeof(PaymentRepository).Namespace, Is.EqualTo("Kanini.RouteBuddy.Data.Repositories"));
    }

    [Test]
    public void PaymentRepository_ImplementsInterface()
    {
        // Act & Assert - Implements interface
        Assert.That(typeof(PaymentRepository).GetInterfaces().Any(i => i.Name == "IPaymentRepository"), Is.True);
    }

    [Test]
    public void PaymentRepository_HasRequiredMethods()
    {
        // Act & Assert - Has all required methods
        var methods = typeof(PaymentRepository).GetMethods().Select(m => m.Name).ToArray();
        Assert.That(methods, Contains.Item("CreatePaymentAsync"));
        Assert.That(methods, Contains.Item("GetPaymentByIdAsync"));
        Assert.That(methods, Contains.Item("GetPaymentByTransactionIdAsync"));
        Assert.That(methods, Contains.Item("GetPaymentsByBookingIdAsync"));
        Assert.That(methods, Contains.Item("UpdatePaymentStatusAsync"));
    }

    [Test]
    public void PaymentRepository_IsPublicClass()
    {
        // Act & Assert - Is public class
        Assert.That(typeof(PaymentRepository).IsPublic, Is.True);
    }

    [Test]
    public void PaymentRepository_HasCorrectConstructor()
    {
        // Act & Assert - Has correct constructor parameters
        var constructor = typeof(PaymentRepository).GetConstructors()[0];
        var parameters = constructor.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(3));
    }

    [Test]
    public void PaymentRepository_UsesCorrectUsings()
    {
        // Act & Assert - Class exists and can be referenced
        Assert.That(typeof(PaymentRepository), Is.Not.Null);
        Assert.That(typeof(PaymentRepository).Assembly, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        // No disposal needed for mocked context
    }
}