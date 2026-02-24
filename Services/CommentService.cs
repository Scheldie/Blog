using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Blog.Data;
using Blog.Entities;
using Blog.Models;
using Blog.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services;

public class CommentService(BlogDbContext context, ILogger<CommentService> logger)
{
    private ILogger<CommentService> _logger = logger;

    private IQueryable<Comment> BuildCommentQuery(bool includeReplies)
        {
            IQueryable<Comment> query = context.Comments
                .Include(c => c.User)
                .Include(c => c.CommentLikes);

            if (includeReplies)
            {
                query = query
                    .Include(c => c.Replies)
                    .ThenInclude(r => r.User);
            }

            return query;
        }
        public async Task<List<CommentModel>> LoadComments(
            Expression<Func<Comment, bool>> filter,
            bool includeReplies,
            bool isReply,
            int userId)
        {

            var comments = await BuildCommentQuery(includeReplies)
                .Where(filter)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(c => c.toModel(
                userId > 0 && c.CommentLikes.Any(l => l.Like.UserId == userId),
                c.UserId == userId,
                isReply
            )).ToList();
        }
        
        [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
        public async Task<CommentModel?> AddComment(CommentCreateRequest model, int userId)
        {
            var user = await context.Users.FindAsync(userId);
            var parent = model.ParentId.HasValue
                ? await context.Comments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == model.ParentId.Value) : null;

            if (model.ParentId.HasValue && parent == null) return null;
            
            var isReply = parent != null;
            var rootParentId = parent?.ParentId ?? parent?.Id;
            var replyToId = parent?.Id;
            if (parent != null) model.PostId = parent.PostId;

            var comment = new Comment
            {
                Text = model.Text.Trim(), PostId = model.PostId, ParentId = rootParentId,
                ReplyTo = replyToId, UserId = userId, User = user!, CreatedAt = DateTime.UtcNow,
            };
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            return comment.toModel(false, true, isReply);
        }
        
        public async ValueTask<bool> EditComment(CommentEditRequest model, int userId)
        {
            Comment? comment = await context.Comments
                .FirstOrDefaultAsync(c => c.Id == model.Id && c.UserId == userId);

            if (comment == null) return false;

            comment.Text = model.Text.Trim();
            comment.UpdatedAt = DateTime.UtcNow;
            
            await context.SaveChangesAsync();

            return true;
        }

        
        public async ValueTask<bool> DeleteComment(int commentId, int userId)
        {
            var comment = await context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null) return false;
            if (comment.User.Id != userId) return false;

            await context.Comments.Where(c=>c.Id == commentId || c.ParentId == commentId).ExecuteDeleteAsync();
            await context.SaveChangesAsync();

            return true;
        }
}