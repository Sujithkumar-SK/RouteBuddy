using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Data.Repositories.Customer;
using Microsoft.Extensions.Logging;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AdminCustomerDTO>> GetAllCustomersAsync()
        {
            _logger.LogInformation("CustomerService GetAllCustomersAsync started");
            _logger.LogInformation("Calling repository GetAllCustomersAsync");
            var customers = await _customerRepository.GetAllCustomersAsync();
            _logger.LogInformation("Repository call completed");
            _logger.LogInformation("Starting AutoMapper mapping");
            var result = _mapper.Map<IEnumerable<AdminCustomerDTO>>(customers);
            _logger.LogInformation("AutoMapper mapping completed");
            _logger.LogInformation("CustomerService GetAllCustomersAsync completed");
            return result;
        }

        public async Task<IEnumerable<AdminCustomerDTO>> FilterCustomersAsync(string? searchName, bool? isActive, int? minAge, int? maxAge)
        {
            _logger.LogInformation("CustomerService FilterCustomersAsync started");
            _logger.LogInformation("Calling repository FilterCustomersAsync");
            var customers = await _customerRepository.FilterCustomersAsync(searchName, isActive, minAge, maxAge);
            _logger.LogInformation("Repository call completed");
            _logger.LogInformation("Starting AutoMapper mapping");
            var result = _mapper.Map<IEnumerable<AdminCustomerDTO>>(customers);
            _logger.LogInformation("AutoMapper mapping completed");
            _logger.LogInformation("CustomerService FilterCustomersAsync completed");
            return result;
        }

        public async Task<AdminCustomerDTO?> GetCustomerByIdAsync(int customerId)
        {
            _logger.LogInformation("CustomerService GetCustomerByIdAsync started");

            if (customerId < 1 || customerId > 999999)
            {
                _logger.LogWarning("Invalid customer ID {CustomerId}", customerId);
                return null;
            }

            _logger.LogInformation("Validation passed");
            _logger.LogInformation("Calling repository GetCustomerByIdAsync");
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            _logger.LogInformation("Repository call completed");

            if (customer == null)
            {
                _logger.LogInformation("Customer not found, returning null");
                return null;
            }

            _logger.LogInformation("Starting AutoMapper mapping");
            var result = _mapper.Map<AdminCustomerDTO>(customer);
            _logger.LogInformation("CustomerService GetCustomerByIdAsync completed");
            return result;
        }

        public async Task<bool> SoftDeleteCustomerAsync(int customerId)
        {
            _logger.LogInformation("CustomerService SoftDeleteCustomerAsync started");

            if (customerId < 1 || customerId > 999999)
            {
                _logger.LogWarning("Invalid customer ID {CustomerId}", customerId);
                return false;
            }

            _logger.LogInformation("Validation passed");
            _logger.LogInformation("Calling repository SoftDeleteCustomerAsync");
            var result = await _customerRepository.SoftDeleteCustomerAsync(customerId);
            _logger.LogInformation("Repository call completed");

            if (result)
            {
                _logger.LogInformation("Customer deleted {CustomerId}", customerId);
                return true;
            }

            _logger.LogWarning("Customer delete failed {CustomerId}", customerId);
            return false;
        }
    }
}