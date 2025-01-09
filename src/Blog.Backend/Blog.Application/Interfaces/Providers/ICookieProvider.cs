using Microsoft.AspNetCore.Http;

namespace Blog.Application.Interfaces.Providers;

public interface ICookieProvider
{
    void AddTokens(HttpResponse response, string access, string refresh);
}