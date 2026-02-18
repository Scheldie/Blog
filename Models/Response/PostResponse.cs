namespace Blog.Models.Response;

public class PostResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string? AvatarPath { get; set; }
    public bool IsCurrentUser { get; set; }
    
    public List<string> ImagePaths { get; set; } = new List<string>();
    
    public int LikesCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public int CommentsCount { get; set; }
}