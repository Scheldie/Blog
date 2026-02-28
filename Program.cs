using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Security.Claims;
using Blog.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder();

var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BlogDbContext>
                            (optionsAction: options =>
                                options.UseLazyLoadingProxies()
                                .UseNpgsql((builder.Configuration
                                .GetConnectionString("DefaultConnection"))));


var jwtOptions = builder.Configuration.GetSection("JwtConfig").Get<JwtOptions>();


builder.Services.AddAuthorization();

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<LikeService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddSingleton(new AuthOptions
{
    LockoutMaxFailedAccessAttempts = 5,
    LockoutDuration = TimeSpan.FromMinutes(15),
    CookieLifetime = TimeSpan.FromDays(7)
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthorizationService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = ".Blog.Auth";

        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

var services = builder.Services;




if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.SameAsRequest
});

app.UseAuthentication();


app.UseAuthorization();
app.UseMiddleware<RequireUserMiddleware>();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true) {
        var path = context.Request.Path.Value?.ToLower();
        
        if (path == "/login")
        {
            var userName = context.User.FindFirstValue(ClaimTypes.Name);
            context.Response.Redirect($"/Profile/User?name={userName}");
            return;
        }
    }

    await next();
});




app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogDebug("Request path: {Path}", context.Request.Path);
    await next();
});

app.Use(async (context, next) => {
    try
    {
        await next();
    }
    catch (System.IO.IOException ex) when
        (ex.InnerException is System.Net.Sockets.SocketException)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Socket exception handled: {Message}", ex.Message);
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsync("Service temporarily unavailable");
    }
});

app.Run();

public class AuthOptions
{
    public int LockoutMaxFailedAccessAttempts { get; set; } = 5;
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan CookieLifetime { get; set; } = TimeSpan.FromDays(7);
}

