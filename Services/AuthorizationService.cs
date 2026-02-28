using System.Security.Claims;
using Blog.Data;
using Blog.Entities;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class AuthorizationService
{
    private readonly BlogDbContext _dbContext;
    private readonly ILogger<AuthorizationService> _logger;
    private readonly AuthOptions _options;
    private readonly IHttpContextAccessor _http;
    private readonly PasswordHasherService _hasher;
    private readonly IMemoryCache _cache;

    public AuthorizationService(
        BlogDbContext dbContext,
        ILogger<AuthorizationService> logger,
        AuthOptions options,
        IHttpContextAccessor http,
        IMemoryCache cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _options = options;
        _http = http;
        _cache = cache;
        _hasher = new PasswordHasherService();
    }
    
    
    public async Task<(bool Success, string? Error)> RegisterAsync(SignUpModel model)
    {

        var exists = await _dbContext.Users
            .AnyAsync(u => u.Email == model.Email);

        if (exists)
            return (false, "Пользователь с таким email уже существует");

        var user = new User
        {
            Email = model.Email,
            UserName = model.UserName,
            PasswordHash = _hasher.Generate(model.Password),
            AvatarPath = "/default-avatar.png",
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            LastActiveAt = DateTime.UtcNow,
            IsActive = true
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return (true, null);
    }
    
    public async Task<(bool Success, string? Error)> LoginAsync(LoginModel model)
    {

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
            return (false, "Неверный email или пароль");
        
        if (_cache.TryGetValue<(int fails, DateTime? lockoutEnd)>(model.Email,out var entry))
        {
            if (entry.lockoutEnd.HasValue && entry.lockoutEnd > DateTime.UtcNow)
                return (false, "Аккаунт временно заблокирован. Попробуйте позже.");
        }
        
        if (!_hasher.Verify(model.Password, user.PasswordHash))
        {
            var fails = entry.fails + 1;
            DateTime? lockoutEnd = entry.lockoutEnd;

            if (fails >= _options.LockoutMaxFailedAccessAttempts)
            {
                lockoutEnd = DateTime.UtcNow + _options.LockoutDuration;
                fails = 0;
            }

            _cache.Set(model.Email, (fails, lockoutEnd),
                TimeSpan.FromMinutes(30)); // TTL для очистки

            return (false, "Неверный email или пароль");
        }
        
        _cache.Remove(model.Email);

        user.LastLoginAt = DateTime.UtcNow;
        user.LastActiveAt = DateTime.UtcNow;
        user.IsActive = true;

        await _dbContext.SaveChangesAsync();
        await SignInAsync(user);

        return (true, null);
    }
    
    public async Task LogoutAsync()
    {
        if (_http.HttpContext != null)
            await _http.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
    
    private async Task SignInAsync(User user)
    {
        var ctx = _http.HttpContext;
        if (ctx == null) return;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "User")
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await ctx.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow + _options.CookieLifetime,
                AllowRefresh = true
            });
    }
}
