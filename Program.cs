using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Blog.Data;
using Blog.Data.Intefaces;
using Blog.Data.Repositories;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using Blog.Data.Intefaces;
using Blog.Data.Repositories;
using Blog.Data;
using Blog.Services;

var modelBuilder = WebApplication.CreateBuilder();

var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>();

modelBuilder.Services.AddControllersWithViews();
modelBuilder.Services.AddDbContext<BlogDbContext>
                            (optionsAction: options =>
                                options.UseLazyLoadingProxies()
                                .UseNpgsql((modelBuilder.Configuration
                                .GetConnectionString("DefaultConnection"))));


var jwtOptions =
    modelBuilder.Configuration.GetSection(JwtOptions.JwtConfig)
        .Get<JwtOptions>();
ApiExtension.AddApiAuthentification(modelBuilder.Services, jwtOptions);

modelBuilder.Services.AddScoped<IUserRepository, UserRepository>();

var app = modelBuilder.Build();

app.UseAuthentication();


var services = modelBuilder.Services;



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//services.AddScoped<IUserRepository, UserRepository>();
//services.AddScoped<UserService>();
//services.AddScoped<IJwtProvider, JwtProvider>();
//services.AddScoped<IPasswordHasher, PasswordHasher>();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";
}
