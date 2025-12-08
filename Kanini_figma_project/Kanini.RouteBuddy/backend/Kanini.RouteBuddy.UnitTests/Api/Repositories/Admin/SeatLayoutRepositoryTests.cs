using Kanini.RouteBuddy.Data.Repositories.Admin;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kanini.RouteBuddy.UnitTests.Api.Repositories.Admin;

[TestFixture]
public class SeatLayoutRepositoryTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<SeatLayoutRepository>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<SeatLayoutRepository>>();
    }

    [Test]
    public void GetAllTemplatesAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(SeatLayoutRepository).GetMethod("GetAllTemplatesAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetTemplateByIdAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(SeatLayoutRepository).GetMethod("GetTemplateByIdAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void GetTemplateWithDetailsAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(SeatLayoutRepository).GetMethod("GetTemplateWithDetailsAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void CreateTemplateAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(SeatLayoutRepository).GetMethod("CreateTemplateAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void UpdateTemplateAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(SeatLayoutRepository).GetMethod("UpdateTemplateAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void DeactivateTemplateAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(SeatLayoutRepository).GetMethod("DeactivateTemplateAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void CheckTemplateNameExistsAsync_MethodExists()
    {
        // Act & Assert - Method exists
        Assert.DoesNotThrow(() => 
        {
            var method = typeof(SeatLayoutRepository).GetMethod("CheckTemplateNameExistsAsync");
            Assert.That(method, Is.Not.Null);
        });
    }

    [Test]
    public void SeatLayoutRepository_HasCorrectNamespace()
    {
        // Act & Assert - Correct namespace
        Assert.That(typeof(SeatLayoutRepository).Namespace, Is.EqualTo("Kanini.RouteBuddy.Data.Repositories.Admin"));
    }

    [Test]
    public void SeatLayoutRepository_ImplementsInterface()
    {
        // Act & Assert - Implements interface
        Assert.That(typeof(SeatLayoutRepository).GetInterfaces().Any(i => i.Name == "ISeatLayoutRepository"), Is.True);
    }

    [Test]
    public void SeatLayoutRepository_IsPublicClass()
    {
        // Act & Assert - Is public class
        Assert.That(typeof(SeatLayoutRepository).IsPublic, Is.True);
    }

    [Test]
    public void SeatLayoutRepository_HasRequiredMethods()
    {
        // Act & Assert - Has all required methods
        var methods = typeof(SeatLayoutRepository).GetMethods().Select(m => m.Name).ToArray();
        Assert.That(methods, Contains.Item("GetAllTemplatesAsync"));
        Assert.That(methods, Contains.Item("GetTemplateByIdAsync"));
        Assert.That(methods, Contains.Item("GetTemplateWithDetailsAsync"));
        Assert.That(methods, Contains.Item("CreateTemplateAsync"));
        Assert.That(methods, Contains.Item("UpdateTemplateAsync"));
        Assert.That(methods, Contains.Item("DeactivateTemplateAsync"));
        Assert.That(methods, Contains.Item("CheckTemplateNameExistsAsync"));
    }

    [Test]
    public void SeatLayoutRepository_HasCorrectConstructor()
    {
        // Act & Assert - Has correct constructor parameters
        var constructor = typeof(SeatLayoutRepository).GetConstructors()[0];
        var parameters = constructor.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(3));
    }

    [TearDown]
    public void TearDown()
    {
        // No disposal needed for mocked context
    }
}