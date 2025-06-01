using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Account
{
    public class ForgottenModel
    {
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }
    }
}
