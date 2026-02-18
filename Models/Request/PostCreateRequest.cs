using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Request;

public class PostCreateRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }

    [DataType(DataType.Upload)]
    public IEnumerable<IFormFile>? ImageFiles { get; set; }
}
