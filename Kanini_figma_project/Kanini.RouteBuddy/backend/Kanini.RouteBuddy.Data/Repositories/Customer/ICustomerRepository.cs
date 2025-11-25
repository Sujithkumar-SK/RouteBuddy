using System.Threading.Tasks;
using CustomerEntity = Kanini.RouteBuddy.Domain.Entities.Customer;

namespace Kanini.RouteBuddy.Data.Repositories.Customer
{
    /// <summary>
    /// Interface defining operations for managing Customer data.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Adds a new customer to the database (EF Core write operation).
        /// </summary>
        Task AddCustomerAsync(CustomerEntity customer);
        
        /// <summary>
        /// Creates a new customer to the database (EF Core write operation).
        /// </summary>
        Task CreateCustomerAsync(CustomerEntity customer);

        /// <summary>
        /// Retrieves customer details by email using ADO.NET (Stored Procedure).
        /// </summary>
        Task<CustomerEntity?> GetCustomerByEmailAsync(string email);




        //jack

        Task<IEnumerable<CustomerEntity>> GetAllCustomersAsync();
        Task<IEnumerable<CustomerEntity>> FilterCustomersAsync(string? searchName, bool? isActive, int? minAge, int? maxAge);
        Task<CustomerEntity?> GetCustomerByIdAsync(int customerId);
        Task<bool> SoftDeleteCustomerAsync(int customerId);
    }
}
