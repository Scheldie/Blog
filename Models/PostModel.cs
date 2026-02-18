using Blog.Entities;
using System.ComponentModel.DataAnnotations;
using Blog.Data;

namespace Blog.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        
        [DataType(DataType.Upload)]
        public bool DeleteExistingImages { get; set; }
        public List<string>? DeletedFilesPaths { get; set; }
        
        [DataType(DataType.Upload)]
        public IEnumerable<IFormFile>? NewImageFiles { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int LikesCount { get; set; }
        public int ImagesCount { get; set; }
        
        public int ViewCount { get; set; }
        
        public int UserId { get; set; }
        
        public bool IsCurrentUser { get; set; }

        public int WatcherId { get; set; }

        public bool IsLiked { get; set; }
        public virtual User? User { get; set; } 
        public virtual IEnumerable<Post_Image>? PostImages { get; set; } = new List<Post_Image>();
        [DataType(DataType.Upload)]
        public IEnumerable<IFormFile>? ImageFiles { get; set; } = new List<IFormFile>();

        public virtual IEnumerable<Post_Like>? PostLikes { get; set; }

        public virtual IEnumerable<Comment>? Comments { get; set; }
        
        public Post ToEntity(int userId)
        {
            var post = new Post
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PublishedAt = DateTime.UtcNow,
                ImagesCount = this.ImagesCount,
                ViewCount = this.ViewCount,
                LikesCount = this.LikesCount,
                Author = this.User,
                PostImages = new List<Post_Image>(),
                PostLikes = this.PostLikes,
                Comments = this.Comments,
            };

            return post;
        }
    }
}
