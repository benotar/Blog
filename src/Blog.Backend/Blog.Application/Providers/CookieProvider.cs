using Blog.Application.Configurations;
using Blog.Application.Interfaces.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Blog.Application.Providers;

public class CookieProvider : ICookieProvider
{
    private readonly CookieConfiguration _cookieConfiguration;

    public CookieProvider(IOptions<CookieConfiguration> cookieConfiguration)
    {
        _cookieConfiguration = cookieConfiguration.Value;
    }

    public void AddTokens(HttpResponse response, string access, string refresh)
    {
        var cookieOptions = new CookieOptions
        {
            Secure = false,
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        };

        response.Cookies.Append(_cookieConfiguration.AccessTokenKey, access, cookieOptions);

        response.Cookies.Append(_cookieConfiguration.RefreshTokenKey, refresh, cookieOptions);
    }
}