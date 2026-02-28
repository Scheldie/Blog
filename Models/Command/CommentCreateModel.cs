using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Request;

public class CommentCreateModel
{
    [Required]
    [StringLength(600, MinimumLength = 1)]
    public required string Text { get; set; }
    public int? ParentId { get; set; }
    [Required]
    public int PostId { get; set; }

}