namespace Blog.Models;

public class FollowUserModel
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string AvatarPath { get; set; }
    public bool IsFollowing { get; set; }
    
    public string CurrentUsername { get; set; }
}
