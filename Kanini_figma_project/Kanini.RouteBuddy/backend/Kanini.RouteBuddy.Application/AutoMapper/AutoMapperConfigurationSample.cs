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
            config.AddProfile<BusMappingProfile>();
            config.AddProfile<BusPhotoMappingProfile>();
            config.AddProfile<BookingProfile>();
            config.AddProfile<BookingMappingProfile>();
            config.AddProfile<RouteStopProfile>();
            config.AddProfile<BusFilterProfile>();
            config.AddProfile<ConnectingRouteProfile>();
            config.AddProfile<ConnectingBookingProfile>();
            config.AddProfile<SeatLayoutProfile>();
            config.AddProfile<PaymentProfile>();
            config.AddProfile<CustomerProfileMappingProfile>();
            config.AddProfile<CustomerMappingProfile>();
            config.AddProfile<VendorMappingProfile>();
            config.AddProfile<VendorDocumentMappingProfile>();
            config.AddProfile<VendorApprovalMappingProfile>();
            config.AddProfile<PlaceAutocompleteProfile>();
            config.AddProfile<RouteMappingProfile>();
            config.AddProfile<ScheduleMappingProfile>();
            config.AddProfile<StopMappingProfile>();
        }
    }
}
