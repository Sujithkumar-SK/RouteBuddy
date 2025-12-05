using System.Security.Claims;
using Kanini.RouteBuddy.Application.Dto.Auth;
using Kanini.RouteBuddy.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.RouteBuddy.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register-with-otp")]
    public async Task<IActionResult> RegisterWithOtp([FromBody] RegisterWithOtpRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterWithOtpAsync(request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RegisterWithOtp");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("resend-registration-otp")]
    public async Task<IActionResult> ResendRegistrationOtp([FromBody] ResendOtpRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResendRegistrationOtpAsync(request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ResendRegistrationOtp");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("verify-registration-otp")]
    public async Task<IActionResult> VerifyRegistrationOtp([FromBody] VerifyRegistrationOtpRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.VerifyRegistrationOtpAsync(request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in VerifyRegistrationOtp");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            SetTokenCookies(result.Value.AccessToken, result.Value.RefreshToken);

            return Ok(new
            {
                result.Value.UserId,
                result.Value.Email,
                result.Value.Role,
                result.Value.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Login");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { error = "Refresh token not found" });

            var result = await _authService.RefreshTokenAsync(new RefreshTokenRequestDto { RefreshToken = refreshToken });
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            SetTokenCookies(result.Value.AccessToken, result.Value.RefreshToken);

            return Ok(new { message = "Token refreshed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RefreshToken");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest(new { error = "Invalid user token" });

            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var result = await _authService.LogoutAsync(refreshToken, userId);
                if (result.IsFailure)
                    return BadRequest(new { error = result.Error });
            }

            ClearTokenCookies();
            return Ok(new { message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Logout");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest(new { error = "Invalid user token" });

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ChangePassword");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForgotPassword");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("verify-forgot-password-otp")]
    public async Task<IActionResult> VerifyForgotPasswordOtp([FromBody] VerifyForgotPasswordOtpRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.VerifyForgotPasswordOtpAsync(request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "OTP verified successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in VerifyForgotPasswordOtp");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ResetPassword");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("complete-customer-profile/{userId}")]
    public async Task<IActionResult> CompleteCustomerProfile(int userId, [FromBody] CompleteCustomerProfileDto request)
    {
        try
        {
            var result = await _authService.CompleteCustomerProfileAsync(userId, request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Customer profile completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CompleteCustomerProfile");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpPost("complete-vendor-profile/{userId}")]
    public async Task<IActionResult> CompleteVendorProfile(int userId, [FromForm] CompleteVendorProfileDto request)
    {
        try
        {
            var result = await _authService.CompleteVendorProfileAsync(userId, request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Vendor profile submitted for approval" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CompleteVendorProfile");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    private void SetTokenCookies(string accessToken, string refreshToken)
    {
        Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Set to false for development (HTTP)
            SameSite = SameSiteMode.Lax, // Changed to Lax for cross-origin requests
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        });

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Set to false for development (HTTP)
            SameSite = SameSiteMode.Lax, // Changed to Lax for cross-origin requests
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private void ClearTokenCookies()
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");
    }
}
