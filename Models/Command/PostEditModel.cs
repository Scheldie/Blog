using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Request;

public class PostEditModel
{
    public int Id { get; set; }
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Длина заголовка должна быть от 1 до 200 символов")]
    public required string Title { get; set; }
    [StringLength(6000, MinimumLength = 1, ErrorMessage = "Длина описания должна быть от 1 до 6000 символов")]
    public required string Description { get; set; }
    public List<string>? ExistingImagePaths { get; set; }
    [DataType(DataType.Upload)]
    public IEnumerable<IFormFile>? NewImageFiles { get; set; }
}