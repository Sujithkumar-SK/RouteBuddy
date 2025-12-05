using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Common;

namespace Kanini.RouteBuddy.Data.Repositories.OTP;

public class OTPRepository : IOTPRepository
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<OTPRepository> _logger;

    public OTPRepository(IMemoryCache cache, ILogger<OTPRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task StoreOTPAsync(string email, string otp, DateTime expiryTime)
    {
        try
        {
            var cacheKey = $"OTP_{email}";
            var otpData = new { OTP = otp, ExpiryTime = expiryTime };
            
            _cache.Set(cacheKey, otpData, expiryTime);
            _logger.LogInformation("OTP stored in cache for email: {Email}", email);
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing OTP in cache for email: {Email}", email);
            throw new Exception(MagicStrings.ErrorMessages.InternalServerError);
        }
    }

    public Task<bool> ValidateOTPAsync(string email, string otp)
    {
        try
        {
            var cacheKey = $"OTP_{email}";
            
            if (_cache.TryGetValue(cacheKey, out var cachedData) && cachedData != null)
            {
                var otpData = (dynamic)cachedData;
                return Task.FromResult(otpData.OTP == otp && DateTime.UtcNow <= otpData.ExpiryTime);
            }
            
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating OTP for email: {Email}", email);
            throw new Exception(MagicStrings.ErrorMessages.InternalServerError);
        }
    }

    public async Task DeleteOTPAsync(string email)
    {
        try
        {
            var cacheKey = $"OTP_{email}";
            _cache.Remove(cacheKey);
            _logger.LogInformation("OTP removed from cache for email: {Email}", email);
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing OTP from cache for email: {Email}", email);
            throw new Exception(MagicStrings.ErrorMessages.InternalServerError);
        }
    }
}