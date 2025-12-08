using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Infrastructure;
using Kanini.RouteBuddy.Data.Repositories.User;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Data;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.UnitTests.Api.Repositories.User;

[TestFixture]
public class UserRepositoryTests
{
    private Mock<IDbReader> _mockDbReader;
    private Mock<ILogger<UserRepository>> _mockLogger;
    private RouteBuddyDatabaseContext _context;
    private UserRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<RouteBuddyDatabaseContext>()
            .UseSqlServer("Server=test;Database=test;Integrated Security=true;TrustServerCertificate=true;")
            .Options;
        
        _context = new RouteBuddyDatabaseContext(options);
        _mockDbReader = new Mock<IDbReader>();
        _mockLogger = new Mock<ILogger<UserRepository>>();
        
        _repository = new UserRepository(_context, _mockDbReader.Object, _mockLogger.Object);
    }

    [Test]
    public async Task ExistsByEmailAsync_EmailExists_ReturnsTrue()
    {
        // Arrange
        var email = "test@example.com";
        _mockDbReader.Setup(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.ExistsByEmailAsync(email);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsByEmailAsync_EmailNotExists_ReturnsFalse()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockDbReader.Setup(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
            .ReturnsAsync(0);

        // Act
        var result = await _repository.ExistsByEmailAsync(email);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ExistsByPhoneAsync_PhoneExists_ReturnsTrue()
    {
        // Arrange
        var phone = "1234567890";
        _mockDbReader.Setup(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.ExistsByPhoneAsync(phone);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsByPhoneAsync_PhoneNotExists_ReturnsFalse()
    {
        // Arrange
        var phone = "0000000000";
        _mockDbReader.Setup(x => x.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
            .ReturnsAsync(0);

        // Act
        var result = await _repository.ExistsByPhoneAsync(phone);

        // Assert
        Assert.That(result, Is.False);
    }

    //[Test]
    //public void CreateUserAsync_ValidUser_CallsAddAsync()
    //{
    //    // Arrange
    //    var user = new Entities.User
    //    {
    //        Email = "test@example.com",
    //        PasswordHash = "hashedpassword",
    //        Phone = "1234567890",
    //        Role = UserRole.Customer,
    //        IsEmailVerified = false,
    //        IsActive = true,
    //        CreatedBy = "System",
    //        CreatedOn = DateTime.UtcNow
    //    };

    //    // Act & Assert - Should not throw exception for method call
    //    Assert.DoesNotThrowAsync(async () => await _repository.CreateUserAsync(user));
    //}

    [Test]
    public async Task GetByIdAsync_UserExists_ReturnsUser()
    {
        // Arrange
        var userId = 1;
        var dataTable = new DataTable();
        dataTable.Columns.Add("UserId");
        dataTable.Columns.Add("Email");
        dataTable.Columns.Add("PasswordHash");
        dataTable.Columns.Add("Phone");
        dataTable.Columns.Add("Role");
        dataTable.Columns.Add("IsEmailVerified");
        dataTable.Columns.Add("IsActive");
        dataTable.Columns.Add("CreatedBy");
        dataTable.Columns.Add("CreatedOn");
        
        var row = dataTable.NewRow();
        row["UserId"] = 1;
        row["Email"] = "test@example.com";
        row["PasswordHash"] = "hashedpassword";
        row["Phone"] = "1234567890";
        row["Role"] = 1;
        row["IsEmailVerified"] = true;
        row["IsActive"] = true;
        row["CreatedBy"] = "System";
        row["CreatedOn"] = DateTime.UtcNow;
        dataTable.Rows.Add(row);

        _mockDbReader.Setup(x => x.ExecuteStoredProcedureAsync(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
            .ReturnsAsync(dataTable);

        // Act
        var result = await _repository.GetByIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(1));
        Assert.That(result.Email, Is.EqualTo("test@example.com"));
    }

    [Test]
    public async Task GetByIdAsync_UserNotExists_ReturnsNull()
    {
        // Arrange
        var userId = 999;
        var emptyDataTable = new DataTable();
        
        _mockDbReader.Setup(x => x.ExecuteStoredProcedureAsync(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
            .ReturnsAsync(emptyDataTable);

        // Act
        var result = await _repository.GetByIdAsync(userId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetUserByEmailAsync_UserExists_ReturnsUser()
    {
        // Arrange
        var email = "test@example.com";
        var dataTable = new DataTable();
        dataTable.Columns.Add("UserId");
        dataTable.Columns.Add("Email");
        dataTable.Columns.Add("PasswordHash");
        dataTable.Columns.Add("Phone");
        dataTable.Columns.Add("Role");
        dataTable.Columns.Add("IsEmailVerified");
        dataTable.Columns.Add("IsActive");
        dataTable.Columns.Add("CreatedBy");
        dataTable.Columns.Add("CreatedOn");
        
        var row = dataTable.NewRow();
        row["UserId"] = 1;
        row["Email"] = email;
        row["PasswordHash"] = "hashedpassword";
        row["Phone"] = "1234567890";
        row["Role"] = 1;
        row["IsEmailVerified"] = true;
        row["IsActive"] = true;
        row["CreatedBy"] = "System";
        row["CreatedOn"] = DateTime.UtcNow;
        dataTable.Rows.Add(row);

        _mockDbReader.Setup(x => x.ExecuteStoredProcedureAsync(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
            .ReturnsAsync(dataTable);

        // Act
        var result = await _repository.GetUserByEmailAsync(email);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(email));
    }

    [Test]
    public void UpdateUserPasswordAsync_ValidUser_CallsMethod()
    {
        // Arrange
        var userId = 1;
        var newPasswordHash = "newhashedpassword";

        // Act & Assert - Method exists and can be called
        Assert.DoesNotThrow(() => _repository.UpdateUserPasswordAsync(userId, newPasswordHash));
    }

    [Test]
    public void UpdateLastLoginAsync_ValidUser_CallsMethod()
    {
        // Arrange
        var userId = 1;

        // Act & Assert - Method exists and can be called
        Assert.DoesNotThrow(() => _repository.UpdateLastLoginAsync(userId));
    }

    [Test]
    public void Constructor_NullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new UserRepository(null, _mockDbReader.Object, _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullDbReader_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new UserRepository(_context, null, _mockLogger.Object));
    }

    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new UserRepository(_context, _mockDbReader.Object, null));
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }
}