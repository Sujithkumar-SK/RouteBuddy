using System.Security.Claims;
using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Services.Buses;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Services;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Vendor")]
public class BusController : ControllerBase
{
    private readonly IBusService _busService;
    private readonly ILogger<BusController> _logger;
    private const int MaxPageSize = 50;

    public BusController(IBusService busService, ILogger<BusController> logger)
    {
        _busService = busService;
        _logger = logger;
    }

    private async Task<int> GetVendorIdFromClaimsAsync()
    {
        try
        {
            if (User?.Identity?.IsAuthenticated != true)
                return 0;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return 0;

            var vendor = await _busService.GetVendorByUserIdAsync(userId);
            return vendor?.VendorId ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.VendorIdRetrievalFailed, ex.Message);
            return 0;
        }
    }

    [HttpGet("my-buses")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<BusResponseDto>>>> GetMyBuses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] BusStatus? status = null,
        [FromQuery] BusType? busType = null,
        [FromQuery] string? search = null
    )
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<PagedResultDto<BusResponseDto>>.ErrorResult(
                        "Invalid vendor authentication"
                    )
                );

            _logger.LogInformation(
                BusMessages.LogMessages.GettingBusesByVendor,
                vendorId,
                pageNumber,
                pageSize
            );

            if (pageNumber <= 0)
                pageNumber = 1;
            if (pageSize <= 0 || pageSize > MaxPageSize)
                pageSize = 10;

            var result = await _busService.GetBusesByVendorAsync(
                vendorId,
                pageNumber,
                pageSize,
                status,
                busType,
                search
            );

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    BusMessages.LogMessages.BusesByVendorRetrievedSuccessfully,
                    result.Value.Data.Count(),
                    vendorId
                );

                // Check if search returned no results
                if (!string.IsNullOrWhiteSpace(search) && result.Value.Data.Count() == 0)
                {
                    return Ok(
                        ApiResponseDto<PagedResultDto<BusResponseDto>>.SuccessResult(
                            result.Value,
                            $"No buses found matching '{search}'"
                        )
                    );
                }

                return Ok(
                    ApiResponseDto<PagedResultDto<BusResponseDto>>.SuccessResult(result.Value)
                );
            }

            _logger.LogWarning(BusMessages.LogMessages.BusesByVendorRetrievalFailed, vendorId);
            return BadRequest(
                ApiResponseDto<PagedResultDto<BusResponseDto>>.ErrorResult(result.Error.Description)
            );
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                BusMessages.LogMessages.BusesByVendorException,
                await GetVendorIdFromClaimsAsync()
            );
            return StatusCode(
                500,
                ApiResponseDto<PagedResultDto<BusResponseDto>>.ErrorResult(
                    BusMessages.ErrorMessages.DatabaseError
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                BusMessages.LogMessages.BusesByVendorException,
                await GetVendorIdFromClaimsAsync()
            );
            return StatusCode(
                500,
                ApiResponseDto<PagedResultDto<BusResponseDto>>.ErrorResult(
                    BusMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<BusResponseDto>>> CreateBus(
        [FromForm] CreateBusDto dto
    )
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<BusResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            _logger.LogInformation(BusMessages.LogMessages.CreatingBus, dto?.BusName ?? "Unknown");

            if (dto == null)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusCreationFailed, "Unknown");
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        BusMessages.ErrorMessages.InvalidBusData
                    )
                );
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var errorMessage = string.Join("; ", errors.SelectMany(e => e.Value));
                _logger.LogWarning(
                    BusMessages.LogMessages.BusCreationFailed,
                    dto?.BusName ?? "Unknown"
                );
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult($"Validation failed: {errorMessage}")
                );
            }

            // Additional file validation
            if (dto.RegistrationCertificate == null || dto.RegistrationCertificate.Length == 0)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusCreationFailed, dto.BusName ?? "Unknown");
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        "Registration certificate file is required"
                    )
                );
            }

            // Validate file size (5MB limit)
            if (dto.RegistrationCertificate.Length > 5 * 1024 * 1024)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusCreationFailed, dto.BusName ?? "Unknown");
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        "Registration certificate file must be under 5MB"
                    )
                );
            }

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var fileName = dto.RegistrationCertificate.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogWarning(BusMessages.LogMessages.BusCreationFailed, dto.BusName ?? "Unknown");
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        "Registration certificate file name is required"
                    )
                );
            }
            
            var fileExtension = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                _logger.LogWarning(BusMessages.LogMessages.BusCreationFailed, dto.BusName ?? "Unknown");
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        "Registration certificate must be a PDF, JPG, JPEG, or PNG file"
                    )
                );
            }

            var result = await _busService.CreateBusAsync(dto, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    BusMessages.LogMessages.BusCreatedSuccessfully,
                    result.Value.BusId
                );
                return Ok(ApiResponseDto<BusResponseDto>.SuccessResult(result.Value));
            }

            _logger.LogWarning(BusMessages.LogMessages.BusCreationFailed, dto.BusName ?? "Unknown");
            return BadRequest(ApiResponseDto<BusResponseDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                BusMessages.LogMessages.BusCreationException,
                dto?.BusName ?? "Unknown"
            );
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(BusMessages.ErrorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                BusMessages.LogMessages.BusCreationException,
                dto?.BusName ?? "Unknown"
            );
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(
                    BusMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<BusResponseDto>>> GetById(int id)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<BusResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            _logger.LogInformation(BusMessages.LogMessages.GettingBusById, id);

            if (id <= 0)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, id);
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        BusMessages.ErrorMessages.InvalidBusData
                    )
                );
            }

            var result = await _busService.GetBusByIdAsync(id, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation(BusMessages.LogMessages.BusRetrievedSuccessfully, id);
                return Ok(ApiResponseDto<BusResponseDto>.SuccessResult(result.Value));
            }

            _logger.LogWarning(BusMessages.LogMessages.BusNotFound, id);
            return NotFound(ApiResponseDto<BusResponseDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusRetrievalException, id);
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(BusMessages.ErrorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusRetrievalException, id);
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(
                    BusMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpPut("{id}/activate")]
    public async Task<ActionResult<ApiResponseDto<BusResponseDto>>> ActivateBus(int id)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<BusResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            _logger.LogInformation(BusMessages.LogMessages.ActivatingBus, id);

            if (id <= 0)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, id);
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        BusMessages.ErrorMessages.InvalidBusData
                    )
                );
            }

            var result = await _busService.ActivateBusAsync(id, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation(BusMessages.LogMessages.BusActivatedSuccessfully, id);
                return Ok(ApiResponseDto<BusResponseDto>.SuccessResult(result.Value));
            }

            _logger.LogWarning(BusMessages.LogMessages.BusActivationFailed, id);
            return BadRequest(ApiResponseDto<BusResponseDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusActivationException, id);
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(BusMessages.ErrorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusActivationException, id);
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(
                    BusMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpPut("{id}/deactivate")]
    public async Task<ActionResult<ApiResponseDto<BusResponseDto>>> DeactivateBus(int id)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<BusResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            _logger.LogInformation(BusMessages.LogMessages.DeactivatingBus, id);

            if (id <= 0)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, id);
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        BusMessages.ErrorMessages.InvalidBusData
                    )
                );
            }

            var result = await _busService.DeactivateBusAsync(id, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation(BusMessages.LogMessages.BusDeactivatedSuccessfully, id);
                return Ok(ApiResponseDto<BusResponseDto>.SuccessResult(result.Value));
            }

            _logger.LogWarning(BusMessages.LogMessages.BusDeactivationFailed, id);
            return BadRequest(ApiResponseDto<BusResponseDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusDeactivationException, id);
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(BusMessages.ErrorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusDeactivationException, id);
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(
                    BusMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpPut("{id}/maintenance")]
    public async Task<ActionResult<ApiResponseDto<BusResponseDto>>> SetMaintenance(int id)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<BusResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            _logger.LogInformation(BusMessages.LogMessages.SettingBusMaintenance, id);

            if (id <= 0)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, id);
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        BusMessages.ErrorMessages.InvalidBusData
                    )
                );
            }

            var result = await _busService.SetMaintenanceAsync(id, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation(BusMessages.LogMessages.BusMaintenanceSetSuccessfully, id);
                return Ok(ApiResponseDto<BusResponseDto>.SuccessResult(result.Value));
            }

            _logger.LogWarning(BusMessages.LogMessages.BusMaintenanceFailed, id);
            return BadRequest(ApiResponseDto<BusResponseDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusMaintenanceException, id);
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(BusMessages.ErrorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, BusMessages.LogMessages.BusMaintenanceException, id);
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(
                    BusMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<BusResponseDto>>> Update(
        int id,
        [FromForm] UpdateBusDto dto
    )
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<BusResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            if (id <= 0 || dto == null)
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(
                        BusMessages.ErrorMessages.InvalidBusData
                    )
                );

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(
                    ApiResponseDto<BusResponseDto>.ErrorResult(string.Join("; ", errors))
                );
            }

            // First check if bus belongs to vendor
            var busResult = await _busService.GetBusByIdAsync(id, vendorId);
            if (busResult.IsFailure)
                return NotFound(
                    ApiResponseDto<BusResponseDto>.ErrorResult(busResult.Error.Description)
                );

            var result = await _busService.UpdateBusAsync(id, dto);

            if (result.IsSuccess)
                return Ok(ApiResponseDto<BusResponseDto>.SuccessResult(result.Value));

            return BadRequest(ApiResponseDto<BusResponseDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException)
        {
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(BusMessages.ErrorMessages.DatabaseError)
            );
        }
        catch (Exception)
        {
            return StatusCode(
                500,
                ApiResponseDto<BusResponseDto>.ErrorResult(
                    BusMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            if (id <= 0)
                return BadRequest(
                    ApiResponseDto<object>.ErrorResult(BusMessages.ErrorMessages.InvalidBusData)
                );

            // First check if bus belongs to vendor
            var busResult = await _busService.GetBusByIdAsync(id, vendorId);
            if (busResult.IsFailure)
                return NotFound(ApiResponseDto<object>.ErrorResult(busResult.Error.Description));

            var result = await _busService.DeleteBusAsync(id);

            if (result.IsSuccess)
                return Ok(ApiResponseDto<object>.SuccessResult(null!));

            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (SqlException)
        {
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(BusMessages.ErrorMessages.DatabaseError)
            );
        }
        catch (Exception)
        {
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("awaiting-confirmation")]
    public async Task<ActionResult<ApiResponseDto<List<BusResponseDto>>>> GetAwaitingConfirmation()
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<List<BusResponseDto>>.ErrorResult(
                        "Invalid vendor authentication"
                    )
                );

            _logger.LogInformation(
                BusMessages.LogMessages.GettingAwaitingConfirmationBuses,
                vendorId
            );

            var result = await _busService.GetAwaitingConfirmationAsync(vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    BusMessages.LogMessages.AwaitingConfirmationBusesRetrievedSuccessfully,
                    result.Value.Count,
                    vendorId
                );
                return Ok(ApiResponseDto<List<BusResponseDto>>.SuccessResult(result.Value));
            }

            _logger.LogWarning(
                BusMessages.LogMessages.AwaitingConfirmationBusesRetrievalFailed,
                vendorId
            );
            return BadRequest(
                ApiResponseDto<List<BusResponseDto>>.ErrorResult(result.Error.Description)
            );
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                BusMessages.LogMessages.AwaitingConfirmationBusesException,
                0 // Cannot await in catch block
            );
            return StatusCode(
                500,
                ApiResponseDto<List<BusResponseDto>>.ErrorResult(
                    BusMessages.ErrorMessages.DatabaseError
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                BusMessages.LogMessages.AwaitingConfirmationBusesException,
                0 // Cannot await in catch block
            );
            return StatusCode(
                500,
                ApiResponseDto<List<BusResponseDto>>.ErrorResult(
                    BusMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpPost("{busId}/apply-template/{templateId}")]
    public async Task<IActionResult> ApplyTemplate(int busId, int templateId)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            if (busId <= 0 || templateId <= 0)
                return BadRequest(
                    ApiResponseDto<object>.ErrorResult("Invalid bus ID or template ID")
                );

            _logger.LogInformation(MagicStrings.LogMessages.BusTemplateApplicationStarted, busId, templateId);

            var result = await _busService.ApplyTemplateAsync(busId, templateId, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.BusTemplateApplicationCompleted, busId);
                return Ok(ApiResponseDto<object>.SuccessResult(new { }, "Template applied successfully"));
            }

            _logger.LogWarning(MagicStrings.LogMessages.BusTemplateApplicationFailed, result.Error.Description);
            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BusTemplateApplicationFailed, ex.Message);
            return StatusCode(500, ApiResponseDto<object>.ErrorResult("Template application failed"));
        }
    }
}
