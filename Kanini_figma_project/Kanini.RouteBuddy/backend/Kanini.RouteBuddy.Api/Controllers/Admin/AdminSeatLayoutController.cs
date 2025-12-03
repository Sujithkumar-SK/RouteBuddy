using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Services.Admin;
using Kanini.RouteBuddy.Common;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.RouteBuddy.Api.Controllers;

[Route("api/admin/[controller]")]
[ApiController]
public class AdminSeatLayoutController : ControllerBase
{
    private readonly IAdminSeatLayoutService _adminSeatLayoutService;
    private readonly ILogger<AdminSeatLayoutController> _logger;

    public AdminSeatLayoutController(
        IAdminSeatLayoutService adminSeatLayoutService,
        ILogger<AdminSeatLayoutController> logger
    )
    {
        _adminSeatLayoutService = adminSeatLayoutService;
        _logger = logger;
    }

    [HttpGet("seat-layout-templates")]
    public async Task<IActionResult> GetTemplates([FromQuery] int? busType = null)
    {
        try
        {
            if (busType.HasValue)
            {
                _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeStarted, busType.Value);
                
                var result = await _adminSeatLayoutService.GetTemplatesByBusTypeAsync(busType.Value);
                
                if (result.IsFailure)
                {
                    _logger.LogError(
                        MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeFailed,
                        result.Error.Description
                    );
                    return BadRequest(result.Error);
                }
                
                _logger.LogInformation(
                    MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeCompleted,
                    result.Value.Count
                );
                return Ok(result.Value);
            }
            else
            {
                _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplateGetAllStarted);

                var result = await _adminSeatLayoutService.GetAllTemplatesAsync();

                if (result.IsFailure)
                {
                    _logger.LogError(
                        MagicStrings.LogMessages.SeatLayoutTemplateGetAllFailed,
                        result.Error.Description
                    );
                    return BadRequest(result.Error);
                }

                _logger.LogInformation(
                    MagicStrings.LogMessages.SeatLayoutTemplateGetAllCompleted,
                    result.Value.Count
                );
                return Ok(result.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateGetAllFailed,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("seat-layout-templates/{id}")]
    public async Task<IActionResult> GetTemplateById(int id)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplateGetByIdStarted, id);

            var result = await _adminSeatLayoutService.GetTemplateByIdAsync(id);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplateGetByIdCompleted, id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("seat-layout-templates")]
    public async Task<IActionResult> CreateTemplate(
        [FromBody] CreateSeatLayoutTemplateRequestDto request
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateCreationStarted,
                request.TemplateName
            );

            var result = await _adminSeatLayoutService.CreateTemplateAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateCreationFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateCreationCompleted,
                result.Value.SeatLayoutTemplateId
            );
            return CreatedAtAction(
                nameof(GetTemplateById),
                new { id = result.Value.SeatLayoutTemplateId },
                result.Value
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateCreationFailed,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPut("seat-layout-templates/{id}")]
    public async Task<IActionResult> UpdateTemplate(
        int id,
        [FromBody] UpdateSeatLayoutTemplateRequestDto request
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplateUpdateStarted, id);

            var result = await _adminSeatLayoutService.UpdateTemplateAsync(id, request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateUpdateFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplateUpdateCompleted, id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateUpdateFailed,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpDelete("seat-layout-templates/{id}")]
    public async Task<IActionResult> DeactivateTemplate(int id)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateDeactivationStarted,
                id
            );

            var result = await _adminSeatLayoutService.DeactivateTemplateAsync(id);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateDeactivationFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateDeactivationCompleted,
                id
            );
            return Ok(new { Message = result.Value });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateDeactivationFailed,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}
