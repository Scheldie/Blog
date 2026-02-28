using Blog.Entities;

namespace Blog.Entities
{
    public class Post_View : IEntity
    {
        public int Id { get; set; }
        public DateOnly Date { get; init; }
        public int Count { get; init; }

        public virtual Post? Post { get; set; }
    }
}
