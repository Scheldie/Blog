namespace Blog.Entities
{
    public class Comment_Like : IEntity
    {
        public int Id { get; set; }

        public int PostId { get; init; }

        public virtual Post? Post { get; init; }

        public int CommentId { get; init; }

        public virtual Comment? Comment { get; init; }

        public int LikeId { get; init; }

        public  virtual required Like Like { get; init; }
    }
}

