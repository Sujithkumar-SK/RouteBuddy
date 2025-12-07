using Kanini.RouteBuddy.Data.Infrastructure;
using Kanini.RouteBuddy.Data.Repositories.Customer;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using CustomerEntity = Kanini.RouteBuddy.Domain.Entities.Customer;

namespace Kanini.RouteBuddy.UnitTests.Api.Repositories.Customer;

[TestFixture]
public class CustomerRepositoryTests
{
    private Mock<IDbReader> _mockDbReader;
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<CustomerRepository>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _mockDbReader = new Mock<IDbReader>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<CustomerRepository>>();
    }

    [Test]
    public void AddCustomerAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("AddCustomerAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void CreateCustomerAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("CreateCustomerAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetCustomerByEmailAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("GetCustomerByEmailAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetAllCustomersAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("GetAllCustomersAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void FilterCustomersAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("FilterCustomersAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetCustomerByIdAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("GetCustomerByIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void SoftDeleteCustomerAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("SoftDeleteCustomerAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetCustomerProfileByUserIdAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("GetCustomerProfileByUserIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetCustomerProfileByIdAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("GetCustomerProfileByIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void UpdateCustomerProfilePictureAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("UpdateCustomerProfilePictureAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void UpdateCustomerProfileAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("UpdateCustomerProfileAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetCustomerBookingsAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("GetCustomerBookingsAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetCustomerBookingsWithDetailsAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(CustomerRepository).GetMethod("GetCustomerBookingsWithDetailsAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void CustomerRepository_HasCorrectNamespace()
    {
        // Act & Assert - Correct namespace
        Assert.That(typeof(CustomerRepository).Namespace, Is.EqualTo("Kanini.RouteBuddy.Data.Repositories.Customer"));
    }

    [Test]
    public void CustomerRepository_ImplementsInterface()
    {
        // Act & Assert - Implements interface
        Assert.That(typeof(CustomerRepository).GetInterfaces().Any(i => i.Name == "ICustomerRepository"), Is.True);
    }

    [Test]
    public void CustomerRepository_IsPublicClass()
    {
        // Act & Assert - Is public class
        Assert.That(typeof(CustomerRepository).IsPublic, Is.True);
    }

    [Test]
    public void CustomerRepository_HasRequiredMethods()
    {
        // Act & Assert - Has all required methods
        var methods = typeof(CustomerRepository).GetMethods().Select(m => m.Name).ToArray();
        Assert.That(methods, Contains.Item("AddCustomerAsync"));
        Assert.That(methods, Contains.Item("CreateCustomerAsync"));
        Assert.That(methods, Contains.Item("GetCustomerByEmailAsync"));
        Assert.That(methods, Contains.Item("GetAllCustomersAsync"));
        Assert.That(methods, Contains.Item("FilterCustomersAsync"));
        Assert.That(methods, Contains.Item("GetCustomerByIdAsync"));
        Assert.That(methods, Contains.Item("UpdateCustomerProfileAsync"));
        Assert.That(methods, Contains.Item("GetCustomerBookingsAsync"));
    }

    [Test]
    public void CustomerRepository_HasCorrectConstructor()
    {
        // Act & Assert - Has correct constructor parameters
        var constructor = typeof(CustomerRepository).GetConstructors()[0];
        var parameters = constructor.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(4));
    }

    [Test]
    public void GetCustomerByIdAsync_HasCorrectParameters()
    {
        // Act & Assert - Method has correct parameters
        var method = typeof(CustomerRepository).GetMethod("GetCustomerByIdAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void GetCustomerByEmailAsync_HasCorrectParameters()
    {
        // Act & Assert - Method has correct parameters
        var method = typeof(CustomerRepository).GetMethod("GetCustomerByEmailAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void UpdateCustomerProfileAsync_HasCorrectParameters()
    {
        // Act & Assert - Method has correct parameters
        var method = typeof(CustomerRepository).GetMethod("UpdateCustomerProfileAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(7));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
        Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void FilterCustomersAsync_HasCorrectParameters()
    {
        // Act & Assert - Method has correct parameters
        var method = typeof(CustomerRepository).GetMethod("FilterCustomersAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(4));
    }

    [Test]
    public void SoftDeleteCustomerAsync_HasCorrectReturnType()
    {
        // Act & Assert - Method returns correct type
        var method = typeof(CustomerRepository).GetMethod("SoftDeleteCustomerAsync");
        Assert.That(method.ReturnType.Name, Is.EqualTo("Task`1"));
    }

    [Test]
    public void UpdateCustomerProfilePictureAsync_HasCorrectParameters()
    {
        // Act & Assert - Method has correct parameters
        var method = typeof(CustomerRepository).GetMethod("UpdateCustomerProfilePictureAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(2));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
        Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(byte[])));
    }

    [TearDown]
    public void TearDown()
    {
        // No cleanup needed for reflection tests
    }
}