using System.Linq.Expressions;
using Blog.Data;
using Blog.Entities;
using Blog.Infrastructure.Markdown;
using Blog.Models;
using Blog.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services;

public class CommentService(BlogDbContext context, ILogger<CommentService> logger, IMarkdownRenderer _markdown)
{
    
    public async Task<List<CommentModel>> LoadCommentsPaged(
        Expression<Func<Comment, bool>> filter,
        bool isReply,
        int userId,
        int page,
        int pageSize)
    {
        var query = context.Comments
            .AsNoTracking()
            .Where(filter);
        
        query = isReply
            ? query.OrderBy(c => c.CreatedAt)              
            : query.OrderByDescending(c => c.CreatedAt);

        var comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                Comment = c,
                IsLiked = userId > 0 && c.CommentLikes
                    .Any(l => l.Like != null && l.Like.UserId == userId),
                User = new CommentUserModel
                {
                    AvatarPath = c.User.AvatarSmall32Url,
                    UserName = c.User.UserName
                }
            })
            .ToListAsync();

        return comments .Select(x =>
            CommentMapper.ToModel( 
                x.Comment, 
                x.User, 
                _markdown.ToHtml(x.Comment.Text.Trim()),
                x.IsLiked, 
                x.Comment.UserId == userId, 
                isReply)
        ) .ToList();
    }


    
    public async Task<CommentModel?> AddComment(CommentCreateModel model, int userId)
    {
        var user = await context.Users.FindAsync(userId);
        var commentUser = new CommentUserModel(){AvatarPath = user?.AvatarSmall32Url,  UserName = user?.UserName};
        if (user == null) return null;

        Comment? parent = null;

        if (model.ParentId.HasValue)
        {
            parent = await context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == model.ParentId.Value);

            if (parent == null) return null;

            model.PostId = parent.PostId;
        }

        var isReply = parent != null;
        var rootParentId = parent?.ParentId ?? parent?.Id;
        var replyToId = parent?.Id;

        var comment = new Comment
        {
            Text = model.Text.Trim(),
            PostId = model.PostId,
            ParentId = rootParentId,
            ReplyTo = replyToId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            User = user
        };

        await context.Comments.AddAsync(comment);

        if (parent != null)
        {
            var parentTracked = await context.Comments.FirstAsync(c => c.Id == rootParentId);
            parentTracked.RepliesCount++;
        }

        await context.SaveChangesAsync();
        var commentModel = CommentMapper.ToModel(
            comment,
            commentUser,
            _markdown.ToHtml(comment.Text.Trim()),
            false,
            true,
            isReply
        );
        return commentModel;

    }
    
    public async Task<CommentModel?> EditComment(CommentEditModel model, int userId)
    {
        var user = await context.Users.FindAsync(userId);
        var commentUser = new CommentUserModel(){AvatarPath = user?.AvatarSmall32Url,  UserName = user?.UserName};
        var comment = await context.Comments
            .FirstOrDefaultAsync(c => c.Id == model.Id && c.UserId == userId);

        if (comment == null) return null;

        comment.Text = model.Text.Trim();
        comment.UpdatedAt = DateTime.UtcNow;
        var isReply = comment.ParentId != null;
        await context.SaveChangesAsync();
        var commentModel = CommentMapper.ToModel(
            comment,
            commentUser,
            _markdown.ToHtml(comment.Text.Trim()),
            false,
            true,
            isReply
        );
        return commentModel;
    }
    
    public async ValueTask<bool> DeleteComment(int commentId, int userId)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

        if (comment == null) return false;

        if (comment.ParentId.HasValue)
        {
            var parent = await context.Comments.FirstOrDefaultAsync(c => c.Id == comment.ParentId.Value);
            if (parent != null) parent.RepliesCount--;
        }

        await context.Comments
            .Where(c => c.Id == commentId || c.ParentId == commentId)
            .ExecuteDeleteAsync();

        return true;
    }
}
