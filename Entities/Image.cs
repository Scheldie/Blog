using System.ComponentModel.DataAnnotations;
using Blog.Entities;

namespace Blog.Entities
{
    public class Image : IEntity
    {
        public Image(String path, DateTime createdAt, int userId)
        {
            Path = path; 
            CreatedAt = createdAt;
            UserId = userId;
            PostImages = new List<Post_Image>();
        }
        public int Id { get; set; }

        public virtual User User { get; set; }

        public int UserId { get; set; }

        [MaxLength(200)]
        public string Path { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Post_Image>? PostImages { get; set; }

    }
}
