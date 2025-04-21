using Blog.Data.Intefaces;
using Blog.Models;

namespace Blog.Entities
{
    public class Post : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public User Author { get; set; }
    }
}
