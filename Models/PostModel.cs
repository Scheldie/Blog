using Blog.Entities;
using Blog.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class PostModel
    {
        public int Id { get; init; }
        
        public required string Title { get; init; }
        
        public required string Description { get; init; }
        
        [DataType(DataType.Upload)]
        public IEnumerable<IFormFile>? ImageFiles { get; set; }

        public bool DeleteExistingImages { get; set; }
        public int DeletedExistingImagesCount { get; set; }

        public List<string>? DeletedFilesPaths { get; set; }

        [DataType(DataType.Upload)]
        public IEnumerable<IFormFile>? NewImageFiles { get; set; }
        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; set; }


        public int ImagesCount { get; set; }

        public virtual IEnumerable<Post_Image>? PostImages { get; init; }

        public virtual IEnumerable<Post_Like>? PostLikes { get; init; }

        public virtual IEnumerable<Comment>? Comments { get; init; }

        public int ViewCount { get; set; }

        public virtual User? User { get; init; }

        public int UserId { get; set; }
        
        public bool IsCurrentUser { get; init; }

        public int WatcherId { get; init; }

        public bool IsLiked { get; set; }
    }
}
