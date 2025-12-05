using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Infrastructure;
using Kanini.RouteBuddy.Data.Repositories;
using Kanini.RouteBuddy.Data.Repositories.Admin;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Data.Repositories.BusPhoto;
using Kanini.RouteBuddy.Data.Repositories.Email;
using Kanini.RouteBuddy.Data.Repositories.SmartEngine;
using Kanini.RouteBuddy.Data.Repositories.Stop;
using Kanini.RouteBuddy.Data.Repositories.RouteStop;
using Kanini.RouteBuddy.Data.Repositories.Token;
using Kanini.RouteBuddy.Data.Repositories.User;
using Kanini.RouteBuddy.Data.Repositories.Vendor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kanini.RouteBuddy.Data
{
    public static class DataServiceRegistration
    {
        public static IServiceCollection AddDataServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<RouteBuddyDatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DatabaseConnectionString"));
            });
            
            // Infrastructure
            services.AddScoped<IDbReader, AdoDbReader>();
            
            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBusRepository, BusRepository>();
            services.AddScoped<IBusPhotoRepository, BusPhotoRepository>();
            services.AddScoped<IBus_Search_Book_Repository, Bus_Search_Book_Repository>();
            services.AddScoped<ISmartEngineRepository, SmartEngineRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<Kanini.RouteBuddy.Data.Repositories.Admin.ISeatLayoutRepository, Kanini.RouteBuddy.Data.Repositories.Admin.SeatLayoutRepository>();
            services.AddScoped<IVendorSeatLayoutRepository, VendorSeatLayoutRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped<Repositories.Customer.ICustomerRepository, Repositories.Customer.CustomerRepository>();
            services.AddScoped<Repositories.VendorDocuments.IVendorDocumentRepository, Repositories.VendorDocuments.VendorDocumentRepository>();
            services.AddScoped<Repositories.Booking.IBookingRepository, Repositories.Booking.BookingRepository>();
            services.AddScoped<Repositories.Booking.IBookingCancellationRepository, Repositories.Booking.BookingCancellationRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<Repositories.Schedule.IScheduleRepository, Repositories.Schedule.ScheduleRepository>();
            services.AddScoped<Repositories.Route.IRouteRepository, Repositories.Route.RouteRepository>();
            services.AddScoped<IRouteStopRepository, RouteStopRepository>();
            services.AddScoped<IStopRepository, StopRepository>();
            services.AddScoped<Repositories.OTP.IOTPRepository, Repositories.OTP.OTPRepository>();
            services.AddScoped<Repositories.VendorAnalytics.IVendorAnalyticsRepository, Repositories.VendorAnalytics.VendorAnalyticsRepository>();
            return services;
        }
    }
}
