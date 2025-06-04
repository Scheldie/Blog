using Blog.Entities;

namespace Blog.Entities
{
    public class Post_View : IEntity
    {
        public int Id { get; set; } // PostId
        public DateOnly Date { get; set; }
        public int Count { get; set; }

        public virtual Post Post { get; set; }
    }
}
