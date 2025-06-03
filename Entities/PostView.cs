using Blog.Entities;

namespace Blog.Entities
{
    public class PostView : IEntity
    {
        public int Id { get; set; } // PostId
        public DateOnly Date { get; set; }
        public int Count { get; set; }

        public virtual Post Post { get; set; }
    }
}
