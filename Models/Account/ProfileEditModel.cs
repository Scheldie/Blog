using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Account
{
    public class ProfileEditModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Bio")]
        [StringLength(500)]
        public string Bio { get; set; }
        public IFormFile AvatarFile { get; set; }

        public string AvatarPath { get; set; }
    }
}
