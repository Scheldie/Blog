namespace Blog.Models;

public class PostModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    
    public string DescriptionHtml { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string? UserAvatar { get; set; }

    public List<ImageModel> Images { get; set; } = new();
    public int ImagesCount { get; set; }

    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int ViewCount { get; set; }

    public bool IsLiked { get; set; }
    public bool IsCurrentUser { get; set; }
}
