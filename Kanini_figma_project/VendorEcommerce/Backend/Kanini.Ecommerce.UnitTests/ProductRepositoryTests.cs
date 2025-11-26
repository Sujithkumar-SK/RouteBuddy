using FluentAssertions;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Products;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kanini.Ecommerce.UnitTests;

public class ProductRepositoryTests
{
    private readonly Mock<IProductRepository> _mockRepository;

    public ProductRepositoryTests()
    {
        _mockRepository = new Mock<IProductRepository>();
    }

    [Fact]
    public async Task CreateProductAsync_ValidProduct_ReturnsSuccess()
    {
        // Arrange
        var product = new Product { Name = "Test Product", SKU = "TEST001", Price = 100m };
        var expectedResult = Result.Success(product);
        _mockRepository.Setup(x => x.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.CreateProductAsync(product);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(x => x.CreateProductAsync(product), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var product = new Product { Name = "Test", SKU = "TEST001", Price = 100m };
        var expectedResult = Result.Failure<Product>(Error.Database("DB_ERROR", "Database error"));
        _mockRepository.Setup(x => x.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.CreateProductAsync(product);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Database);
    }

    [Fact]
    public async Task UpdateProductAsync_ExistingProduct_ReturnsSuccess()
    {
        // Arrange
        var updatedProduct = new Product { Name = "Updated", Price = 75m };
        var expectedResult = Result.Success();
        _mockRepository.Setup(x => x.UpdateProductAsync(1, It.IsAny<Product>(), "UpdatedBy")).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.UpdateProductAsync(1, updatedProduct, "UpdatedBy");

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(x => x.UpdateProductAsync(1, updatedProduct, "UpdatedBy"), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_NonExistentProduct_ReturnsNotFound()
    {
        // Arrange
        var updatedProduct = new Product { Name = "Updated", Price = 75m };
        var expectedResult = Result.Failure(Error.NotFound("NOT_FOUND", "Product not found"));
        _mockRepository.Setup(x => x.UpdateProductAsync(999, It.IsAny<Product>(), "UpdatedBy")).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.UpdateProductAsync(999, updatedProduct, "UpdatedBy");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteProductAsync_ExistingProduct_ReturnsSuccess()
    {
        // Arrange
        var expectedResult = Result.Success();
        _mockRepository.Setup(x => x.DeleteProductAsync(1, "DeletedBy")).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.DeleteProductAsync(1, "DeletedBy");

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(x => x.DeleteProductAsync(1, "DeletedBy"), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_NonExistentProduct_ReturnsNotFound()
    {
        // Arrange
        var expectedResult = Result.Failure(Error.NotFound("NOT_FOUND", "Product not found"));
        _mockRepository.Setup(x => x.DeleteProductAsync(999, "DeletedBy")).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.DeleteProductAsync(999, "DeletedBy");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateProductStatusAsync_ValidStatus_ReturnsSuccess()
    {
        // Arrange
        var expectedResult = Result.Success();
        _mockRepository.Setup(x => x.UpdateProductStatusAsync(1, "Active", "UpdatedBy")).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.UpdateProductStatusAsync(1, "Active", "UpdatedBy");

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(x => x.UpdateProductStatusAsync(1, "Active", "UpdatedBy"), Times.Once);
    }

    [Fact]
    public async Task UpdateProductStatusAsync_InvalidStatus_ReturnsValidationError()
    {
        // Arrange
        var expectedResult = Result.Failure(Error.Validation("INVALID_STATUS", "Invalid status"));
        _mockRepository.Setup(x => x.UpdateProductStatusAsync(1, "InvalidStatus", "UpdatedBy")).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.UpdateProductStatusAsync(1, "InvalidStatus", "UpdatedBy");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task SaveProductImagesAsync_ValidImages_ReturnsSuccess()
    {
        // Arrange
        var imagePaths = new List<string> { "/img1.jpg", "/img2.jpg" };
        var expectedResult = Result.Success();
        _mockRepository.Setup(x => x.SaveProductImagesAsync(1, imagePaths)).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.SaveProductImagesAsync(1, imagePaths);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(x => x.SaveProductImagesAsync(1, imagePaths), Times.Once);
    }

    [Fact]
    public async Task GetVendorProductCountAsync_ExistingVendor_ReturnsCount()
    {
        // Arrange
        var expectedResult = Result.Success(2);
        _mockRepository.Setup(x => x.GetVendorProductCountAsync(1)).ReturnsAsync(expectedResult);

        // Act
        var result = await _mockRepository.Object.GetVendorProductCountAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2);
        _mockRepository.Verify(x => x.GetVendorProductCountAsync(1), Times.Once);
    }
}