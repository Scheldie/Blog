using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class ForgottenModel
    {
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter correct email")]
        public required string Email { get; init; }
    }
}
