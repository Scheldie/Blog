using System.Security.Claims;
using Blog.Data;
using Blog.Entities;
using Blog.Entities.Enums;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class LikeController : Controller
{
    private readonly BlogDbContext _context;
    private readonly ILogger<ProfileController> _logger;


    public LikeController(BlogDbContext context, 
        ILogger<ProfileController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> ToggleLike([FromBody] LikeModel model)
    {
            try
            {
                Console.WriteLine($"ToggleLike called for postId: {model.PostId}, isComment: {model.IsComment}");


                var entityExists = model.IsComment
                    ? await _context.Comments.AnyAsync(c => c.Id == model.CommentId)
                    : await _context.Posts.AnyAsync(p => p.Id == model.PostId);
                if (!entityExists)
                    return NotFound(new { error = "Post/Comment not found" });
                Console.WriteLine($"Received toggle like for post {model.PostId}");

                int userId;
                if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId)) return Unauthorized();
                Console.WriteLine($"User ID: {userId}");

                var likeType = model.IsComment ? LikeType.Comment : LikeType.Post;
                int entityId = model.CommentId.HasValue ? model.CommentId.Value : model.PostId;
                var like = await _context.Likes
                    .FirstOrDefaultAsync(l => l.EntityId == entityId
                        && l.UserId == userId
                        && l.LikeType == likeType);

                if (like == null)
                {
                    var likeDb = new Like
                    {
                        EntityId = entityId,
                        UserId = userId,
                        LikeType = likeType,
                        CreatedAt = DateTime.UtcNow,
                        User = (await _context.Users.FirstOrDefaultAsync(u=>u.Id == userId))
                    };
                    await _context.Likes.AddAsync(likeDb);
                    await _context.SaveChangesAsync();
                    if (likeType == LikeType.Comment)
                    {
                        await _context.Comment_Likes.AddAsync(new Comment_Like
                        {
                            CommentId = entityId,
                            PostId = model.PostId,
                            LikeId = likeDb.Id,
                            Like = likeDb
                        });
                    }
                    else
                    {
                        await _context.Post_Likes.AddAsync(new Post_Like()
                        {
                            PostId = entityId,
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
                    .CountAsync(l => l.EntityId == entityId && l.LikeType == likeType);

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
}