using Blog.Entities;

namespace Blog.Models;

public static class PostMapper
{
    public static PostModel ToModel(
        Post post,
        int userId)
    {
        return new PostModel
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,

            UserId = post.UserId,
            UserName = post.Author.UserName,
            UserAvatar = post.Author.AvatarPath,

            Images = post.PostImages
                .OrderBy(i => i.Order)
                .Select(i => i.Image.Path)
                .ToList(),
            ImagesCount = post.ImagesCount,

            LikesCount = post.LikesCount,
            CommentsCount = post.Comments.Count(),
            ViewCount = post.ViewCount,

            IsLiked = post.PostLikes.Any(l=>l.Like.UserId == userId),
            IsCurrentUser = post.UserId == userId
        };
    }
}