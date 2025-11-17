using Kanini.RouteBuddy.Application.AutoMapper;
using Kanini.RouteBuddy.Application.Services;
using Kanini.RouteBuddy.Application.Services.Admin;
using Kanini.RouteBuddy.Application.Services.Buses;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Application.Services.Pdf;
using Kanini.RouteBuddy.Application.Services.SmartEnigne;
using Kanini.RouteBuddy.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace Kanini.RouteBuddy.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(AutoMapperConfiguration.Configure().CreateMapper());
            //services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBusService, BusService>();
            services.AddScoped<ISmartEngineService, SmartEngineService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<ISmartEmailService, SmartEmailService>();
            services.AddScoped<ISmartPdfService, SmartPdfService>();
            services.AddScoped<IAdminSeatLayoutService, AdminSeatLayoutService>();
            services.AddHostedService<BookingExpiryService>();
            return services;
        }
    }
}
