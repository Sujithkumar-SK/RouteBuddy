using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Application.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(config =>
            {
                ConfigureMappings(config);
            });

            return config;
        }

        private static void ConfigureMappings(IMapperConfigurationExpression config)
        {
            config.CreateMap<CreateUserRequestDto, User>();
            config.CreateMap<User, UserResponseDto>();
            config.AddProfile<AuthMappingProfile>();
            config.AddProfile<BusProfile>();
            config.AddProfile<BookingProfile>();
            config.AddProfile<RouteStopProfile>();
            config.AddProfile<BusFilterProfile>();
            config.AddProfile<ConnectingRouteProfile>();
            config.AddProfile<ConnectingBookingProfile>();
            config.AddProfile<SmartEmailProfile>();
            config.AddProfile<SeatLayoutProfile>();
            config.AddProfile<PaymentProfile>();
        }
    }
}
