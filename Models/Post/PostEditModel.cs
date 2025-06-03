using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Blog.Models.Post
{
    public class PostEditModel
    {
        public int PostId { get; set; }
        public string Description { get; set; }
        public bool DeleteExistingImages { get; set; }
        public List<IFormFile> NewImageFiles { get; set; }
    }
}
