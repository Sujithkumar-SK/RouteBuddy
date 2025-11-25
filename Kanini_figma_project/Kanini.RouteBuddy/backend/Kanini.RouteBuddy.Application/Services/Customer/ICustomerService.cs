using Kanini.RouteBuddy.Application.Dto.Admin;

namespace Kanini.RouteBuddy.Application.Services.Customer
{
    public interface ICustomerService
    {
        Task<IEnumerable<AdminCustomerDTO>> GetAllCustomersAsync();
        Task<IEnumerable<AdminCustomerDTO>> FilterCustomersAsync(string? searchName, bool? isActive, int? minAge, int? maxAge);
        Task<AdminCustomerDTO?> GetCustomerByIdAsync(int customerId);
        Task<bool> SoftDeleteCustomerAsync(int customerId);
    }
}