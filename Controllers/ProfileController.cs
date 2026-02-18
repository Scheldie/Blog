using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Microsoft.AspNetCore.Authorization;
using Blog.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blog.Services;
using Blog.Entities.Enums;
using Microsoft.AspNetCore.Authentication;

using Blog.Models;
using Blog.Models.Request;

namespace Blog.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfileController(BlogDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            int userId;
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId)) return Unauthorized();
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return Unauthorized();
            user.IsActive = false;
            await _context.SaveChangesAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Feed()
        {
            int userId;
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId)) return Unauthorized();
            var user = await _context.Users.FindAsync(userId);
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.PostImages)
                .ThenInclude(pi => pi.Image)
                .Include(p => p.PostLikes)
                    .ThenInclude(pl => pl.Like)
                        .ThenInclude(l => l.User)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var model = posts.Select(post => post.ToModel(userId)).ToList();

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Users(int id = 0, string? username = null)
        {
            // Если id не указан (равен 0) - показываем профиль текущего пользователя
            if (id == 0)
            {

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                {
                    return RedirectToAction("Login", "Authorization");
                }

                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
                if (currentUser == null) return NotFound();

                currentUser.IsActive = true;
                currentUser.LastActiveAt = DateTime.UtcNow;
                _context.SaveChanges();

                return await GetUserProfile(currentUser, true);
            }

            // Просмотр профиля по ID
            User? user;

            // Если указан username, проверяем его соответствие с ID
            if (!string.IsNullOrEmpty(username))
            {
                user = await _context.Users.FirstOrDefaultAsync(u=>u.UserName ==  username && u.Id == id);
                if (user == null)
                {
                    // Если пользователь с такой парой id+username не найден
                    return NotFound();
                }
            }
            else
            {
                // Если username не указан, ищем только по ID
                user = await _context.Users.FirstOrDefaultAsync(u=>u.Id == id);
                if (user == null) return NotFound();
            }

            bool isCurrentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value == id.ToString();

            return await GetUserProfile(user, isCurrentUser);
        }

        private async Task<IActionResult> GetUserProfile(User user, bool isCurrentUser)
        {
            var posts = await _context.Posts
                .Include(p => p.PostImages)
                .ThenInclude(pi => pi.Image)
                .Include(pl => pl.PostLikes)
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            var model = new ProfileModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = isCurrentUser ? user.Email : null, // Показываем email только для своего профиля
                Bio = user.Bio,
                AvatarPath = user.AvatarPath,
                IsCurrentUser = isCurrentUser,
                WatcherId = user.Id,
                IsActive = user.IsActive,
                Posts = posts.Select(post => post.ToModel(user.Id)).ToList()
            };

            return View(model);
        }

        // POST: Profile/EditProfile
        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId;
            if (!Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            user.UserName = model.UserName;
            user.Bio = model.Bio;
            
            if (model.RemoveAvatar)
            {
                if (!string.IsNullOrEmpty(user.AvatarPath))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, user.AvatarPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                user.AvatarPath = null;
            }
            
            if (model.Avatar != null && model.Avatar.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Avatar.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await model.Avatar.CopyToAsync(fileStream);

                // Удаляем старый аватар, если был
                if (!string.IsNullOrEmpty(user.AvatarPath))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, user.AvatarPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                user.AvatarPath = $"/uploads/avatars/{uniqueFileName}";
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, avatarPath = user.AvatarPath });
        }

        

    }
} 
