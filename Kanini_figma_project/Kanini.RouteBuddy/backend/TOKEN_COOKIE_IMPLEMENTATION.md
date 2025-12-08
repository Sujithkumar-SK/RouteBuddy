# HttpOnly Cookie Implementation for Refresh Tokens

## 🔐 Why httpOnly Cookies?

### Security Benefits:
✅ **XSS Protection** - JavaScript cannot access the cookie
✅ **Automatic sending** - Browser sends cookie with every request
✅ **Industry standard** - Used by Google, Facebook, Amazon

### Trade-offs:
⚠️ **CSRF vulnerability** - Need CSRF protection
⚠️ **Backend changes required** - Must set cookies in response

---

## 🔧 Backend Implementation

### Step 1: Update TokenController.cs

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
{
    try
    {
        var user = await _tokenService.ValidateUserAsync(loginDto.Email, loginDto.Password);
        if (user == null)
            return Unauthorized(new { error = "Invalid email or password" });

        if (!user.IsActive)
            return Unauthorized(new { error = "Account is inactive" });

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenResult = await _tokenService.GenerateRefreshTokenAsync(user.UserId);

        if (refreshTokenResult.IsFailure)
            return StatusCode(500, new { error = "Failed to generate refresh token" });

        // ✅ Set refresh token as httpOnly cookie
        Response.Cookies.Append("refreshToken", refreshTokenResult.Value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // HTTPS only
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        // ❌ Don't send refresh token in response body
        return Ok(new
        {
            accessToken,
            email = user.Email,
            role = user.Role.ToString(),
            userId = user.UserId,
            accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error during login");
        return StatusCode(500, new { error = "An error occurred during login" });
    }
}

[HttpPost("refresh")]
public async Task<IActionResult> RefreshToken()
{
    try
    {
        // ✅ Read refresh token from cookie
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Unauthorized(new { error = "Refresh token not found" });

        var result = await _tokenService.RefreshTokenAsync(refreshToken);
        if (result.IsFailure)
            return Unauthorized(new { error = result.Error.Description });

        // ✅ Set new refresh token as httpOnly cookie
        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(new
        {
            accessToken = result.Value.AccessToken,
            email = result.Value.Email,
            role = result.Value.Role.ToString(),
            userId = result.Value.UserId,
            accessTokenExpiresAt = result.Value.AccessTokenExpiresAt
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error refreshing token");
        return StatusCode(500, new { error = "An error occurred while refreshing token" });
    }
}

[HttpPost("revoke")]
public async Task<IActionResult> RevokeToken()
{
    try
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return BadRequest(new { error = "Refresh token not found" });

        var result = await _tokenService.RevokeRefreshTokenAsync(refreshToken);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Description });

        // ✅ Delete cookie
        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "Token revoked successfully" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error revoking token");
        return StatusCode(500, new { error = "An error occurred while revoking token" });
    }
}
```

### Step 2: Add CORS Configuration in Program.cs

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite dev server
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // ✅ Required for cookies
    });
});

app.UseCors("AllowFrontend");
```

---

## 🎨 Frontend Implementation

### Step 1: Update api.ts

```typescript
import axios from 'axios';
import { API_BASE_URL } from '../utils/constants';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // ✅ Send cookies with requests
});

let accessToken: string | null = null;

export const setAccessToken = (token: string | null) => {
  accessToken = token;
};

export const getAccessToken = () => accessToken;

// Request interceptor
api.interceptors.request.use(
  (config) => {
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for auto-refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        // ✅ No need to send refresh token - it's in cookie
        const response = await axios.post(
          `${API_BASE_URL}/token/refresh`,
          {},
          { withCredentials: true } // ✅ Send cookies
        );

        const { accessToken: newAccessToken } = response.data;
        setAccessToken(newAccessToken);

        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return api(originalRequest);
      } catch (refreshError) {
        setAccessToken(null);
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default api;
```

### Step 2: Update authAPI.ts

```typescript
export const authAPI = {
  login: async (data: LoginRequest) => {
    // ✅ Cookie is set automatically by backend
    const response = await api.post(API_ENDPOINTS.LOGIN, data);
    const { accessToken } = response.data;
    
    setAccessToken(accessToken);
    // ❌ No localStorage.setItem('refreshToken', ...)
    
    return response.data;
  },

  logout: async () => {
    try {
      // ✅ Backend deletes cookie
      await api.post(API_ENDPOINTS.REVOKE_TOKEN);
    } catch (error) {
      console.error('Logout error:', error);
    }
    setAccessToken(null);
  },
};
```

---

## ✅ Benefits of httpOnly Cookies

1. **XSS Protection** - Even if attacker injects script, can't steal token
2. **Automatic Management** - Browser handles cookie storage/sending
3. **Industry Standard** - What production apps use
4. **Better Security** - Refresh token never exposed to JavaScript

---

## ⚠️ CSRF Protection (Optional but Recommended)

Add CSRF token for extra security:

```csharp
// Backend: Generate CSRF token
[HttpPost("login")]
public async Task<IActionResult> Login(...)
{
    var csrfToken = Guid.NewGuid().ToString();
    Response.Cookies.Append("XSRF-TOKEN", csrfToken, new CookieOptions
    {
        HttpOnly = false, // Frontend needs to read this
        Secure = true,
        SameSite = SameSiteMode.Strict
    });
    
    // Return CSRF token in response
    return Ok(new { accessToken, csrfToken });
}

// Frontend: Send CSRF token in headers
api.interceptors.request.use((config) => {
  const csrfToken = document.cookie
    .split('; ')
    .find(row => row.startsWith('XSRF-TOKEN='))
    ?.split('=')[1];
    
  if (csrfToken) {
    config.headers['X-XSRF-TOKEN'] = csrfToken;
  }
  return config;
});
```

---

## 🎯 Recommendation

**For Production:** Use httpOnly cookies (implement above changes)
**For Development:** Current localStorage is fine for testing

**Your Choice:**
- Keep localStorage for now (faster development)
- Implement httpOnly cookies (better security)

Which approach do you want to use?
