using Kanini.RouteBuddy.Api.Constants;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Services.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.RouteBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminCustomerDTO>>> GetAllCustomers()
        {
            _logger.LogInformation("Getting all customers");
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                _logger.LogInformation("Retrieved {Count} customers", customers.Count());
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all customers");
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<AdminCustomerDTO>>> FilterCustomers(
            [FromQuery] string? searchName,
            [FromQuery] bool? isActive,
            [FromQuery] int? minAge,
            [FromQuery] int? maxAge)
        {
            try
            {
                // Enhanced validations
                if (!string.IsNullOrEmpty(searchName) && searchName.Length > 100)
                    return BadRequest(ErrorMessages.SearchNameTooLong);
                
                if (minAge.HasValue && (minAge < 0 || minAge > 120))
                    return BadRequest(ErrorMessages.AgeRangeInvalid);
                
                if (maxAge.HasValue && (maxAge < 0 || maxAge > 120))
                    return BadRequest(ErrorMessages.AgeRangeInvalid);
                
                if (minAge.HasValue && maxAge.HasValue && minAge > maxAge)
                    return BadRequest(ErrorMessages.MinAgeGreaterThanMaxAge);

                var customers = await _customerService.FilterCustomersAsync(searchName, isActive, minAge, maxAge);
                return Ok(customers);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in filter customers: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database connection error in filter customers");
                return StatusCode(503, string.Format(ErrorMessages.DatabaseConnectionError));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt in filter customers");
                return StatusCode(403, ErrorMessages.UnauthorizedAccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in filter customers");
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminCustomerDTO>> GetCustomerById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(ErrorMessages.CustomerIdInvalid);

                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound(string.Format(ErrorMessages.CustomerNotFound, id));

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteCustomer(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(ErrorMessages.CustomerIdInvalid);

                var result = await _customerService.SoftDeleteCustomerAsync(id);
                if (!result)
                    return NotFound(string.Format(ErrorMessages.CustomerNotFound, id));

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }
    }
}