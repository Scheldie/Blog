using System.ComponentModel.DataAnnotations;
using Blog.Entities;

namespace Blog.Entities
{
    public class Image : IEntity
    {
        public Image(DateTime createdAt, int userId)
        {
             
            CreatedAt = createdAt;
            UserId = userId;
            PostImages = new List<Post_Image>();
        }
        public int Id { get; set; }

        public virtual User User { get; set; }

        public int UserId { get; set; }

        [MaxLength(120)]
        public string ThumbnailUrl { get; set; } 
        [MaxLength(120)]
        public string PreviewUrl { get; set; } 
        [MaxLength(120)]
        public string FullUrl { get; set; } 
        [MaxLength(120)]
        public string OriginalUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Post_Image>? PostImages { get; set; }

    }
}
