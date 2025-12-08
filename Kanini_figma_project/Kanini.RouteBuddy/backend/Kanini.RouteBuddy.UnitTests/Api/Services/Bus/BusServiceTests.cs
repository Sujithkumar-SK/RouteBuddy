using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Services.Buses;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Data.Repositories.Vendor;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kanini.RouteBuddy.UnitTests.Api.Services.Bus;

[TestFixture]
public class BusServiceTests
{
    private Mock<IBusRepository> _mockBusRepository;
    private Mock<IVendorRepository> _mockVendorRepository;
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<BusService>> _mockLogger;
    private BusService _service;

    [SetUp]
    public void Setup()
    {
        _mockBusRepository = new Mock<IBusRepository>();
        _mockVendorRepository = new Mock<IVendorRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<BusService>>();
        _service = new BusService(_mockBusRepository.Object, _mockVendorRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Test]
    public async Task CreateBusAsync_EmptyBusName_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateBusDto { BusName = "", TotalSeats = 50 };

        // Act
        var result = await _service.CreateBusAsync(dto, 1);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Bus.InvalidName"));
    }

    [Test]
    public async Task CreateBusAsync_InvalidSeatCount_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateBusDto { BusName = "Test Bus", TotalSeats = 5 };

        // Act
        var result = await _service.CreateBusAsync(dto, 1);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Bus.InvalidSeats"));
    }

    [Test]
    public async Task CreateBusAsync_InvalidRegistrationFormat_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateBusDto { BusName = "Test Bus", TotalSeats = 50, RegistrationNo = "INVALID" };

        // Act
        var result = await _service.CreateBusAsync(dto, 1);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Bus.InvalidRegistration"));
    }

    [Test]
    public async Task CreateBusAsync_RegistrationExists_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateBusDto { BusName = "Test Bus", TotalSeats = 50, RegistrationNo = "MH01AB1234" };
        _mockBusRepository.Setup(x => x.ExistsByRegistrationNoAsync("MH01AB1234"))
            .ReturnsAsync(Result.Success(true));

        // Act
        var result = await _service.CreateBusAsync(dto, 1);

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task GetBusByIdAsync_UnauthorizedVendor_ReturnsFailure()
    {
        // Arrange
        var bus = new Domain.Entities.Bus { BusId = 1, VendorId = 2 };
        _mockBusRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Result.Success(bus));

        // Act
        var result = await _service.GetBusByIdAsync(1, 1);

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task GetBusByIdAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var bus = new Domain.Entities.Bus { BusId = 1, VendorId = 1 };
        var vendor = new Vendor { VendorId = 1, AgencyName = "Test Agency" };
        var busDto = new BusResponseDto { BusId = 1 };

        _mockBusRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Result.Success(bus));
        _mockVendorRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(vendor);
        _mockMapper.Setup(x => x.Map<BusResponseDto>(bus)).Returns(busDto);

        // Act
        var result = await _service.GetBusByIdAsync(1, 1);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.VendorName, Is.EqualTo("Test Agency"));
    }

    [Test]
    public async Task GetBusesByVendorAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var buses = new List<Domain.Entities.Bus> { new Domain.Entities.Bus { BusId = 1 } };
        var busDtos = new List<BusResponseDto> { new BusResponseDto { BusId = 1 } };
        var vendor = new Vendor { AgencyName = "Test Agency" };

        _mockBusRepository.Setup(x => x.GetByVendorIdAsync(1, 1, 10, null, null, null))
            .ReturnsAsync(Result.Success(buses));
        _mockBusRepository.Setup(x => x.GetCountByVendorIdAsync(1, null, null, null))
            .ReturnsAsync(Result.Success(1));
        _mockVendorRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(vendor);
        _mockMapper.Setup(x => x.Map<List<BusResponseDto>>(buses)).Returns(busDtos);

        // Act
        var result = await _service.GetBusesByVendorAsync(1, 1, 10);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Data.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateBusAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var bus = new Domain.Entities.Bus { BusId = 1, VendorId = 1 };
        var dto = new UpdateBusDto { BusName = "Updated Bus" };
        var vendor = new Vendor { AgencyName = "Test Agency" };
        var busDto = new BusResponseDto { BusId = 1 };

        _mockBusRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Result.Success(bus));
        _mockVendorRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(vendor);
        _mockMapper.Setup(x => x.Map(dto, bus));
        _mockBusRepository.Setup(x => x.UpdateAsync(bus)).ReturnsAsync(Result.Success(bus));
        _mockMapper.Setup(x => x.Map<BusResponseDto>(bus)).Returns(busDto);

        // Act
        var result = await _service.UpdateBusAsync(1, dto);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task DeleteBusAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        _mockBusRepository.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result.Success(true));

        // Act
        var result = await _service.DeleteBusAsync(1);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.True);
    }

    [Test]
    public async Task ActivateBusAsync_InvalidStatus_ReturnsFailure()
    {
        // Arrange
        var bus = new Domain.Entities.Bus { BusId = 1, VendorId = 1, Status = BusStatus.Active };
        _mockBusRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Result.Success(bus));

        // Act
        var result = await _service.ActivateBusAsync(1, 1);

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task ActivateBusAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var bus = new Domain.Entities.Bus { BusId = 1, VendorId = 1, Status = BusStatus.PendingApproval };
        var vendor = new Vendor { AgencyName = "Test Agency" };
        var busDto = new BusResponseDto { BusId = 1 };

        _mockBusRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Result.Success(bus));
        _mockVendorRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(vendor);
        _mockBusRepository.Setup(x => x.UpdateAsync(bus)).ReturnsAsync(Result.Success(bus));
        _mockMapper.Setup(x => x.Map<BusResponseDto>(bus)).Returns(busDto);

        // Act
        var result = await _service.ActivateBusAsync(1, 1);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(bus.Status, Is.EqualTo(BusStatus.Active));
        Assert.That(bus.IsActive, Is.True);
    }

    [Test]
    public async Task DeactivateBusAsync_AlreadyInactive_ReturnsFailure()
    {
        // Arrange
        var bus = new Domain.Entities.Bus { BusId = 1, VendorId = 1, IsActive = false };
        _mockBusRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Result.Success(bus));

        // Act
        var result = await _service.DeactivateBusAsync(1, 1);

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task SetMaintenanceAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var bus = new Domain.Entities.Bus { BusId = 1, VendorId = 1 };
        var vendor = new Vendor { AgencyName = "Test Agency" };
        var busDto = new BusResponseDto { BusId = 1 };

        _mockBusRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Result.Success(bus));
        _mockVendorRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(vendor);
        _mockBusRepository.Setup(x => x.UpdateAsync(bus)).ReturnsAsync(Result.Success(bus));
        _mockMapper.Setup(x => x.Map<BusResponseDto>(bus)).Returns(busDto);

        // Act
        var result = await _service.SetMaintenanceAsync(1, 1);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(bus.Status, Is.EqualTo(BusStatus.Maintenance));
    }

    [Test]
    public async Task GetAwaitingConfirmationAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var buses = new List<Domain.Entities.Bus> { new Domain.Entities.Bus { BusId = 1 } };
        var busDtos = new List<BusResponseDto> { new BusResponseDto { BusId = 1 } };

        _mockBusRepository.Setup(x => x.GetAwaitingConfirmationByVendorAsync(1))
            .ReturnsAsync(Result.Success(buses));
        _mockMapper.Setup(x => x.Map<List<BusResponseDto>>(buses)).Returns(busDtos);

        // Act
        var result = await _service.GetAwaitingConfirmationAsync(1);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Count, Is.EqualTo(1));
    }
}