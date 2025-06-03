using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Post
{
    public class PostCreateModel
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(1200)]
        public string Description { get; set; }
        public List<IFormFile> ImageFiles { get; set; }
    }
}
