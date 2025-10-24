using Blog.Entities;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class CommentModel
    {
        public int Id { get; set; }

        public virtual User User { get; set; }
        public string UserName => User?.UserName;
        public string AvatarPath => User?.AvatarPath;

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Text { get; set; }

        public virtual Entities.Post Post { get; set; }
        [Required]
        public int PostId { get; set; }

        public int? ParentId { get; set; }  
        public virtual Comment Parent { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public string CreatedAtFormatted => CreatedAt.ToString("dd.MM.yyyy HH:mm");

        public DateTime UpdatedAt { get; set; }

        public virtual IEnumerable<Comment_Like>? Comment_Likes { get; set; }
        public int LikesCount => Comment_Likes?.Count() ?? 0;

        public virtual IEnumerable<CommentModel>? Replies { get; set; }
        public int RepliesCount => Replies?.Count() ?? 0;

        public bool IsCurrentUserComment { get; set; }  
    }
}
