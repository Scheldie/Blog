using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Blog.Entities;
using Blog.Data.Repositories;
using Blog.Data.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blog.Models.Post;
using Microsoft.Extensions.Hosting;

namespace Blog.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IPostImageRepository _postImageRepository;
        private readonly IPostRepository _postRepository;

        public ProfileController(BlogDbContext context, IWebHostEnvironment env, IUserRepository userRepository)
        {
            _context = context;
            _env = env;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Users()
        {
            if (!User.Identity.IsAuthenticated)
                return Challenge(); // Вернет 401
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Login", "Account"); 
            }

            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.IsActive = true;
            user.LastActiveAt = DateTime.UtcNow;
            var posts = await _context.Posts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var model = new ProfileModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Bio = user.Bio,
                AvatarPath = user.AvatarPath,
                IsCurrentUser = true,
                Posts = posts.Select(p => new PostModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Images = p.Images,
                    CreatedAt = p.CreatedAt
                }).ToList()
            };

            return View(model);
        }

        // POST: Profile/EditProfile
        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Bio = model.Bio;

            // Обработка загрузки аватара
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AvatarFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AvatarFile.CopyToAsync(fileStream);
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

            await _context.SaveChangesAsync();

            return Ok(new { success = true, avatarPath = user.AvatarPath });
        }

        // POST: Profile/CreatePost
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] PostModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = 0;
            if (!Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId))
            {
                return Unauthorized();
            }
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var post = new Post
            {
                UserId = userId,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow,
                PublishedAt = DateTime.UtcNow,
                ViewCount = 0,
                LikesCount = 0,
                ImagesCount = 0
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Обработка загрузки изображений
            if (model.ImageFiles != null && model.ImageFiles.Count() > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var counter = 0;
                foreach (var file in model.ImageFiles.Take(4)) // Максимум 4 изображения
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        var image = new Image
                        {
                            CreatedAt = DateTime.UtcNow,
                            Path = $"/uploads/posts/{uniqueFileName}",
                            UserId = userId
                        };
                        _context.Images.Add(image);
                        var postImage = new PostImage
                        {
                            PostId = post.Id,
                            ImageId = _imageRepository.GetByPath(image.Path).Id,
                            Order = counter++
                        };

                        _context.PostImages.Add(postImage);
                    }
                }
            }
            post.ImagesCount = _context.PostImages.Count(p => p.PostId == post.Id);
            _context.Posts.Update(post);

            await _context.SaveChangesAsync();

            return Ok(new { success = true, postId = post.Id });
        }

        // POST: Profile/EditPost
        [HttpPost]
        public async Task<IActionResult> EditPost([FromForm] PostEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = 0;
            if (!Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId))
            {
                return Unauthorized();
            }
            var post = await _context.Posts
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == model.PostId && p.UserId == userId);

            if (post == null)
            {
                return NotFound();
            }

            post.Description = model.Description;

            // Обработка новых изображений
            if (model.NewImageFiles != null && model.NewImageFiles.Count > 0)
            {
                // Удаляем старые изображения, если нужно
                if (model.DeleteExistingImages)
                {
                    foreach (var image in post.Images)
                    {
                        
                        var filePath = Path.Combine(_env.WebRootPath, image.Path.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        _context.PostImages.Remove(_postImageRepository.GetByImageId(image.Id));
                    }
                }

                // Добавляем новые изображения
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var counter = post.Images.Count();
                foreach (var file in model.NewImageFiles.Take(4 - post.Images.Count())) // Максимум 4 изображения
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        var image = new Image
                        {
                            CreatedAt = DateTime.UtcNow,
                            Path = $"/uploads/posts/{uniqueFileName}",
                            UserId = userId
                        };
                        _context.Images.Add(image);
                        
                        var postImage = new PostImage
                        {
                            PostId = post.Id,
                            ImageId = _imageRepository.GetByPath(image.Path).Id,
                            Order = counter++
                        };

                        _context.PostImages.Add(postImage);
                    }
                }
            }
            await _context.SaveChangesAsync();
            post.ImagesCount = _context.PostImages.Count(p => p.PostId == post.Id);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // POST: Profile/DeletePost
        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var userId = 0;
            if (!Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId))
            {
                return Unauthorized();
            }
            var post = await _context.Posts
                .Include(p => p.Images)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);

            if (post == null)
            {
                return NotFound();
            }

            // Удаляем связанные изображения с диска
            foreach (var image in post.Images)
            {
                var filePath = Path.Combine(_env.WebRootPath, image.Path.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // POST: Profile/ToggleLike
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = 0;
            if (!Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId))
            {
                return Unauthorized();
            }
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.EntityId == postId 
                    && l.UserId == userId && l.LikeType == Entities.Enums.LikeType.Post);

            if (like == null)
            {
                // Добавляем лайк
                _context.Likes.Add(new Like
                {
                    EntityId = postId,
                    UserId = userId,
                    LikeType = Entities.Enums.LikeType.Post,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                // Удаляем лайк
                _context.Likes.Remove(like);
            }

            await _context.SaveChangesAsync();

            // Получаем обновленное количество лайков
            var likesCount = await _context.Likes.CountAsync(l => l.EntityId == postId 
                && l.LikeType == Entities.Enums.LikeType.Post);

            return Ok(new { success = true, isLiked = like == null, likesCount });
        }
    }
} 
