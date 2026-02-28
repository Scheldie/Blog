using Microsoft.AspNetCore.Mvc;
using Blog.Models;

public class AuthorizationController : Controller
{
    private readonly AuthorizationService _authService;
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(AuthorizationService authService, ILogger<AuthorizationController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (success, error) = await _authService.LoginAsync(model);

        if (!success)
        {
            ModelState.AddModelError("", error ?? "Ошибка входа");
            return View(model);
        }

        return RedirectToAction("Users", "Profile");
    }
    
    [HttpGet("signUp")]
    public IActionResult SignUp()
    {
        return View();
    }
    
    [HttpPost("signUp")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(SignUpModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (success, error) = await _authService.RegisterAsync(model);

        if (!success)
        {
            ModelState.AddModelError("", error ?? "Ошибка регистрации");
            return View(model);
        }
        
        return RedirectToAction("Login");
    }
    
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Login");
    }
}
