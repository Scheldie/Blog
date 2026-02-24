namespace Blog.Entities
{
    public class Post_Like : IEntity
    {
        public Post_Like() {}

        public Post_Like(int entityId, int likeId, Like like)
        {
            PostId = entityId;
            LikeId = likeId;
            Like = like;
        }
        public int Id { get; set; }

        public int PostId { get; init; }

        public virtual Post? Post { get; init; }

        public int LikeId { get; init; }

        public virtual Like? Like { get; init; }
    }
}
