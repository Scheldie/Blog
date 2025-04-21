using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Blog.Data.Intefaces;
using Blog.Data.Repositories;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Blog.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(ILogger<AuthorizationController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;

        }
        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult LogIn()
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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Profile", "Profile");
            }
            return View();
        }
        [HttpPost]
        [Route("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LogInModel loginModel,
            IOptions<JwtOptions> jwtOptions,
            HttpContext context)
        {
            var user = _userRepository.GetByEmail(loginModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "User with this email does not exist.");
                return View(loginModel);
            }
            
            var hasher = new PasswordHasher();
            if (hasher.Verify(loginModel.Password, user.PasswordHash))
            {
                await Authenticate(loginModel.Email);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("Password", "Please enter correct password.");
            return View(loginModel);
            JwtProvider jwtProvider = new JwtProvider(jwtOptions);
            var token = jwtProvider.GenerateToken(_userRepository.GetByEmail(loginModel.Email));
            context.Response.Cookies.Append("cookies", token);
        }
        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie",
                                        ClaimsIdentity.DefaultNameClaimType,
                                        ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }

}
