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
using Blog.Services;

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
        private readonly ILogger<ProfileController> _logger;
        private readonly IFileService _fileService;

        public ProfileController(BlogDbContext context, IWebHostEnvironment env, 
            IUserRepository userRepository, ILogger<ProfileController> logger,
            IPostRepository postRepository, IPostImageRepository postImageRepository,
            IImageRepository imageRepository, IFileService fileService)
        {
            _context = context;
            _env = env;
            _userRepository = userRepository;
            _logger = logger;
            _postRepository = postRepository;
            _imageRepository = imageRepository;
            _postImageRepository = postImageRepository;
            _fileService = fileService;
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
                    Post_Images = p.Post_Images,
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([FromForm] PostCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                // 1. Создаем пост
                var post = new Post
                {
                    UserId = userId,
                    Title = model.Title,
                    Description = model.Description,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    PublishedAt = DateTime.UtcNow,
                    ViewCount = 0,
                    LikesCount = 0,
                    ImagesCount = 0
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync(); // Сохраняем, чтобы получить ID

                // 2. Обрабатываем изображения
                if (model.ImageFiles == null || model.ImageFiles.Count == 0)
                {
                    return BadRequest("Необходимо загрузить хотя бы одно изображение");
                }

                var savedImages = new List<Image>();
                foreach (var file in model.ImageFiles)
                {
                    if (file == null || file.Length == 0) continue;

                    // Сохраняем файл
                    var imagePath = await _fileService.SaveFileAsync(file);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        continue; // Пропускаем если не удалось сохранить
                    }

                    var image = new Image
                    {
                        Path = imagePath,
                        CreatedAt = DateTime.UtcNow,
                        UserId = userId
                    };

                    _context.Images.Add(image);
                    await _context.SaveChangesAsync(); // Сохраняем каждое изображение

                    savedImages.Add(image);
                }

                // 3. Связываем изображения с постом
                for (int i = 0; i < savedImages.Count; i++)
                {
                    _context.Post_Images.Add(new Post_Image
                    {
                        PostId = post.Id,
                        ImageId = savedImages[i].Id,
                        Order = i
                    });
                }

                // 4. Обновляем счетчик изображений
                post.ImagesCount = savedImages.Count;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    postId = post.Id,
                    imagesCount = savedImages.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании поста");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Внутренняя ошибка сервера",
                    error = ex.Message
                });
            }
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
                .Include(p => p.Post_Images)
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
                    foreach (var postImage in post.Post_Images)
                    {
                        
                        var filePath = Path.Combine(_env.WebRootPath, postImage.Image.Path.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        _context.Post_Images.Remove(_postImageRepository.GetByImageId(postImage.Image.Id));
                    }
                }

                // Добавляем новые изображения
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var counter = post.Post_Images.Count();
                foreach (var file in model.NewImageFiles.Take(4 - post.Post_Images.Count())) // Максимум 4 изображения
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
                        
                        var postImage = new Post_Image
                        {
                            PostId = post.Id,
                            ImageId = _imageRepository.GetByPath(image.Path).Id,
                            Order = counter++
                        };

                        _context.Post_Images.Add(postImage);
                    }
                }
            }
            await _context.SaveChangesAsync();
            post.ImagesCount = _context.Post_Images.Count(p => p.PostId == post.Id);
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
                .Include(p => p.Post_Images)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);

            if (post == null)
            {
                return NotFound();
            }

            // Удаляем связанные изображения с диска
            foreach (var postImage in post.Post_Images)
            {
                var filePath = Path.Combine(_env.WebRootPath, postImage.Image.Path.TrimStart('/'));
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
