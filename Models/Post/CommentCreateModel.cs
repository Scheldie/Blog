using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Post
{
    public class CommentCreateModel
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Text { get; set; }

        public int? ParentId { get; set; }
    }
}
