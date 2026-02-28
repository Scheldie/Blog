using Blog.Entities;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class CommentModel
    {
        public int Id { get; set; }

        public CommentUserModel? User { get; set; }

        [Required]
        [StringLength(600, MinimumLength = 1)]
        public required string? Text { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        
        public int LikesCount { get; set; }
        public int? ParentId { get; set; }
        public bool IsLiked { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool IsReply { get; set; }

        public int RepliesCount { get; set; } = 0;


    }
    public class CommentUserModel
    {
        public string? UserName { get; set; } 
        public string? AvatarPath { get; set; }
    }
}
