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
                Post_Images = post.Post_Images,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                ImagesCount = post.ImagesCount,
                ViewCount = post.ViewCount,
                Comments = post.Comments,
                Post_Likes = post.Post_Likes,
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

            var model = new ProfileRequest
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
                    Post_Images = post.Post_Images,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    ImagesCount = post.ImagesCount,
                    ViewCount = post.ViewCount,
                    Comments = post.Comments,
                    Post_Likes = post.Post_Likes,
                    IsLiked = post.Post_Likes.Any(p=>p.Like.UserId == user.Id),
                }).ToList()
            };

            return View(model);
        }

        // POST: Profile/EditProfile
        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileRequest model)
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
        


        

        // POST: Profile/ToggleLike
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int postId, [FromQuery] bool isComment = false)
        {
            try
            {
                Console.WriteLine($"ToggleLike called for postId: {postId}, isComment: {isComment}");


                var entityExists = isComment
                    ? await _context.Comments.AnyAsync(c => c.Id == postId)
                    : await _context.Posts.AnyAsync(p => p.Id == postId);
                if (!entityExists)
                    return NotFound(new { error = "Post/Comment not found" });
                Console.WriteLine($"Received toggle like for post {postId}");

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"User ID: {userId}");

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                {
                    Console.WriteLine("Unauthorized request");
                    return Json(new { error = "Unauthorized" });
                }

                var likeType = isComment ? LikeType.Comment : LikeType.Post;

                var like = await _context.Likes
                    .FirstOrDefaultAsync(l => l.EntityId == postId
                        && l.UserId == userIdInt
                        && l.LikeType == likeType);

                if (like == null)
                {
                    var likeDb = new Like
                    {
                        EntityId = postId,
                        UserId = userIdInt,
                        LikeType = likeType,
                        CreatedAt = DateTime.UtcNow,
                        User = _userRepository.GetById(userIdInt)
                    };
                    _context.Likes.Add(likeDb);
                    _context.SaveChanges();
                    if (likeType == LikeType.Comment)
                    {
                        _context.Comment_Likes.Add(new Comment_Like
                        {
                            CommentId = postId,
                            PostId = _context.Comments.FirstOrDefaultAsync(c=>c.Id==postId).Result.PostId,
                            LikeId = likeDb.Id,
                            Like = likeDb
                        });
                    }
                    else
                    {
                        _context.Post_Likes.Add(new Post_Like
                        {
                            PostId = postId,
                            LikeId = likeDb.Id,
                            Like = likeDb
                        });
                    }
                }
                else
                {
                    _context.Likes.Remove(like);
                }

                await _context.SaveChangesAsync();

                var likesCount = await _context.Likes
                    .CountAsync(l => l.EntityId == postId && l.LikeType == likeType);

                return Ok(new
                {
                    success = true,
                    isLiked = like == null,
                    likesCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling like");
                return StatusCode(500, new { error = "Failed to toggle like" });
            }
        }
        
        
        
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string query, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return BadRequest("Слишком короткий запрос");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _context.Users.FindAsync(int.Parse(currentUserId));

            // Ищем пользователей, чьи имена содержат запрос (регистронезависимо)
            var usersQuery = _context.Users
                .Where(u => u.UserName.ToLower().Contains(query.ToLower()) && u.Id != currentUser.Id)
                .Select(u => new UserSearchResultModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    AvatarPath = u.AvatarPath,
                })
                .OrderBy(u => u.UserName);

            if (limit > 0)
            {
                usersQuery = (IOrderedQueryable<UserSearchResultModel>)usersQuery.Take(limit);
            }

            var users = await usersQuery.ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return RedirectToAction("Users");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _context.Users.FindAsync(int.Parse(currentUserId));

            var users = await _context.Users
                .Where(u => u.UserName.ToLower().Contains(query.ToLower()) && u.Id != currentUser.Id)
                .Select(u => new UserSearchResultModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    AvatarPath = u.AvatarPath,
                })
                .OrderBy(u => u.UserName)
                .ToListAsync();


            ViewBag.SearchQuery = query;
            return View(users);
        }

    }
} 
