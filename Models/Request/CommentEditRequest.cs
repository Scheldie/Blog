using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Request;

public class CommentEditRequest
{
    public int Id { get; set; }
    [Required]
    [StringLength(600, MinimumLength = 1)]
    public required string Text { get; set; }
}