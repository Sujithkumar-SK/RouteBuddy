using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Kanini.RouteBuddy.Application.Services.Customer
{
    public interface ICustomerService
    {
        Task<IEnumerable<AdminCustomerDTO>> GetAllCustomersAsync();
        Task<IEnumerable<AdminCustomerDTO>> FilterCustomersAsync(string? searchName, bool? isActive, int? minAge, int? maxAge);
        Task<AdminCustomerDTO?> GetCustomerByIdAsync(int customerId);
        Task<bool> SoftDeleteCustomerAsync(int customerId);
        Task<CustomerProfileDto?> GetCustomerProfileAsync(int customerId);
        Task<CustomerProfileDto?> GetCustomerProfileByUserIdAsync(int userId);
        Task<bool> UpdateCustomerProfileAsync(int customerId, UpdateCustomerProfileDto updateDto);
        Task<IEnumerable<CustomerBookingDto>> GetCustomerBookingsAsync(int customerId, BookingStatus? status, DateTime? fromDate, DateTime? toDate);
        Task<byte[]?> GenerateTicketAsync(int customerId, int bookingId);
        Task<bool> UpdateCustomerProfilePictureAsync(int customerId, IFormFile profilePicture);
        Task<CancelBookingResponseDto> CancelBookingAsync(int customerId, CancelBookingRequestDto request);
    }
}