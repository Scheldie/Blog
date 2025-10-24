using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class SignUpModel
    {
        [DataType(DataType.Text)]
        [MinLength(4, ErrorMessage = "UserName should not be less than 4 symbols")]
        public string UserName { get; set; }

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password should not be less than 6 symbols")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords you entered don't match")]
        public string ConfirmPassword { get; set; }

    }
}
