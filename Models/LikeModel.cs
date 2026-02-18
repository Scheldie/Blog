namespace Blog.Models;

public class LikeModel
{
    public int PostId { get; set; }
    public int? CommentId { get; set; }
    public bool IsComment { get; set; }
}