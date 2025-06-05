namespace Blog.Entities
{
    public class Post_Like : IEntity
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        public int LikeId { get; set; }

        public virtual Like Like { get; set; }
    }
}
