namespace Blog.Models
{
    public class UserSearchResultModel
    {
        public int Id { get; init; }
        public required string UserName { get; init; }
        public required string AvatarPath { get; init; }

    }
}
