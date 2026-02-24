using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Blog.Services;
using Blog.Entities;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly BlogDbContext _dbContext;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(ILogger<AuthorizationController> logger, BlogDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;

        }
        [HttpGet]
        [Route("signUp")]
        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult Forgotten()
        {
            return View();
        }
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            
            return View();
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == loginModel.Email);
                var hasher = new PasswordHasherService();
                if (user == null || !hasher.Verify(loginModel.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Invalid credentials");
                    return View(loginModel);
                }

                user.LastLoginAt = DateTime.UtcNow;
                user.IsActive = true;
                _dbContext.Update(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "User") 
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7)
                    });

                return RedirectToAction("Users", "Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return StatusCode(500, "Internal server error");
            }
        }

        
        [HttpPost]
        [Route("signUp")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(
            SignUpModel signUpModel
            )
        {
            if (ModelState.IsValid)
            {
                if (await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == signUpModel.Email) != null)
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    return View(signUpModel);
                }
                var hasher = new PasswordHasherService();
                var passwordHash = hasher.Generate(signUpModel.Password);
                var user = new User
                {
                    Email = signUpModel.Email,
                    PasswordHash = passwordHash,
                    UserName = signUpModel.UserName,
                    AvatarPath = "/default-avatar.png",
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow

                };
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                await Authenticate(signUpModel.Email);

                return RedirectToAction("Index", "Login");
            }
            return View(signUpModel);
        }
        private async Task Authenticate(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation($"Authenticating user: {email}");
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(12),
                    AllowRefresh = true
                });
            
            _logger.LogInformation("Authentication completed");
        }
    }

}
