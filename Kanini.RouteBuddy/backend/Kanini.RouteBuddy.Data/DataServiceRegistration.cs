using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Repositories.Admin;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Data.Repositories.Email;
using Kanini.RouteBuddy.Data.Repositories.SmartEngine;
using Kanini.RouteBuddy.Data.Repositories.User;
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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBusRepository, BusRepository>();
            services.AddScoped<ISmartEngineRepository, SmartEngineRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<ISmartEmailRepository, SmartEmailRepository>();
            services.AddScoped<ISeatLayoutRepository, SeatLayoutRepository>();
            return services;
        }
    }
}
