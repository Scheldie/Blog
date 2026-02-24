using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Microsoft.AspNetCore.Authorization;
using Blog.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blog.Services;
using Blog.Entities.Enums;
using Blog.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;

using Blog.Models;
using Blog.Models.Request;

namespace Blog.Controllers
{
    [Authorize]
    public class ProfileController(ProfileService profileService) : Controller
    {
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.GetUserId();
            await profileService.Logout(userId);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Route("/Profile/User")]
        [Authorize]
        public async Task<IActionResult> Users(string? name)
        {
            var userId = User.GetUserId();
            return View(await profileService.GetUser(userId,name));
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileEditRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var userId = User.GetUserId();
            
            if(await profileService.EditProfile(userId, model)) 
                return Ok(new { success = true, avatarPath = model.Avatar });
            return BadRequest(new { success = false, avatarPath = model.Avatar });
        }

        

    }
} 
