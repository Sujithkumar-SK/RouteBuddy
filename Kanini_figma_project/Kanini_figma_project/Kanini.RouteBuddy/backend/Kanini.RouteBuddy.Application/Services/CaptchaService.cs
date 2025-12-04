using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kanini.RouteBuddy.Application.Services;

public class CaptchaService : ICaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;
    private readonly ILogger<CaptchaService> _logger;

    public CaptchaService(HttpClient httpClient, IConfiguration configuration, ILogger<CaptchaService> logger)
    {
        _httpClient = httpClient;
        _secretKey = configuration[MagicStrings.ConfigKeys.RecaptchaSecretKey] 
            ?? throw new ArgumentNullException(MagicStrings.ConfigKeys.RecaptchaSecretKey);
        _logger = logger;
    }

    public async Task<Result<bool>> VerifyRecaptchaAsync(string token, string email)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.RecaptchaVerificationStarted, email);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning(MagicStrings.ErrorMessages.RecaptchaTokenRequired);
                return Result.Failure<bool>(
                    Error.Failure(
                        MagicStrings.ErrorCodes.RecaptchaFailed,
                        MagicStrings.ErrorMessages.RecaptchaTokenRequired
                    )
                );
            }

            var response = await _httpClient.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"secret", _secretKey},
                    {"response", token}
                })
            );

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RecaptchaResponse>(jsonString);

            if (result?.Success == true)
            {
                _logger.LogInformation(MagicStrings.LogMessages.RecaptchaVerificationCompleted, email);
                return Result.Success(true);
            }

            var errorDetails = string.Join(", ", result?.ErrorCodes ?? Array.Empty<string>());
            _logger.LogWarning(MagicStrings.LogMessages.RecaptchaVerificationFailed, email, errorDetails);
            
            return Result.Failure<bool>(
                Error.Unauthorized(
                    MagicStrings.ErrorCodes.RecaptchaFailed,
                    MagicStrings.ErrorMessages.RecaptchaVerificationFailed
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.RecaptchaVerificationFailed, email, ex.Message);
            return Result.Failure<bool>(
                Error.Failure(
                    MagicStrings.ErrorCodes.RecaptchaFailed,
                    MagicStrings.ErrorMessages.RecaptchaVerificationFailed
                )
            );
        }
    }

    private class RecaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        
        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; } = Array.Empty<string>();
    }
}
