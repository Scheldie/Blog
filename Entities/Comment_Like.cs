namespace Blog.Entities
{
    public class Comment_Like : IEntity
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        public int CommentId { get; set; }

        public virtual Comment Comment { get; set; }

        public int LikeId { get; set; }

        public virtual Like Like { get; set; }
    }
}

