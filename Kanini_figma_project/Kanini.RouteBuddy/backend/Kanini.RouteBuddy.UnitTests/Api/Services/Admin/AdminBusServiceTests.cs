using NUnit.Framework;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Application.Services.Admin;
using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Domain.Enums;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using BusEntity = Kanini.RouteBuddy.Domain.Entities.Bus;

namespace Kanini.RouteBuddy.UnitTests.Api.Services.Admin;

[TestFixture]
public class AdminBusServiceTests
{
    private Mock<IBusRepository> _mockBusRepository;
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<AdminBusService>> _mockLogger;
    private AdminBusService _adminBusService;

    [SetUp]
    public void Setup()
    {
        _mockBusRepository = new Mock<IBusRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<AdminBusService>>();
        
        _adminBusService = new AdminBusService(
            _mockBusRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }

    [Test]
    public async Task GetAllBusesAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var buses = new List<BusEntity> { new BusEntity() };
        var busesDto = new List<BusResponseDto> { new BusResponseDto() };

        _mockBusRepository.Setup(x => x.GetAllBusesForAdminAsync()).ReturnsAsync(buses);
        _mockMapper.Setup(x => x.Map<List<BusResponseDto>>(buses)).Returns(busesDto);

        // Act
        var result = await _adminBusService.GetAllBusesAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllBusesAsync_WithException_ReturnsFailure()
    {
        // Arrange
        _mockBusRepository.Setup(x => x.GetAllBusesForAdminAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _adminBusService.GetAllBusesAsync();

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Bus.DatabaseError"));
    }

    [Test]
    public async Task GetPendingBusesAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var buses = new List<BusEntity> { new BusEntity { Status = BusStatus.PendingApproval } };
        var busesDto = new List<BusResponseDto> { new BusResponseDto() };

        _mockBusRepository.Setup(x => x.GetBusesByStatusAsync(BusStatus.PendingApproval)).ReturnsAsync(buses);
        _mockMapper.Setup(x => x.Map<List<BusResponseDto>>(buses)).Returns(busesDto);

        // Act
        var result = await _adminBusService.GetPendingBusesAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task FilterBusesAsync_WithValidParameters_ReturnsSuccess()
    {
        // Arrange
        var searchName = "Test";
        var status = 1;
        var isActive = true;
        var buses = new List<BusEntity> { new BusEntity() };
        var busesDto = new List<BusResponseDto> { new BusResponseDto() };

        _mockBusRepository.Setup(x => x.FilterBusesForAdminAsync(searchName, status, isActive)).ReturnsAsync(buses);
        _mockMapper.Setup(x => x.Map<List<BusResponseDto>>(buses)).Returns(busesDto);

        // Act
        var result = await _adminBusService.FilterBusesAsync(searchName, status, isActive);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetBusDetailsAsync_WithValidBusId_ReturnsSuccess()
    {
        // Arrange
        var busId = 1;
        var bus = new BusEntity();
        var busDto = new BusResponseDto();

        _mockBusRepository.Setup(x => x.GetBusDetailsForAdminAsync(busId)).ReturnsAsync(bus);
        _mockMapper.Setup(x => x.Map<BusResponseDto>(bus)).Returns(busDto);

        // Act
        var result = await _adminBusService.GetBusDetailsAsync(busId);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
    }

    [Test]
    public async Task GetBusDetailsAsync_WithNonExistentBusId_ReturnsFailure()
    {
        // Arrange
        var busId = 999;
        _mockBusRepository.Setup(x => x.GetBusDetailsForAdminAsync(busId)).ReturnsAsync((BusEntity?)null);

        // Act
        var result = await _adminBusService.GetBusDetailsAsync(busId);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Bus.NotFound"));
    }

    [Test]
    public async Task ApproveBusAsync_WithValidBusId_ReturnsSuccess()
    {
        // Arrange
        var busId = 1;
        var bus = new BusEntity { Status = BusStatus.PendingApproval };

        _mockBusRepository.Setup(x => x.GetByIdAsync(busId)).ReturnsAsync(Result.Success(bus));
        _mockBusRepository.Setup(x => x.UpdateAsync(It.IsAny<BusEntity>())).ReturnsAsync(Result.Success(bus));

        // Act
        var result = await _adminBusService.ApproveBusAsync(busId);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(bus.Status, Is.EqualTo(BusStatus.Active));
    }

    [Test]
    public async Task ApproveBusAsync_WithNonExistentBusId_ReturnsFailure()
    {
        // Arrange
        var busId = 999;
        var error = Error.NotFound("Bus.NotFound", "Bus not found");
        _mockBusRepository.Setup(x => x.GetByIdAsync(busId)).ReturnsAsync(Result.Failure<BusEntity>(error));

        // Act
        var result = await _adminBusService.ApproveBusAsync(busId);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Bus.NotFound"));
    }

    [Test]
    public async Task RejectBusAsync_WithValidBusId_ReturnsSuccess()
    {
        // Arrange
        var busId = 1;
        var rejectionReason = "Invalid documents";
        var bus = new BusEntity { Status = BusStatus.PendingApproval };

        _mockBusRepository.Setup(x => x.GetByIdAsync(busId)).ReturnsAsync(Result.Success(bus));
        _mockBusRepository.Setup(x => x.UpdateAsync(It.IsAny<BusEntity>())).ReturnsAsync(Result.Success(bus));

        // Act
        var result = await _adminBusService.RejectBusAsync(busId, rejectionReason);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(bus.Status, Is.EqualTo(BusStatus.Rejected));
    }

    [Test]
    public async Task DeactivateBusAsync_WithValidBusId_ReturnsSuccess()
    {
        // Arrange
        var busId = 1;
        var reason = "Maintenance required";
        var bus = new BusEntity { IsActive = true };

        _mockBusRepository.Setup(x => x.GetByIdAsync(busId)).ReturnsAsync(Result.Success(bus));
        _mockBusRepository.Setup(x => x.UpdateAsync(It.IsAny<BusEntity>())).ReturnsAsync(Result.Success(bus));

        // Act
        var result = await _adminBusService.DeactivateBusAsync(busId, reason);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(bus.IsActive, Is.False);
    }

    [Test]
    public async Task ReactivateBusAsync_WithValidBusId_ReturnsSuccess()
    {
        // Arrange
        var busId = 1;
        var reason = "Maintenance completed";
        var bus = new BusEntity { IsActive = false };

        _mockBusRepository.Setup(x => x.GetByIdAsync(busId)).ReturnsAsync(Result.Success(bus));
        _mockBusRepository.Setup(x => x.UpdateAsync(It.IsAny<BusEntity>())).ReturnsAsync(Result.Success(bus));

        // Act
        var result = await _adminBusService.ReactivateBusAsync(busId, reason);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(bus.IsActive, Is.True);
    }

    [Test]
    public async Task ApproveBusAsync_WithUpdateFailure_ReturnsFailure()
    {
        // Arrange
        var busId = 1;
        var bus = new BusEntity { Status = BusStatus.PendingApproval };
        var updateError = Error.Failure("Update.Failed", "Update failed");

        _mockBusRepository.Setup(x => x.GetByIdAsync(busId)).ReturnsAsync(Result.Success(bus));
        _mockBusRepository.Setup(x => x.UpdateAsync(It.IsAny<BusEntity>())).ReturnsAsync(Result.Failure<BusEntity>(updateError));

        // Act
        var result = await _adminBusService.ApproveBusAsync(busId);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Update.Failed"));
    }

    [Test]
    public async Task GetPendingBusesAsync_WithException_ReturnsFailure()
    {
        // Arrange
        _mockBusRepository.Setup(x => x.GetBusesByStatusAsync(BusStatus.PendingApproval))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _adminBusService.GetPendingBusesAsync();

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Bus.DatabaseError"));
    }

    [Test]
    public async Task FilterBusesAsync_WithException_ReturnsFailure()
    {
        // Arrange
        _mockBusRepository.Setup(x => x.FilterBusesForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<bool?>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _adminBusService.FilterBusesAsync("test", 1, true);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Bus.DatabaseError"));
    }
}