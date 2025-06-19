using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Post
{
    public class CommentEditModel
    {
        [Required]
        public int CommentId { get; set; }
        [Required]
        [MinLength(1, ErrorMessage ="Введите комментарий длиннее 1 символа")]
        public string Text { get; set; }
    }
}
