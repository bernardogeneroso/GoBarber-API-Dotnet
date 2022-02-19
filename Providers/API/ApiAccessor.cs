using Microsoft.AspNetCore.Http;
using Services.Interfaces;

namespace Providers.API;

public class ApiAccessor : IApiAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ApiAccessor(IHttpContextAccessor httpContextAccessor)
    {
            this._httpContextAccessor = httpContextAccessor;
    }

    public string GetOrigin()
    {
        var scheme = _httpContextAccessor?.HttpContext?.Request.Scheme ?? null;
        var host = _httpContextAccessor?.HttpContext?.Request.Host ?? null;

        if (scheme is null || host is null) return null;

        return $"{scheme}://{host}";
    }

    public string GetRoutePath()
    {
        string path = _httpContextAccessor?.HttpContext?.Request.Path;

        if (path is null) return null;

        return path.Replace("/api/", "");
    }

    public void SetCookie(string cookieName, string cookieValie, CookieOptions cookieOptions = null)
    {
        if (cookieOptions is null) cookieOptions = new CookieOptions();

        _httpContextAccessor?.HttpContext?.Response.Cookies.Append(cookieName, cookieValie, cookieOptions);
    }
}
