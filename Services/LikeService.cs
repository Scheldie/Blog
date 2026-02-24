using Blog.Data;
using Blog.Entities;
using Blog.Entities.Enums;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services;

public class LikeService(
    BlogDbContext context,
    ILogger<LikeService> logger)
{

    public async ValueTask<Tuple<bool, int>> ToggleLike(LikeModel model, int userId)
    {
        
        var entityId = model.IsComment ? model.CommentId : model.PostId;
        if (entityId == null) return new(false, 0);
        if (!await EntityExists(model)) return new (false, 0);

        var user =  await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        var likeType = model.IsComment ? LikeType.Comment : LikeType.Post;

        var like = await GetExistingLike(entityId.Value, userId, likeType);

        if (like == null) await AddLike(entityId.Value, userId, likeType, model.PostId, user);
        else context.Likes.Remove(like);

        await context.SaveChangesAsync();

        var likesCount = await CountLikes(entityId.Value, likeType);

        return new(like == null, likesCount);
    }

    private async Task<bool> EntityExists(LikeModel model)
    {
        return model.IsComment
            ? await context.Comments.AnyAsync(c => c.Id == model.CommentId)
            : await context.Posts.AnyAsync(p => p.Id == model.PostId);
    }

    private Task<Like?> GetExistingLike(int entityId, int userId, LikeType type)
    {
        return context.Likes.FirstOrDefaultAsync(l =>
            l.EntityId == entityId &&
            l.UserId == userId &&
            l.LikeType == type);
    }

    private async Task AddLike(int entityId, int userId, LikeType type, int postId, User? user)
    {
        var like = new Like(entityId, userId, type, user);
        await context.Likes.AddAsync(like);
        await context.SaveChangesAsync();

        if (type == LikeType.Comment)
        {
            await context.Comment_Likes.AddAsync(new Comment_Like(entityId, postId, like.Id, like));
        }
        else
        {
            await context.Post_Likes.AddAsync(new Post_Like(entityId, like.Id, like));
        }
    }

    private async ValueTask<int> CountLikes(int entityId, LikeType type)
    {
        return await context.Likes.CountAsync(l => l.EntityId == entityId && l.LikeType == type);
    }
}