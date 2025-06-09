using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Blog.Data;
using Blog.Data.Interfaces;
using Blog.Data.Repositories;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Security.Claims;

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
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Authorization/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.SlidingExpiration = true;   
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IPostImageRepository, PostImageRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IFileService, FileService>();

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

app.MapControllerRoute(
    name: "userProfile",
    pattern: "user/{id}-{username}",
    defaults: new { controller = "Profile", action = "Users" });

app.MapControllerRoute(
    name: "userProfileById",
    pattern: "user/{id}",
    defaults: new { controller = "Profile", action = "Users", username = (string?)null });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/Profile/Users", context =>
{
    context.Response.Redirect($"/user/{context.User.FindFirstValue(ClaimTypes.NameIdentifier)}");
    return Task.CompletedTask;
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
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";
}
