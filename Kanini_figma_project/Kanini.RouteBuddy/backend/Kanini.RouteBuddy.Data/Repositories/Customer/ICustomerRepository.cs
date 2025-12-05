using System.Threading.Tasks;
using CustomerEntity = Kanini.RouteBuddy.Domain.Entities.Customer;
using BookingEntity = Kanini.RouteBuddy.Domain.Entities.Booking;
using Kanini.RouteBuddy.Domain.Enums;
using Kanini.RouteBuddy.Data.Models;
using System.Data;

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
        Task<CustomerEntity?> GetCustomerProfileByIdAsync(int customerId);
        Task<CustomerEntity?> GetCustomerProfileByUserIdAsync(int userId);
        Task<bool> UpdateCustomerProfileAsync(int customerId, string firstName, string? middleName, string lastName, DateTime dateOfBirth, int gender, string phone);
        Task<bool> UpdateCustomerProfilePictureAsync(int customerId, byte[] profilePicture);
        Task<IEnumerable<BookingEntity>> GetCustomerBookingsAsync(int customerId, BookingStatus? status, DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<BookingWithDetails>> GetCustomerBookingsWithDetailsAsync(int customerId, BookingStatus? status, DateTime? fromDate, DateTime? toDate);
    }
}
