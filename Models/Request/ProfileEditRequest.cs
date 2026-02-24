using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models.Request;

public class ProfileEditRequest
{
    public int Id { get; init; }

    [Display(Name = "User Name")]
    [Required(ErrorMessage = "Please enter a username")]
    public required string UserName { get; init; }

    [Display(Name = "Bio")]
    [StringLength(500)]
    public string? Bio { get; init; }
    
    [Display(Name = "Avatar")]
    public IFormFile? Avatar { get; init; }
    
    public bool RemoveAvatar { get; init; }
}