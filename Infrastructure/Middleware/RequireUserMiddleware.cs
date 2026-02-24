using System.Security.Claims;

namespace Blog.Infrastructure.Middleware;

public class RequireUserMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var claim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(claim, out var userId) || userId <= 0)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }
}