using System.ComponentModel.DataAnnotations;
using Blog.Models;

namespace Blog.Entities
{
    public class Post : IEntity
    {
        public PostModel ToModel(int userId)
        {
            return new PostModel()
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                LikesCount = this.LikesCount,
                ImagesCount = this.ImagesCount,
                ViewCount = this.ViewCount,
                UserId = this.UserId,
                User = this.Author,
                PostImages = this.PostImages,
                PostLikes = this.PostLikes,
                Comments = this.Comments,
                WatcherId = userId,
                IsLiked = this.PostLikes.Any(pl => pl.Like.UserId == userId),
                IsCurrentUser = this.UserId == userId
            };
        }
        public int Id { get; set; }
        [MaxLength(200)]
        public required string Title { get; set; }
        [MaxLength(1500)]
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
    }
   

}
