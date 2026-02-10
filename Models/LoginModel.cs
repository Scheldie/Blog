using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class LoginModel
    {

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter correct email")]
        public required string Email { get; init; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password should not be less than 6 symbols")]
        public required string Password { get; init; }

    }
}
