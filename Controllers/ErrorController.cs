using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("Error/{code}")]
    public IActionResult ErrorPage(int code)
    {
        if (code == 404)
        {
            _logger.LogWarning("404 Not Found: {Path}", HttpContext.Request.Path);
            return View("NotFound");
        }

        if (code == 403)
        {
            _logger.LogWarning("403 Forbidden: {Path}", HttpContext.Request.Path);
            return View("Forbidden");
        }

        _logger.LogError("Unexpected status code {Code} at {Path}", code, HttpContext.Request.Path);
        return View("Error");
    }

    [Route("Error/500")]
    public IActionResult ServerError()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception != null)
        {
            _logger.LogError(exception, "Unhandled exception at {Path}", HttpContext.Request.Path);
        }

        return View("Error");
    }
}