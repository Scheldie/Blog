using Blog.Entities;

namespace Blog.Entities
{
    public class Image : IEntity
    {
        public int Id { get; set; }

        public virtual User User { get; set; }

        public int UserId { get; set; }

        public string Path { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual IEnumerable<Post_Image> PostImages { get; set; }

    }
}
