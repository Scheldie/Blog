using Blog.Entities;
using Blog.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models.Post
{
    public class PostModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IEnumerable<IFormFile> ImageFiles { get; set; }


        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


        public int ImagesCount { get; set; }

        public virtual IEnumerable<Post_Image> Post_Images { get; set; }

        public virtual IEnumerable<Post_Like> Post_Likes { get; set; }

        public virtual IEnumerable<Comment> Comments { get; set; }

        public int ViewCount { get; set; }
    }
}
