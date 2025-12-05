using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services;

public interface ICaptchaService
{
    Task<Result<bool>> VerifyRecaptchaAsync(string token, string email);
}
