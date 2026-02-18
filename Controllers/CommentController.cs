using Blog.Data;
using Blog.Entities;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly ILogger<CommentController> _logger;
        
        public CommentController(BlogDbContext context, ILogger<CommentController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetComments(int postId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdInt = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
                var comments = await _context.Comments
                    .Where(c => c.PostId == postId && c.ParentId == null)
                    .Include(c => c.User)
                    .Include(c => c.CommentLikes)
                    .Include(c => c.Replies)
                        .ThenInclude(r => r.User)
                    .Select(c => new
                    {
                        c.Id,
                        User = new
                        {
                            Id = c.UserId,
                            UserName = c.User.UserName,
                            c.User.AvatarPath
                        },
                        c.UserId,
                        c.Text,
                        c.PostId,
                        c.ParentId,
                        c.ReplyTo,
                        ReplyToId = c.ReplyTo,
                        CreatedAt = c.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                        LikesCount = c.CommentLikes == null ? 0 : c.CommentLikes.Count(),
                        IsLiked = userIdInt > 0 && c.CommentLikes.Any(l => l.Like.UserId == userIdInt),
                        IsCurrentUser = c.User.Id == userIdInt && c.UserId == userIdInt,
                        IsReply = false,
                        Replies = c.Replies.Select(r => new
                        {
                            r.Id,
                            User = new
                            {
                                Id = r.UserId,
                                UserName = r.User.UserName,
                                r.User.AvatarPath
                            },
                            r.Text,
                            r.PostId,
                            r.ParentId,
                            IsReply = true,

                            ReplyToId = r.ReplyTo,
                            ReplyToUser = _context.Comments.FirstOrDefault(c => c.Id == r.ReplyTo).User.UserName,
                            LikesCount = r.CommentLikes.Count(),
                            IsLiked = userIdInt > 0 && r.CommentLikes.Any(l => l.Like.UserId == userIdInt),
                            CreatedAt = r.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                            IsCurrentUser = r.User.Id == userIdInt
                        })
                    })
                    .ToListAsync();
                return Ok(comments.OrderByDescending(p => p.CreatedAt));
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error loading comments");
                return StatusCode(500, new { error = "Failed to load comments. Please try again." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentModel dto)
        {
            try
            {
                int userId;
                if (!Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId)) return Unauthorized();
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return Unauthorized();

                int? rootParentId = null;
                int? replyToId = null;

                if (dto.ParentId.HasValue)
                {
                    var parentComment = await _context.Comments
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.Id == dto.ParentId.Value);
                    if (parentComment == null) return BadRequest("Parent comment not found");
                    rootParentId = parentComment.ParentId ?? dto.ParentId;
                    replyToId = dto.ParentId;
                    dto.PostId = parentComment.PostId;
                }

                var comment = new Comment
                {
                    Text = dto.Text.Trim(),
                    PostId = dto.PostId,
                    ParentId = rootParentId,  // Всегда указывает на корневой комментарий
                    ReplyTo = replyToId,   // Указывает на непосредственный комментарий-ответ
                    UserId = user.Id,
                    User = user,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                string? replyToUserName = null;

                if (replyToId.HasValue)
                {
                    replyToUserName = await _context.Comments
                        .Where(c => c.Id == replyToId.Value)
                        .Select(c => c.User.UserName)
                        .FirstOrDefaultAsync();
                }
                return Ok(new
                {
                    success = true,
                    comment.Id,
                    User = new { user.Id, user.UserName, user.AvatarPath },
                    comment.Text,
                    CreatedAt = comment.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                    ParentId = comment.ParentId,
                    ReplyToId = comment.ReplyTo,
                    ReplyToUser = replyToUserName,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment");
                return StatusCode(500, new { error = "Failed to add comment" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditComment([FromBody] CommentModel model)
        {
            try
            {
                int userId;
                if (!Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId)) return Unauthorized();
                Comment? comment = await _context.Comments
                    .FirstOrDefaultAsync(c => c.Id == model.Id && c.UserId == userId);

                if (comment == null)
                {
                    return NotFound(new { error = "Comment not found" });
                }

                comment.Text = model.Text.Trim();
                comment.UpdatedAt = DateTime.UtcNow;

                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing comment");
                return StatusCode(500, new { error = "Failed to edit comment" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetReplies(int commentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdInt = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);

                var replies = await _context.Comments
                    .Where(c => c.ParentId == commentId)
                    .Include(c => c.User)
                    .Include(c => c.CommentLikes)
                    .Select(c => new
                    {
                        c.Id,
                        User = new { Id = c.UserId, c.User.UserName, c.User.AvatarPath },
                        c.Text,
                        CreatedAt = c.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                        LikesCount = c.CommentLikes.Count(),
                        IsLiked = userIdInt > 0 && c.CommentLikes.Any(l => l.Like.UserId == userIdInt),
                        IsCurrentUser = c.User.Id == userIdInt && c.UserId == userIdInt,
                        IsReply = true,
                        ParentId = c.ParentId

                    })
                    .ToListAsync();

                return Ok(replies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading replies");
                return StatusCode(500, new { error = "Failed to load replies" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                int userId;
                if (!Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId)) return Unauthorized();
                var comment = await _context.Comments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment == null) return NotFound();

                // Проверяем, что пользователь удаляет свой комментарий
                if (comment.User.Id != userId)
                    return Unauthorized();

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment");
                return StatusCode(500, new { error = "Failed to delete comment" });
            }
        }
    }
}
