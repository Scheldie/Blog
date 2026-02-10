using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models.Request
{
    public class ProfileModel
    {
        public int Id { get; init; }

        [Display(Name = "User Name")]
        public required string UserName { get; init; }

        [EmailAddress]
        public string? Email { get; init; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; init; }

        [Display(Name = "Bio")]
        [StringLength(500)]
        public string? Bio { get; init; }

        [NotMapped]
        [Display(Name = "Avatar")]
        public IFormFile? Avatar { get; init; }

        public string? AvatarPath { get; init; }

        public bool IsActive { get; init; }

        public bool IsCurrentUser {  get; init; }
        
        [NotMapped]
        [Display(Name = "Watcher")]
        public int? WatcherId { get; init; }

        public virtual IEnumerable<PostModel>? Posts { get; init; }

    }
}
