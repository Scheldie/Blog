using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Request;

public class PostEditRequest
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public List<string>? ExistingImagePaths { get; set; }
    [DataType(DataType.Upload)]
    public IEnumerable<IFormFile>? NewImageFiles { get; set; }
}