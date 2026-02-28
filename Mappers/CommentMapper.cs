using Blog.Entities;

namespace Blog.Models;

public class CommentMapper
{
    public static CommentModel ToModel(
        Comment comment,
        CommentUserModel commentUser,
        bool? isLiked = false, 
        bool? isCurrentUser = false, 
        bool? isReply = false)
    {
        return new CommentModel()
        {
            Id = comment.Id,
            User = commentUser,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            LikesCount = comment.LikesCount,
            ParentId = comment.ParentId,
            IsLiked = isLiked ?? false,
            IsCurrentUser = isCurrentUser ?? false,
            IsReply = isReply ?? false,
            RepliesCount = comment.RepliesCount 
        };
    }
}