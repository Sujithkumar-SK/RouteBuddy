using System.Reflection;
using NUnit.Framework;
using Kanini.RouteBuddy.Data.Repositories.Email;

namespace Kanini.RouteBuddy.UnitTests.Api.Repositories.Email;

[TestFixture]
public class EmailRepositoryTests
{
    private Type _repositoryType;

    [SetUp]
    public void Setup()
    {
        _repositoryType = typeof(EmailRepository);
    }

    [Test]
    public void EmailRepository_ShouldHaveCorrectNamespace()
    {
        Assert.That(_repositoryType.Namespace, Is.EqualTo("Kanini.RouteBuddy.Data.Repositories.Email"));
    }

    [Test]
    public void EmailRepository_ShouldImplementIEmailRepository()
    {
        var interfaces = _repositoryType.GetInterfaces();
        Assert.That(interfaces.Any(i => i.Name == "IEmailRepository"), Is.True);
    }

    [Test]
    public void EmailRepository_ShouldHaveCorrectConstructor()
    {
        var constructor = _repositoryType.GetConstructors().FirstOrDefault();
        Assert.That(constructor, Is.Not.Null);
        
        var parameters = constructor.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(2));
        Assert.That(parameters[0].ParameterType.Name, Is.EqualTo("IConfiguration"));
        Assert.That(parameters[1].ParameterType.Name, Is.EqualTo("ILogger`1"));
    }

    [Test]
    public void GetBookingDetailsForEmailAsync_ShouldExist()
    {
        var method = _repositoryType.GetMethod("GetBookingDetailsForEmailAsync");
        Assert.That(method, Is.Not.Null);
        Assert.That(method.IsPublic, Is.True);
        Assert.That(method.ReturnType.Name, Is.EqualTo("Task`1"));
    }

    [Test]
    public void GetBookingDetailsForEmailAsync_ShouldHaveCorrectParameters()
    {
        var method = _repositoryType.GetMethod("GetBookingDetailsForEmailAsync");
        var parameters = method.GetParameters();
        
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].Name, Is.EqualTo("bookingId"));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void GetConnectingBookingDetailsForEmailAsync_ShouldExist()
    {
        var method = _repositoryType.GetMethod("GetConnectingBookingDetailsForEmailAsync");
        Assert.That(method, Is.Not.Null);
        Assert.That(method.IsPublic, Is.True);
        Assert.That(method.ReturnType.Name, Is.EqualTo("Task`1"));
    }

    [Test]
    public void GetConnectingBookingDetailsForEmailAsync_ShouldHaveCorrectParameters()
    {
        var method = _repositoryType.GetMethod("GetConnectingBookingDetailsForEmailAsync");
        var parameters = method.GetParameters();
        
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].Name, Is.EqualTo("bookingId"));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void EmailRepository_ShouldHavePrivateFields()
    {
        var fields = _repositoryType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var connectionStringField = fields.FirstOrDefault(f => f.Name == "_connectionString");
        var loggerField = fields.FirstOrDefault(f => f.Name == "_logger");
        
        Assert.That(connectionStringField, Is.Not.Null);
        Assert.That(connectionStringField.FieldType, Is.EqualTo(typeof(string)));
        Assert.That(loggerField, Is.Not.Null);
    }

    [Test]
    public void EmailRepository_ShouldHaveCorrectMethodCount()
    {
        var publicMethods = _repositoryType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var repositoryMethods = publicMethods.Where(m => !m.IsSpecialName).ToArray();
        
        Assert.That(repositoryMethods.Length, Is.EqualTo(2));
    }

    [Test]
    public void EmailRepository_AllMethods_ShouldBeAsync()
    {
        var methods = _repositoryType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName);
        
        foreach (var method in methods)
        {
            Assert.That(method.Name.EndsWith("Async"), Is.True, $"Method {method.Name} should be async");
            Assert.That(method.ReturnType.Name.StartsWith("Task"), Is.True, $"Method {method.Name} should return Task");
        }
    }
}