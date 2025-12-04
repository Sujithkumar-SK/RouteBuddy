using Kanini.RouteBuddy.Data.Repositories.Token;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;

    public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Token Cleanup Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

                _logger.LogInformation("Running token cleanup job");
                var result = await refreshTokenRepository.DeleteExpiredTokensAsync();

                if (result.IsSuccess)
                    _logger.LogInformation("Token cleanup completed successfully");
                else
                    _logger.LogWarning("Token cleanup failed: {Error}", result.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in token cleanup service");
            }
        }

        _logger.LogInformation("Token Cleanup Service stopped");
    }
}
