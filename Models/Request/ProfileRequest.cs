using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models.Request
{
    public class ProfileRequest
    {
        public int Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

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
        public IFormFile Avatar { get; set; }

        public string AvatarPath { get; set; }

        public bool IsActive { get; set; }

        public bool IsCurrentUser {  get; set; }
        
        [NotMapped]
        [Display(Name = "Watcher")]
        public int WatcherId { get; set; }

        public virtual IEnumerable<PostModel> Posts { get; set; }

    }
}
