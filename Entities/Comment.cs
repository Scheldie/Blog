using System.ComponentModel.DataAnnotations;
using Blog.Entities;
using Blog.Models;

namespace Blog.Entities
{
    public class Comment : IEntity
    {
        public CommentModel toModel
        (bool? isLiked = false, bool? isCurrentUser = false, bool? isReply = false)
        {
            return new CommentModel()
            {
                Id = Id,
                User = new CommentUserModel()
                {
                    UserName = User.UserName,
                    AvatarPath = User.AvatarPath
                },
                Text = Text,
                CreatedAt = CreatedAt,
                LikesCount = CommentLikes.Count,
                ParentId = ParentId,
                IsLiked = isLiked ?? false,
                IsCurrentUser = isCurrentUser ?? false,
                IsReply = isReply ?? false,
                RepliesCount = Replies?.Count ?? 0 
            };
        }
        public int Id { get; set; }

        public  virtual required User User { get; init; }

        public int UserId { get; init; }
        
        [MaxLength(2000)]

        public string Text { get; set; }

        public virtual Post? Post {  get; init; }
        
        public int PostId { get; init; }

        public int? ParentId { get; init; }

        public virtual Comment? Parent { get; init; }

        public int? ReplyTo { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Comment_Like> CommentLikes { get; set; } = new List<Comment_Like>();

        public virtual ICollection<Comment> Replies { get; init; } = new List<Comment>();


    }
}
