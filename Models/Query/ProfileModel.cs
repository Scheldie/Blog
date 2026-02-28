using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blog.Entities;

namespace Blog.Models
{
    public sealed class ProfileModel
    {
        public ProfileModel(User user, bool isCurrentUser)
        {
            UserName = user.UserName;
            Email = isCurrentUser ? user.Email : null;
            Bio = user.Bio;
            UserName = user.UserName;
            AvatarPath = user.AvatarProfileUrl;
            IsCurrentUser = isCurrentUser;
            WatcherId = user.Id;
            IsActive = user.IsActive;
        }

        [Display(Name = "User Name")]
        public string UserName { get; init; }

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

        public IEnumerable<PostModel>? Posts { get; init; }

        [NotMapped]
        public bool RemoveAvatar { get; init; }


    }
}
