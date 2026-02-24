namespace Blog.Entities
{
    public class Comment_Like : IEntity
    {
        public Comment_Like() {}

        public Comment_Like(int entityId, int postId, int likeId, Like like)
        {
            CommentId = entityId;
            PostId = postId;
            LikeId = likeId;
            Like = like;
        }
        public int Id { get; set; }

        public int PostId { get; init; }

        public virtual Post? Post { get; init; }

        public int CommentId { get; init; }

        public virtual Comment? Comment { get; init; }

        public int LikeId { get; init; }

        public  virtual Like? Like { get; init; }
    }
}

