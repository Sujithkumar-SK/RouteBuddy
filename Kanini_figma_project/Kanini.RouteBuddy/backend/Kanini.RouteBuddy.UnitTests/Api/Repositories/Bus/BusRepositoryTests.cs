using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BusEntity = Kanini.RouteBuddy.Domain.Entities.Bus;

namespace Kanini.RouteBuddy.UnitTests.Api.Repositories.Bus;

[TestFixture]
public class BusRepositoryTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<BusRepository>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<BusRepository>>();
    }

    [Test]
    public void CreateAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("CreateAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetByIdAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("GetByIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetByVendorIdAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("GetByVendorIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetCountByVendorIdAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("GetCountByVendorIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void UpdateAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("UpdateAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void DeleteAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("DeleteAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void ExistsByRegistrationNoAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("ExistsByRegistrationNoAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void ExistsByIdAndVendorAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("ExistsByIdAndVendorAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetAwaitingConfirmationByVendorAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("GetAwaitingConfirmationByVendorAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void ExistsByNameAndVendorAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("ExistsByNameAndVendorAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void ExistsAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("ExistsAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetAllBusesForAdminAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("GetAllBusesForAdminAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetBusesByStatusAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("GetBusesByStatusAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void FilterBusesForAdminAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("FilterBusesForAdminAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetBusDetailsForAdminAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(BusRepository).GetMethod("GetBusDetailsForAdminAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void BusRepository_HasCorrectNamespace()
    {
        // Act & Assert - Correct namespace
        Assert.That(typeof(BusRepository).Namespace, Is.EqualTo("Kanini.RouteBuddy.Data.Repositories.Buses"));
    }

    [Test]
    public void BusRepository_ImplementsInterface()
    {
        // Act & Assert - Implements interface
        Assert.That(typeof(BusRepository).GetInterfaces().Any(i => i.Name == "IBusRepository"), Is.True);
    }

    [Test]
    public void BusRepository_IsPublicClass()
    {
        // Act & Assert - Is public class
        Assert.That(typeof(BusRepository).IsPublic, Is.True);
    }

    [Test]
    public void BusRepository_HasRequiredMethods()
    {
        // Act & Assert - Has all required methods
        var methods = typeof(BusRepository).GetMethods().Select(m => m.Name).ToArray();
        Assert.That(methods, Contains.Item("CreateAsync"));
        Assert.That(methods, Contains.Item("GetByIdAsync"));
        Assert.That(methods, Contains.Item("GetByVendorIdAsync"));
        Assert.That(methods, Contains.Item("UpdateAsync"));
        Assert.That(methods, Contains.Item("DeleteAsync"));
        Assert.That(methods, Contains.Item("ExistsAsync"));
    }

    [Test]
    public void BusRepository_HasCorrectConstructor()
    {
        // Act & Assert - Has correct constructor parameters
        var constructor = typeof(BusRepository).GetConstructors()[0];
        var parameters = constructor.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(3));
    }

    [Test]
    public void CreateAsync_HasCorrectReturnType()
    {
        // Act & Assert - Method returns correct type
        var method = typeof(BusRepository).GetMethod("CreateAsync");
        Assert.That(method.ReturnType.Name, Is.EqualTo("Task`1"));
    }

    [Test]
    public void GetByIdAsync_HasCorrectParameters()
    {
        // Act & Assert - Method has correct parameters
        var method = typeof(BusRepository).GetMethod("GetByIdAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void UpdateAsync_HasCorrectParameters()
    {
        // Act & Assert - Method has correct parameters
        var method = typeof(BusRepository).GetMethod("UpdateAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType.Name, Is.EqualTo("Bus"));
    }

    [Test]
    public void DeleteAsync_HasCorrectParameters()
    {
        // Act & Assert - Method has correct parameters
        var method = typeof(BusRepository).GetMethod("DeleteAsync");
        var parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
    }

    [TearDown]
    public void TearDown()
    {
        // No cleanup needed for reflection tests
    }
}