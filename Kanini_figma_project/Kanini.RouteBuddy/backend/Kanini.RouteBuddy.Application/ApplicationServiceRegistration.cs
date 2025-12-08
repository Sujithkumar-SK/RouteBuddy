using Kanini.RouteBuddy.Application.AutoMapper;
using Kanini.RouteBuddy.Application.Common;
using Kanini.RouteBuddy.Application.Services;
using Kanini.RouteBuddy.Application.Services.Admin;
using Kanini.RouteBuddy.Application.Services.Booking;
using Kanini.RouteBuddy.Application.Services.Buses;
using Kanini.RouteBuddy.Application.Services.Customer;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Application.Services.Pdf;
using Kanini.RouteBuddy.Application.Services.SmartEnigne;
using Kanini.RouteBuddy.Application.Services.Route;
using Kanini.RouteBuddy.Application.Services.RouteStop;
using Kanini.RouteBuddy.Application.Services.Stop;
using Kanini.RouteBuddy.Application.Services.User;
using Kanini.RouteBuddy.Application.Services.BusPhoto;
using Kanini.RouteBuddy.Application.Services.Schedule;
using Kanini.RouteBuddy.Application.Services.OTP;
using Microsoft.Extensions.DependencyInjection;

namespace Kanini.RouteBuddy.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(AutoMapperConfiguration.Configure().CreateMapper());
            services.AddMemoryCache();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<Services.Auth.IAuthService, Services.Auth.AuthService>();
            services.AddScoped<Services.Auth.IJwtTokenService, Services.Auth.JwtTokenService>();
            services.AddScoped<Services.Auth.IJwtOtpService, Services.Auth.JwtOtpService>();
            services.AddScoped<Services.OTP.IOTPService, Services.OTP.OTPService>();
            services.AddScoped<IBusService, BusService>();
            services.AddScoped<IBusPhotoService, BusPhotoService>();
            services.AddScoped<IBus_Search_Book_Service, Bus_Search_Book_Service>();
            services.AddScoped<IStopService, StopService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IRouteStopService, RouteStopService>();
            services.AddScoped<ISmartEngineService, SmartEngineService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<ISmartEmailService, SmartEmailService>();
            services.AddScoped<IAdminSeatLayoutService, AdminSeatLayoutService>();
            services.AddScoped<IAdminBusService, AdminBusService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<Services.Vendor.IVendorService, Services.Vendor.VendorService>();
            services.AddScoped<Services.Vendor.IVendorAnalyticsService, Services.Vendor.VendorAnalyticsService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddHttpClient<ICaptchaService, CaptchaService>();
            services.AddHostedService<BookingExpiryService>();
            services.AddHostedService<TokenCleanupService>();
            return services;
        }
    }
}
