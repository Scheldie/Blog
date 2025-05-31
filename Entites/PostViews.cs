using Blog.Entities;

namespace Blog.Entites
{
    public class PostViews : IEntity
    {
        public int Id { get; set; } // PostId
        public DateOnly Date { get; set; }
        public int Count { get; set; }

        public virtual Post Post { get; set; }
    }
}
