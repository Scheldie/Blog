using Blog.Entities;

namespace Blog.Models
{
    public class PostModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public User Author { get; set; }
    }
}
