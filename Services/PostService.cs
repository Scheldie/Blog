using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Entities;
using Blog.Models;
using Blog.Models.Request;

namespace Blog.Services;

public class PostService
{
    private readonly ILogger<PostService> _logger;
    private readonly BlogDbContext _context;
    private readonly IWebHostEnvironment _env;
    private IFileService _fileService;


    public PostService(BlogDbContext context,
        ILogger<PostService> logger, IFileService fileService,
        IWebHostEnvironment env)
    {
        _context = context;
        _logger = logger;
        _fileService = fileService;
        _env = env;
    }

    public async Task<Post?> CreatePost(int userId, PostCreateRequest request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;
        var post = new Post()
        {
            Title = request.Title,
            Description = request.Description,
            UserId = userId,
            Author = user,
            PostImages = new List<Post_Image>(),
            Comments = new List<Comment>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        try
        {
            await _context.Posts.AddAsync(post);
            var savedImages = new List<Image>();
            if (request.ImageFiles != null)
            {
                foreach (var file in request.ImageFiles)
                {
                    if (file.Length == 0) continue;
                    var imagePath = await _fileService.SaveFileAsync(file);
                    if (string.IsNullOrEmpty(imagePath)) continue;
                    var image = new Image(imagePath, DateTime.UtcNow, userId);
                    savedImages.Add(image);
                }

                await _context.Images.AddRangeAsync(savedImages);
                var postImages = new List<Post_Image>();
                for (int i = 0; i < savedImages.Count; i++)
                {
                    postImages.Add(new Post_Image { Image = savedImages[i], Post = post, Order = i });
                }

                await _context.Post_Images.AddRangeAsync(postImages);
                post.ImagesCount = savedImages.Count;
                post.PostImages = postImages;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return post;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Post creation failed");
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async ValueTask<bool> EditPost(int userId, PostEditRequest model)
    {
        var post = await _context.Posts
            .Include(p => p.PostImages)
            .ThenInclude(pi => pi.Image)
            .FirstOrDefaultAsync(p => p.Id == model.Id && p.UserId == userId);

        if (post == null) return false;

        post.Title = model.Title;
        post.Description = model.Description;
        post.UpdatedAt = DateTime.UtcNow;

        var keepPaths = model.ExistingImagePaths ?? new List<string>();
        var imagesToDelete = post.PostImages
            .Where(pi => !keepPaths.Contains(pi.Image.Path))
            .ToList();

        foreach (var pi in imagesToDelete)
        {
            var fullPath = Path.Combine(_env.WebRootPath, pi.Image.Path.TrimStart('/'));
            await _fileService.DeleteFileAsync(fullPath);

            _context.Post_Images.Remove(pi);
            _context.Images.Remove(pi.Image);
            post.PostImages.Remove(pi);
        }

        if (model.NewImageFiles?.Any() == true)
        {
            var availableSlots = 4 - post.PostImages.Count;
            int order = post.PostImages.Count;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in model.NewImageFiles.Take(availableSlots))
            {
                if (file.Length == 0) continue;

                var imagePath = await _fileService.SaveFileAsync(file);
                if (string.IsNullOrEmpty(imagePath)) continue;

                var image = new Image(imagePath, DateTime.UtcNow, userId);
                _context.Images.Add(image);

                var postImage = new Post_Image
                {
                    Post = post,
                    Image = image,
                    Order = order++
                };
                await _context.Post_Images.AddAsync(postImage);

                post.PostImages.Add(postImage);
            }
        }

        post.ImagesCount = post.PostImages.Count;
        await _context.SaveChangesAsync();

        return true;
    }


    public async ValueTask<bool> DeletePost(int userId, int postId)
    {
        var post = await _context.Posts
            .Include(p => p.PostImages)
            .ThenInclude(pi => pi.Image)
            .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);

        if (post == null) return false;

        foreach (var postImage in post.PostImages)
        {
            var filePath = Path.Combine(_env.WebRootPath, postImage.Image.Path.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        await _context.Posts.Where(p => p.Id == postId).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<List<PostModel>> GetPostsPageAsync(int currentUserId,int userId, int page, int pageSize)
    {
        
        var posts = await _context.Posts
            .Where(p => p.UserId == userId)
            .Include(p => p.PostImages)
            .Include(p => p.PostLikes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return posts.Select(p => p.ToModel(currentUserId)).ToList();

    }
    public async Task<PostModel> GetPostById(int userId, int postId)
    {
        var post = await _context.Posts 
            .Include(p => p.PostImages) 
            .Include(p => p.PostLikes) 
            .Include(p => p.Comments) 
            .FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null) return new PostModel(){Title = "", Description = ""};
        return post.ToModel(userId);

    }
}