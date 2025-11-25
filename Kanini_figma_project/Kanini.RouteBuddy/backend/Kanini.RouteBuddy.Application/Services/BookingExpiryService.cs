using Kanini.RouteBuddy.Application.Services.Buses;
using Kanini.RouteBuddy.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services;

public class BookingExpiryService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BookingExpiryService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);

    public BookingExpiryService(
        IServiceProvider serviceProvider,
        ILogger<BookingExpiryService> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExpirePendingBookingsAsync();
                await Task.Delay(_interval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MagicStrings.LogMessages.BookingExpiryFailed, ex.Message);
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }

    private async Task ExpirePendingBookingsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var busService = scope.ServiceProvider.GetRequiredService<IBus_Search_Book_Service>();

        var result = await busService.ExpirePendingBookingsAsync();

        if (result.IsFailure)
        {
            _logger.LogError(
                MagicStrings.LogMessages.BookingExpiryFailed,
                result.Error.Description
            );
        }
    }
}
