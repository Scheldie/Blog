using Blog.Data;
using Blog.Data.Interfaces;
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
        private readonly IWebHostEnvironment _env;
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IPostImageRepository _postImageRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<CommentController> _logger;
        private readonly IFileService _fileService;

        public CommentController(BlogDbContext context, IWebHostEnvironment env,
            IUserRepository userRepository, ILogger<CommentController> logger,
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
                    .Include(c => c.Comment_Likes)
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
                        LikesCount = c.Comment_Likes.Count(),
                        IsLiked = userIdInt > 0 && c.Comment_Likes.Any(l => l.Like.UserId == userIdInt),
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
                            LikesCount = r.Comment_Likes.Count(),
                            IsLiked = userIdInt > 0 && r.Comment_Likes.Any(l => l.Like.UserId == userIdInt),
                            CreatedAt = r.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                            IsCurrentUser = r.User.Id == userIdInt
                        })
                    })
                    .ToListAsync();
                return Ok(comments.OrderByDescending(p => p.CreatedAt));
            }
            catch (Exception ex)
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return Unauthorized();

                int? rootParentId = null;
                int? replyToId = null;
                string replyToUsername = null;

                if (dto.ParentId.HasValue)
                {
                    var parentComment = await _context.Comments
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.Id == dto.ParentId.Value);

                    if (parentComment == null) return BadRequest("Parent comment not found");

                    // Root parent всегда будет самым верхним комментарием в цепочке
                    rootParentId = parentComment.ParentId ?? dto.ParentId;

                    // ReplyToId - это ID комментария, на который непосредственно отвечаем
                    replyToId = dto.ParentId;

                    // Получаем username автора комментария, на который отвечаем
                    replyToUsername = parentComment.User.UserName;

                    dto.PostId = parentComment.PostId;
                }

                var comment = new Comment
                {
                    Text = dto.Text.Trim(),
                    PostId = dto.PostId,
                    ParentId = rootParentId,  // Всегда указывает на корневой комментарий
                    ReplyTo = replyToId,   // Указывает на непосредственный комментарий-ответ
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                var replyToUser = _context.Comments?.FirstOrDefaultAsync(c => c.Id == comment.ReplyTo)?.Result?.User?.UserName;
                return Ok(new
                {
                    success = true,
                    comment.Id,
                    User = new { user.Id, user.UserName, user.AvatarPath },
                    comment.Text,
                    CreatedAt = comment.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                    ParentId = comment.ParentId,
                    ReplyToId = comment.ReplyTo,
                    ReplyToUser = replyToUser,
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                Comment comment = await _context.Comments
                    .FirstOrDefaultAsync(c => c.Id == model.Id && c.UserId == userId);

                if (comment == null)
                {
                    return NotFound(new { error = "Comment not found or you don't have permission to edit it" });
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
                    .Include(c => c.Comment_Likes)
                    .Select(c => new
                    {
                        c.Id,
                        User = new { Id = c.UserId, c.User.UserName, c.User.AvatarPath },
                        c.Text,
                        CreatedAt = c.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                        LikesCount = c.Comment_Likes.Count(),
                        IsLiked = userIdInt > 0 && c.Comment_Likes.Any(l => l.Like.UserId == userIdInt),
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
