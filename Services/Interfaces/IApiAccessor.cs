using Microsoft.AspNetCore.Http;

namespace Services.Interfaces;

public interface IApiAccessor
{
    string GetOrigin();
    string GetRoutePath();
    void SetCookie(string cookieName, string cookieValie, CookieOptions cookieOptions = null);
}
