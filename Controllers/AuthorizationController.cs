using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Blog.Data.Interfaces;
using Blog.Data.Repositories;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Blog.Entities;
using Blog.Data;
using Blog.Models.Account;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly BlogDbContext dbContext;

        public AuthorizationController(ILogger<AuthorizationController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;

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
                var user = _userRepository.GetByEmail(loginModel.Email);
                var hasher = new PasswordHasher();
                if (user == null || !hasher.Verify(loginModel.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Invalid credentials");
                    return View(loginModel);
                }

                user.LastLoginAt = DateTime.UtcNow;
                user.IsActive = true;
                _userRepository.UpdateEntity(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
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
                if (_userRepository.GetByEmail(signUpModel.Email) != null)
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    return View(signUpModel);
                }
                var hasher = new PasswordHasher();
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
                _userRepository.AddEntity(user);
                _userRepository.Save();
                await Authenticate(signUpModel.Email);

                return RedirectToAction("Index", "Login");
            }
            return View(signUpModel);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
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
