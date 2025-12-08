using Kanini.RouteBuddy.Api.Controllers;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Services.Admin;
using Kanini.RouteBuddy.Common.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kanini.RouteBuddy.UnitTests.Api.Controllers.Admin;

[TestFixture]
public class AdminSeatLayoutControllerTests
{
    private Mock<IAdminSeatLayoutService> _mockService;
    private Mock<ILogger<AdminSeatLayoutController>> _mockLogger;
    private AdminSeatLayoutController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IAdminSeatLayoutService>();
        _mockLogger = new Mock<ILogger<AdminSeatLayoutController>>();
        _controller = new AdminSeatLayoutController(_mockService.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetTemplates_Success_ReturnsOk()
    {
        // Arrange
        var templates = new List<SeatLayoutTemplateListDto>();
        _mockService.Setup(x => x.GetAllTemplatesAsync())
            .ReturnsAsync(Result.Success(templates));

        // Act
        var result = await _controller.GetTemplates();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(templates));
    }

    [Test]
    public async Task GetTemplates_Failure_ReturnsBadRequest()
    {
        // Arrange
        _mockService.Setup(x => x.GetAllTemplatesAsync())
            .ReturnsAsync(Result.Failure<List<SeatLayoutTemplateListDto>>(Error.Failure("Test.Error", "Test error")));

        // Act
        var result = await _controller.GetTemplates();

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task GetTemplateById_Success_ReturnsOk()
    {
        // Arrange
        var template = new SeatLayoutTemplateResponseDto();
        _mockService.Setup(x => x.GetTemplateByIdAsync(1))
            .ReturnsAsync(Result.Success(template));

        // Act
        var result = await _controller.GetTemplateById(1);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(template));
    }

    [Test]
    public async Task GetTemplateById_Failure_ReturnsBadRequest()
    {
        // Arrange
        _mockService.Setup(x => x.GetTemplateByIdAsync(1))
            .ReturnsAsync(Result.Failure<SeatLayoutTemplateResponseDto>(Error.NotFound("Template.NotFound", "Template not found")));

        // Act
        var result = await _controller.GetTemplateById(1);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateTemplate_Success_ReturnsCreated()
    {
        // Arrange
        var request = new CreateSeatLayoutTemplateRequestDto { TemplateName = "Test Template" };
        var response = new SeatLayoutTemplateResponseDto { SeatLayoutTemplateId = 1 };
        _mockService.Setup(x => x.CreateTemplateAsync(request))
            .ReturnsAsync(Result.Success(response));

        // Act
        var result = await _controller.CreateTemplate(request);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result as CreatedAtActionResult;
        Assert.That(createdResult!.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task CreateTemplate_Failure_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateSeatLayoutTemplateRequestDto { TemplateName = "Test Template" };
        _mockService.Setup(x => x.CreateTemplateAsync(request))
            .ReturnsAsync(Result.Failure<SeatLayoutTemplateResponseDto>(Error.Failure("Template.CreateFailed", "Creation failed")));

        // Act
        var result = await _controller.CreateTemplate(request);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateTemplate_Success_ReturnsOk()
    {
        // Arrange
        var request = new UpdateSeatLayoutTemplateRequestDto();
        var response = new SeatLayoutTemplateResponseDto();
        _mockService.Setup(x => x.UpdateTemplateAsync(1, request))
            .ReturnsAsync(Result.Success(response));

        // Act
        var result = await _controller.UpdateTemplate(1, request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task UpdateTemplate_Failure_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateSeatLayoutTemplateRequestDto();
        _mockService.Setup(x => x.UpdateTemplateAsync(1, request))
            .ReturnsAsync(Result.Failure<SeatLayoutTemplateResponseDto>(Error.Failure("Template.UpdateFailed", "Update failed")));

        // Act
        var result = await _controller.UpdateTemplate(1, request);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task DeactivateTemplate_Success_ReturnsOk()
    {
        // Arrange
        _mockService.Setup(x => x.DeactivateTemplateAsync(1))
            .ReturnsAsync(Result.Success("Template deactivated successfully"));

        // Act
        var result = await _controller.DeactivateTemplate(1);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task DeactivateTemplate_Failure_ReturnsBadRequest()
    {
        // Arrange
        _mockService.Setup(x => x.DeactivateTemplateAsync(1))
            .ReturnsAsync(Result.Failure<string>(Error.Failure("Template.DeactivateFailed", "Deactivation failed")));

        // Act
        var result = await _controller.DeactivateTemplate(1);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public void Constructor_ValidParameters_InitializesController()
    {
        // Arrange & Act
        var controller = new AdminSeatLayoutController(_mockService.Object, _mockLogger.Object);

        // Assert
        Assert.That(controller, Is.Not.Null);
    }
}