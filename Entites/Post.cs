using Blog.Data.Intefaces;
using Blog.Models;
using Blog.Entites;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Blog.Entities
{
    public class Post : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual User Author { get; set; }
        
        public int UserId { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime PublishedAt { get; set; }

        public int ImagesCount { get; set; }

        public virtual IEnumerable<Image> Images { get; set; }

        public virtual IEnumerable<Like> Likes { get; set; }
    }
}
