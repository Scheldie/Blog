using System.ComponentModel.DataAnnotations;
using Blog.Models;

namespace Blog.Entities
{
    public class Post : IEntity
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public required string Title { get; set; }
        [MaxLength(6000)]
        public required string Description { get; set; } 
        public  virtual required User Author { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public DateTime PublishedAt { get; set; }
        public int ImagesCount { get; set; }
        public virtual ICollection<Post_Image> PostImages { get; set; } = new List<Post_Image>();
        public virtual IEnumerable<Post_View>? PostViews { get; set; }
        public virtual IEnumerable<Comment>? Comments { get; set; }
        public virtual IEnumerable<Post_Like>? PostLikes { get; set; }
        public int ViewCount { get; set; }
        public int LikesCount { get; set; }
        
        public int CommentsCount { get; set; }
    }
   

}
