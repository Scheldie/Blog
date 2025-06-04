using Blog.Models.Post;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models.Account
{
    public class ProfileModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Display(Name = "Bio")]
        [StringLength(500)]
        public string Bio { get; set; }

        [NotMapped]
        [Display(Name = "Avatar")]
        public IFormFile AvatarFile { get; set; }

        public string AvatarPath { get; set; }

        public bool IsActive { get; set; }

        public bool IsCurrentUser {  get; set; }

        public virtual IEnumerable<PostModel> Posts { get; set; }

    }
}
