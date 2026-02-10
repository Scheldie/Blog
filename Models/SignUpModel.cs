using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class SignUpModel
    {
        [DataType(DataType.Text)]
        [MinLength(4, ErrorMessage = "UserName should not be less than 4 symbols")]
        [Required(ErrorMessage = "Please enter correct name")]
        public required string UserName { get; init; }

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter correct email")]
        public required string Email { get; init; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password should not be less than 6 symbols")]
        [Required(ErrorMessage = "Please enter correct password")]
        public required string Password { get; init; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords you entered don't match")]
        [Required(ErrorMessage = "Please enter correct confirm password")]
        public required string ConfirmPassword { get; init; }

    }
}
