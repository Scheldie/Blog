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

        [NotMapped]
        public IEnumerable<IFormFile> ImageFiles { get; set; }


        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime PublishedAt { get; set; }

        public int ImagesCount { get; set; }

        public virtual IEnumerable<Image> Images { get; set; }

        public virtual IEnumerable<Like> Likes { get; set; }

        public virtual IEnumerable<Like> Comments { get; set; }

        public int ViewCount { get; set; }
    }
}
