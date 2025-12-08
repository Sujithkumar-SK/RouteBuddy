using Kanini.RouteBuddy.Api.Constants;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Application.Services.Vendor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.RouteBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/admin/vendors")]
    [Authorize(Roles = "Admin")]
    public class AdminVendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger<AdminVendorController> _logger;

        public AdminVendorController(IVendorService vendorService, ILogger<AdminVendorController> logger)
        {
            _vendorService = vendorService;
            _logger = logger;
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<AdminVendorDTO>>> GetPendingVendors(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting pending vendors");
            try
            {
                var vendors = await _vendorService.GetPendingVendorsForAdminAsync(pageNumber, pageSize);
                _logger.LogInformation("Retrieved {Count} pending vendors", vendors.Data.Count());
                return Ok(vendors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting pending vendors");
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminVendorDTO>>> GetAllVendors(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var vendors = await _vendorService.GetAllVendorsForAdminAsync(pageNumber, pageSize);
                return Ok(vendors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<AdminVendorDTO>>> FilterVendors(
            [FromQuery] string? searchName,
            [FromQuery] bool? isActive,
            [FromQuery] int? status)
        {
            try
            {
                // Enhanced validations
                if (!string.IsNullOrEmpty(searchName) && searchName.Length > 100)
                    return BadRequest(ErrorMessages.SearchNameTooLong);
                
                if (status.HasValue && (status < 0 || status > 2))
                    return BadRequest(ErrorMessages.VendorStatusInvalid);

                var vendors = await _vendorService.FilterVendorsAsync(searchName, isActive, status);
                if (vendors.IsFailure)
                    return BadRequest(vendors.Error.Description);
                return Ok(vendors.Value);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database connection error while filtering vendors");
                return StatusCode(503, ErrorMessages.DatabaseConnectionError);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, ErrorMessages.UnauthorizedAccess);
            }
            catch (Exception ex)
            {
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("{vendorId}")]
        public async Task<ActionResult<AdminVendorDTO>> GetVendorById(int vendorId)
        {
            try
            {
                if (vendorId <= 0)
                    return BadRequest(ErrorMessages.VendorIdInvalid);

                var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
                if (vendor.IsFailure)
                    return NotFound(vendor.Error.Description);

                return Ok(vendor.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vendor {VendorId}", vendorId);
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("{vendorId}/approval")]
        public async Task<ActionResult> GetVendorForApproval(int vendorId)
        {
            try
            {
                if (vendorId <= 0)
                    return BadRequest(ErrorMessages.VendorIdInvalid);

                var result = await _vendorService.GetVendorForApprovalAsync(vendorId);
                if (result.IsFailure)
                    return NotFound(result.Error.Description);

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vendor for approval {VendorId}", vendorId);
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpPatch("approve/{vendorId}")]
        public async Task<ActionResult> ApproveVendor(int vendorId)
        {
            try
            {
                if (vendorId <= 0)
                    return BadRequest(ErrorMessages.VendorIdInvalid);

                var result = await _vendorService.ApproveVendorWithEmailAsync(vendorId);
                if (result.IsFailure)
                    return NotFound(result.Error.Description);

                return Ok(new { message = "Vendor approved and notification email sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpPatch("reject/{vendorId}")]
        public async Task<ActionResult> RejectVendor(int vendorId, [FromBody] VendorRejectionDTO rejectionDto)
        {
            try
            {
                if (vendorId <= 0)
                    return BadRequest(ErrorMessages.VendorIdInvalid);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Rejecting vendor {VendorId} with reason: {Reason}", vendorId, rejectionDto.RejectionReason);
                
                var result = await _vendorService.RejectVendorWithReasonAsync(vendorId, rejectionDto.RejectionReason);
                if (result.IsFailure)
                    return NotFound(result.Error.Description);

                _logger.LogInformation("Vendor {VendorId} rejected and email sent", vendorId);
                return Ok(new { message = "Vendor rejected and notification email sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting vendor {VendorId}", vendorId);
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpPut("deactivate/{vendorId}")]
        public async Task<ActionResult> DeactivateVendor(int vendorId, [FromBody] VendorRejectionDTO reasonDto)
        {
            try
            {
                if (vendorId <= 0)
                    return BadRequest(ErrorMessages.VendorIdInvalid);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _vendorService.DeactivateVendorWithReasonAsync(vendorId, reasonDto.RejectionReason);
                if (result.IsFailure)
                    return NotFound(result.Error.Description);

                return Ok(new { message = "Vendor deactivated and notification email sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpPut("reactivate/{vendorId}")]
        public async Task<ActionResult> ReactivateVendor(int vendorId, [FromBody] VendorRejectionDTO reasonDto)
        {
            try
            {
                if (vendorId <= 0)
                    return BadRequest(ErrorMessages.VendorIdInvalid);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _vendorService.ReactivateVendorWithReasonAsync(vendorId, reasonDto.RejectionReason);
                if (result.IsFailure)
                    return NotFound(result.Error.Description);

                return Ok(new { message = "Vendor reactivated and notification email sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }
    }
}