using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Microsoft.AspNetCore.Authorization;
using Blog.Entities;
using Blog.Data.Interfaces;
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
        private readonly IUserRepository _userRepository;

        public ProfileController(BlogDbContext context, IWebHostEnvironment env, 
            IUserRepository userRepository)
        {
            _context = context;
            _env = env;
            _userRepository = userRepository;
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);
            user.IsActive = false;
            _context.SaveChanges();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Feed()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);
            // Получаем посты от всех пользователей (или только от подписок, если у вас есть система подписок)
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Post_Images)
                    .ThenInclude(pi => pi.Image)
                .Include(p => p.Post_Likes)
                    .ThenInclude(pl => pl.Like)
                        .ThenInclude(l => l.User)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var model = posts.Select(post => new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                PostImages = post.Post_Images,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                ImagesCount = post.ImagesCount,
                ViewCount = post.ViewCount,
                Comments = post.Comments,
                PostLikes = post.Post_Likes,
                IsLiked = post.Post_Likes.Any(pl => pl.Like.User.Id == userId),
                User = post.Author,
                WatcherId = userId,
                IsCurrentUser = post.Author.Id == userId
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Users(int id = 0, string? username = null)
        {
            // Если id не указан (равен 0) - показываем профиль текущего пользователя
            if (id == 0)
            {
                if (!User.Identity.IsAuthenticated)
                    return Challenge();

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                {
                    return RedirectToAction("Login", "Authorization");
                }

                var currentUser = _userRepository.GetById(currentUserId);
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
                user = _userRepository.GetByIdWithUsername(id, username);
                if (user == null)
                {
                    // Если пользователь с такой парой id+username не найден
                    return NotFound();
                }
            }
            else
            {
                // Если username не указан, ищем только по ID
                user = _userRepository.GetById(id);
                if (user == null) return NotFound();
            }

            bool isCurrentUser = User.Identity.IsAuthenticated &&
                               User.FindFirst(ClaimTypes.NameIdentifier)?.Value == id.ToString();

            return await GetUserProfile(user, isCurrentUser);
        }

        private async Task<IActionResult> GetUserProfile(User user, bool isCurrentUser)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var posts = await _context.Posts
                .Include(p => p.Post_Images)
                .ThenInclude(pi => pi.Image)
                .Include(pl => pl.Post_Likes)
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
                WatcherId = userId,
                IsActive = user.IsActive,
                Posts = posts.Select(post => new PostModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Description = post.Description,
                    PostImages = post.Post_Images,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    ImagesCount = post.ImagesCount,
                    ViewCount = post.ViewCount,
                    Comments = post.Comments,
                    PostLikes = post.Post_Likes,
                    IsLiked = post.Post_Likes.Any(p=>p.Like.UserId == user.Id),
                }).ToList()
            };

            return View(model);
        }

        // POST: Profile/EditProfile
        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Bio = model.Bio;

            // Обработка загрузки аватара
            if (model.Avatar != null && model.Avatar.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Avatar.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Avatar.CopyToAsync(fileStream);
                }

                // Удаляем старый аватар, если он существует
                if (!string.IsNullOrEmpty(user.AvatarPath))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, user.AvatarPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                user.AvatarPath = $"/uploads/avatars/{uniqueFileName}";
            }
            _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, avatarPath = user.AvatarPath });
        }
        

    }
} 
