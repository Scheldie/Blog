using Blog.Data.Intefaces;
using Blog.Entities;

namespace Blog.Entites
{
    public class Image : IEntity
    {
        public int Id { get; set; }

        public virtual User User { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

    }
}
