using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Blog.Data.Intefaces;
using Blog.Data.Repositories;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Blog.Entities;
using Blog.Data;
using Blog.Models;

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
            LogInModel loginModel
            )
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
                return RedirectToAction("LogIn", "Index");
            }
            ModelState.AddModelError("Password", "Please enter correct password.");
            return View(loginModel);
            
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
                    UserName = signUpModel.UserName
                   
                };
                _userRepository.AddEntity(user);
                _userRepository.Save();
                await Authenticate(signUpModel.Email);

                return RedirectToAction("Index", "LogIn");
            }
            return View(signUpModel);
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
