using System.Security.Claims;
using Blog.Data;
using Blog.Data.Interfaces;
using Blog.Entities;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class PostController : Controller
{
    private readonly BlogDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ProfileController> _logger;
    private readonly IFileService _fileService;

    public PostController(BlogDbContext context, IWebHostEnvironment env, 
        ILogger<ProfileController> logger, IFileService fileService)
    {
        _context = context;
        _env = env;
        _logger = logger;
        _fileService = fileService;
    }
    // GET
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([FromForm] PostModel model)
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
                    ImagesCount = 0,
                    Comments = new List<Comment>(),
                    Post_Likes = new List<Post_Like>()
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync(); // Сохраняем, чтобы получить ID

                // 2. Обрабатываем изображения
                if (model.ImageFiles == null || model.ImageFiles.Count() == 0)
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
        public async Task<IActionResult> EditPost([FromForm] PostModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Editing post {model.Id}. DeleteExistingImages: {model.DeleteExistingImages}");
            _logger.LogInformation($"Deleted files paths: {string.Join(", ", model.DeletedFilesPaths ?? new List<string>())}");

            // Удаляем проверку ModelState для NewImageFiles
            ModelState.Remove("NewImageFiles");
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
                .ThenInclude(pi => pi.Image)
                .FirstOrDefaultAsync(p => p.Id == model.Id && p.UserId == userId);

            if (post == null)
            {
                return NotFound();
            }

            post.Title = model.Title;
            post.Description = model.Description;

            // Обработка новых изображений
            if (model.NewImageFiles != null && model.NewImageFiles.Any())
            {
                // Удаляем старые изображения, если нужно
                if (model.DeleteExistingImages)
                {
                    foreach (var postImage in post.Post_Images.ToList())
                    {
                        var filePath = Path.Combine(_env.WebRootPath, postImage.Image.Path.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        _context.Post_Images.Remove(postImage);
                        _context.Images.Remove(postImage.Image);
                    }
                }

                // Добавляем новые изображения
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var counter = post.Post_Images.Count();
                foreach (var file in model.NewImageFiles.Take(4 - post.Post_Images.Count()))
                {
                    if (file.Length > 0)
                    {
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
                        await _context.SaveChangesAsync(); // Сохраняем изображение, чтобы получить ID

                        var postImage = new Post_Image
                        {
                            PostId = post.Id,
                            ImageId = image.Id, // Используем ID сохраненного изображения
                            Order = counter++
                        };

                        _context.Post_Images.Add(postImage);
                    }
                }
            }
            // Обработка удаленных изображений
            if (model.DeletedFilesPaths != null && model.DeletedFilesPaths.Any())
            {
                _logger.LogInformation($"Processing {model.DeletedFilesPaths.Count} deleted images");

                foreach (var filePath in model.DeletedFilesPaths)
                {
                    _logger.LogInformation($"Processing deletion for path: {filePath}");

                    var postImage = post.Post_Images
                        .FirstOrDefault(pi => pi.Image.Path == filePath);

                    if (postImage != null)
                    {
                        _logger.LogInformation($"Found image to delete: {postImage.Image.Path}");

                        // Удаляем физический файл
                        var fullPath = Path.Combine(_env.WebRootPath, postImage.Image.Path.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                            _logger.LogInformation($"Deleted file from disk: {fullPath}");
                        }

                        // Удаляем записи из БД
                        _context.Post_Images.Remove(postImage);
                        _context.Images.Remove(postImage.Image);
                        _logger.LogInformation($"Removed image from database: {postImage.Image.Id}");
                    }
                    else
                    {
                        _logger.LogWarning($"Image not found for path: {filePath}");
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
                .Include(pl => pl.Post_Likes)
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
}