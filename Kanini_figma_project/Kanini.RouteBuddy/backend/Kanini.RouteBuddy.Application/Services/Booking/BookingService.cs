using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Data.Repositories.Booking;
using Microsoft.Extensions.Logging;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.Services.Booking
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IBookingRepository bookingRepository, IMapper mapper, ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AdminBookingDTO>> GetAllBookingsAsync()
        {
            _logger.LogInformation("BookingService GetAllBookingsAsync started");
            _logger.LogInformation("Calling repository GetAllBookingsAsync");
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            _logger.LogInformation("Repository call completed");
            _logger.LogInformation("Starting AutoMapper mapping");
            var result = _mapper.Map<IEnumerable<AdminBookingDTO>>(bookings);
            _logger.LogInformation("AutoMapper mapping completed");
            _logger.LogInformation("BookingService GetAllBookingsAsync completed");
            return result;
        }

        public async Task<IEnumerable<AdminBookingDTO>> FilterBookingsAsync(string? searchName, int? status, DateTime? fromDate, DateTime? toDate)
        {
            _logger.LogInformation("BookingService FilterBookingsAsync started");
            _logger.LogInformation("Calling repository FilterBookingsAsync");
            var bookings = await _bookingRepository.FilterBookingsAsync(searchName, status, fromDate, toDate);
            _logger.LogInformation("Repository call completed");
            _logger.LogInformation("Starting AutoMapper mapping");
            var result = _mapper.Map<IEnumerable<AdminBookingDTO>>(bookings);
            _logger.LogInformation("AutoMapper mapping completed");
            _logger.LogInformation("BookingService FilterBookingsAsync completed");
            return result;
        }

        public async Task<AdminBookingDTO?> GetBookingByIdAsync(int bookingId)
        {
            _logger.LogInformation("BookingService GetBookingByIdAsync started");
            
            if (bookingId < 1 || bookingId > 999999)
            {
                _logger.LogWarning("Invalid booking ID {BookingId}", bookingId);
                return null;
            }
            
            _logger.LogInformation("Validation passed");
            _logger.LogInformation("Calling repository GetBookingByIdAsync");
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            _logger.LogInformation("Repository call completed");
            
            if (booking == null)
            {
                _logger.LogInformation("Booking not found, returning null");
                return null;
            }
            
            _logger.LogInformation("Starting AutoMapper mapping");
            var result = _mapper.Map<AdminBookingDTO>(booking);
            _logger.LogInformation("BookingService GetBookingByIdAsync completed");
            return result;
        }

        public async Task<BookingStatusSummaryDTO> GetBookingStatusSummaryAsync()
        {
            _logger.LogInformation("BookingService GetBookingStatusSummaryAsync started");
            _logger.LogInformation("Calling repository GetBookingStatusSummaryAsync");
            var (pending, confirmed, cancelled, total, revenue) = await _bookingRepository.GetBookingStatusSummaryAsync();
            _logger.LogInformation("Repository call completed");
            _logger.LogInformation("Creating DTO object");
            var result = new BookingStatusSummaryDTO
            {
                PendingBookings = pending,
                ConfirmedBookings = confirmed,
                CancelledBookings = cancelled,
                TotalBookings = total,
                TotalRevenue = revenue
            };
            _logger.LogInformation("DTO creation completed");
            _logger.LogInformation("BookingService GetBookingStatusSummaryAsync completed");
            return result;
        }


    }
}