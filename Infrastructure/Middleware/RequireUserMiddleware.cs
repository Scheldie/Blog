using System.Security.Claims;

namespace Blog.Infrastructure.Middleware;

public class RequireUserMiddleware
{
    private readonly RequestDelegate _next;

    public RequireUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();


        if (path.StartsWith("/login") ||
            path.StartsWith("/signup") ||
            path.StartsWith("/css") ||
            path.StartsWith("/js") ||
            path.StartsWith("/images") ||
            path.StartsWith("/favicon") ||
            path.ToString() == "/")
        {
            await _next(context);
            return;
        }

        var claim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(claim, out var userId) || userId <= 0)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
