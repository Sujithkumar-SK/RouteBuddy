using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Data.Repositories.Vendor;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services.Vendor;

public class VendorSeatLayoutService : IVendorSeatLayoutService
{
    private readonly IVendorSeatLayoutRepository _seatLayoutRepository;
    private readonly IBusRepository _busRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<VendorSeatLayoutService> _logger;

    public VendorSeatLayoutService(
        IVendorSeatLayoutRepository seatLayoutRepository,
        IBusRepository busRepository,
        IMapper mapper,
        ILogger<VendorSeatLayoutService> logger)
    {
        _seatLayoutRepository = seatLayoutRepository;
        _busRepository = busRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<SeatLayoutTemplateListDto>>> GetTemplatesByBusTypeAsync(int busType)
    {
        try
        {
            _logger.LogInformation("Getting seat layout templates for bus type: {BusType}", busType);

            var result = await _seatLayoutRepository.GetTemplatesByBusTypeAsync(busType);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to get templates by bus type: {Error}", result.Error.Description);
                return Result.Failure<List<SeatLayoutTemplateListDto>>(result.Error);
            }

            var templateListDtos = _mapper.Map<List<SeatLayoutTemplateListDto>>(result.Value);

            _logger.LogInformation("Retrieved {Count} templates for bus type {BusType}", templateListDtos.Count, busType);
            return Result.Success(templateListDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting templates by bus type: {Message}", ex.Message);
            return Result.Failure<List<SeatLayoutTemplateListDto>>(
                Error.Failure("VendorSeatLayoutService.GetByTypeFailed", "An unexpected error occurred"));
        }
    }

    public async Task<Result<SeatLayoutTemplateResponseDto>> GetTemplateByIdAsync(int templateId)
    {
        try
        {
            if (templateId <= 0)
            {
                _logger.LogWarning("Invalid template ID: {TemplateId}", templateId);
                return Result.Failure<SeatLayoutTemplateResponseDto>(
                    Error.Failure("VendorSeatLayoutService.InvalidId", "Template ID must be greater than 0"));
            }

            _logger.LogInformation("Getting seat layout template by ID: {TemplateId}", templateId);

            var result = await _seatLayoutRepository.GetTemplateByIdAsync(templateId);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to get template by ID: {Error}", result.Error.Description);
                return Result.Failure<SeatLayoutTemplateResponseDto>(result.Error);
            }

            var templateResponseDto = _mapper.Map<SeatLayoutTemplateResponseDto>(result.Value);

            _logger.LogInformation("Retrieved template: {TemplateId}", templateId);
            return Result.Success(templateResponseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting template by ID: {Message}", ex.Message);
            return Result.Failure<SeatLayoutTemplateResponseDto>(
                Error.Failure("VendorSeatLayoutService.GetByIdFailed", "An unexpected error occurred"));
        }
    }

    public async Task<Result<bool>> ApplyTemplateToLayoutAsync(int busId, int templateId, int vendorId)
    {
        try
        {
            if (busId <= 0 || templateId <= 0 || vendorId <= 0)
            {
                _logger.LogWarning("Invalid parameters - BusId: {BusId}, TemplateId: {TemplateId}, VendorId: {VendorId}", 
                    busId, templateId, vendorId);
                return Result.Failure<bool>(
                    Error.Failure("VendorSeatLayoutService.InvalidParameters", "All IDs must be greater than 0"));
            }

            _logger.LogInformation("Applying template {TemplateId} to bus {BusId} for vendor {VendorId}", 
                templateId, busId, vendorId);

            // Verify bus belongs to vendor
            var busOwnershipResult = await _busRepository.ExistsByIdAndVendorAsync(busId, vendorId);
            if (busOwnershipResult.IsFailure)
            {
                _logger.LogError("Failed to verify bus ownership: {Error}", busOwnershipResult.Error.Description);
                return Result.Failure<bool>(busOwnershipResult.Error);
            }

            if (!busOwnershipResult.Value)
            {
                _logger.LogWarning("Vendor {VendorId} attempted to modify bus {BusId} they don't own", vendorId, busId);
                return Result.Failure<bool>(
                    Error.Failure("VendorSeatLayoutService.BusNotOwned", "You can only modify your own buses"));
            }

            // Verify template exists
            var templateResult = await _seatLayoutRepository.GetTemplateByIdAsync(templateId);
            if (templateResult.IsFailure)
            {
                _logger.LogError("Template not found: {TemplateId}", templateId);
                return Result.Failure<bool>(templateResult.Error);
            }

            // Apply template to bus
            var applyResult = await _seatLayoutRepository.ApplyTemplateToLayoutAsync(busId, templateId);
            if (applyResult.IsFailure)
            {
                _logger.LogError("Failed to apply template: {Error}", applyResult.Error.Description);
                return Result.Failure<bool>(applyResult.Error);
            }

            _logger.LogInformation("Successfully applied template {TemplateId} to bus {BusId}", templateId, busId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error applying template to layout: {Message}", ex.Message);
            return Result.Failure<bool>(
                Error.Failure("VendorSeatLayoutService.ApplyTemplateFailed", "An unexpected error occurred"));
        }
    }
}