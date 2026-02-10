using System.Security.Claims;
using Blog.Data;
using Blog.Data.Interfaces;
using Blog.Entities;
using Blog.Entities.Enums;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class LikeController : Controller
{
    private readonly BlogDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProfileController> _logger;


    public LikeController(BlogDbContext context, 
        IUserRepository userRepository, ILogger<ProfileController> logger)
    {
        _context = context;
        _userRepository = userRepository;
        _logger = logger;
    }

    // GET
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
}