using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Entities;
using Blog.Migrations;
using Blog.Models;
using Blog.Models.Request;

namespace Blog.Services;

public class PostService(
    BlogDbContext context,
    ILogger<PostService> logger,
    IFileService fileService,
    IWebHostEnvironment env)
{
    
    public async Task<List<PostModel>> GetFeedPosts(int userId, int page, int pageSize)
    {
        var posts = await context.Posts
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select( 
                p => 
                    new PostModel { 
                        Id = p.Id, Title = p.Title, Description = p.Description, 
                        CreatedAt = p.CreatedAt, UpdatedAt = p.UpdatedAt, 
                        UserId = p.UserId, UserName = p.Author.UserName, 
                        UserAvatar = p.Author.AvatarPath, 
                        Images = p.PostImages.OrderBy(i => i.Order)
                            .Select(i => i.Image.Path).ToList(), 
                        ImagesCount = p.ImagesCount, 
                        LikesCount = p.LikesCount, 
                        CommentsCount = p.CommentsCount, 
                        ViewCount = p.ViewCount, 
                        IsLiked = p.PostLikes.Any(l => l.Like.UserId == userId), 
                        IsCurrentUser = p.UserId == userId })
            .ToListAsync();

        return posts;
    }


    public async Task<Post?> CreatePostAsync(int userId, PostCreateModel model)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;
        var post = new Post()
        {
            Title = model.Title,
            Description = model.Description,
            UserId = userId,
            Author = user,
            PostImages = new List<Post_Image>(),
            Comments = new List<Comment>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await context.Posts.AddAsync(post);
        var savedImages = new List<Image>();
        if (model.ImageFiles != null)
        {
            foreach (var file in model.ImageFiles)
            {
                if (file.Length == 0) continue;
                var imagePath = await fileService.SaveFileAsync(file);
                if (string.IsNullOrEmpty(imagePath)) continue;
                var image = new Image(imagePath, DateTime.UtcNow, userId);
                savedImages.Add(image);
            }

            await context.Images.AddRangeAsync(savedImages);
            var postImages = new List<Post_Image>();
            for (int i = 0; i < savedImages.Count; i++)
            {
                postImages.Add(new Post_Image { Image = savedImages[i], Post = post, Order = i });
            }

            await context.Post_Images.AddRangeAsync(postImages);
            post.ImagesCount = savedImages.Count;
            post.PostImages = postImages;
        }

        await context.SaveChangesAsync();
        return post;
    }

    public async ValueTask<bool> EditPostAsync(int userId, PostEditModel model)
    {
        var post = await context.Posts
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
            var fullPath = Path.Combine(env.WebRootPath, pi.Image.Path.TrimStart('/'));
            await fileService.DeleteFileAsync(fullPath);

            context.Post_Images.Remove(pi);
            context.Images.Remove(pi.Image);
            post.PostImages.Remove(pi);
        }

        if (model.NewImageFiles?.Any() == true)
        {
            var availableSlots = 4 - post.PostImages.Count;
            int order = post.PostImages.Count;


            foreach (var file in model.NewImageFiles.Take(availableSlots))
            {
                if (file.Length == 0) continue;

                var imagePath = await fileService.SaveFileAsync(file);
                if (string.IsNullOrEmpty(imagePath)) continue;

                var image = new Image(imagePath, DateTime.UtcNow, userId);
                context.Images.Add(image);

                var postImage = new Post_Image
                {
                    Post = post,
                    Image = image,
                    Order = order++
                };
                await context.Post_Images.AddAsync(postImage);

                post.PostImages.Add(postImage);
            }
        }

        post.ImagesCount = post.PostImages.Count;
        await context.SaveChangesAsync();

        return true;
    }


    public async ValueTask<bool> DeletePostAsync(int userId, int postId)
    {
        var post = await context.Posts
            .Include(p => p.PostImages)
            .ThenInclude(pi => pi.Image)
            .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);

        if (post == null) return false;

        foreach (var postImage in post.PostImages)
        {
            var filePath = Path.Combine(env.WebRootPath, postImage.Image.Path.TrimStart('/'));
            await fileService.DeleteFileAsync(filePath);
        }

        await context.Posts.Where(p => p.Id == postId).ExecuteDeleteAsync();
        await context.SaveChangesAsync();
        return true;
    }


    public async Task<PostModel?> GetPostByIdAsync(int userId, int postId)
    {
        return await context.Posts
            .AsNoTracking()
            .Where(p => p.Id == postId)
            .Select(p => PostMapper.ToModel(p,userId))
            .FirstOrDefaultAsync();
    }


    public async Task<List<PostModel>> GetPostsPageAsync(
        int userId,
        string userName,
        int page,
        int pageSize)
    {
        var posts = await context.Posts
            .AsNoTracking()
            .Where(p => p.Author.UserName == userName)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select( 
                p => 
                    new PostModel { 
                        Id = p.Id, Title = p.Title, Description = p.Description, 
                        CreatedAt = p.CreatedAt, UpdatedAt = p.UpdatedAt, 
                        UserId = p.UserId, UserName = p.Author.UserName, 
                        UserAvatar = p.Author.AvatarPath, 
                        Images = p.PostImages.OrderBy(i => i.Order)
                            .Select(i => i.Image.Path).ToList(), 
                        ImagesCount = p.ImagesCount, 
                        LikesCount = p.LikesCount, 
                        CommentsCount = p.CommentsCount, 
                        ViewCount = p.ViewCount, 
                        IsLiked = p.PostLikes.Any(l => l.Like.UserId == userId), 
                        IsCurrentUser = p.UserId == userId })
            .ToListAsync();

        if (posts.Count == 0) return new List<PostModel>();
        return posts;
    }
}