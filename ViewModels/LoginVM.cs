using System.ComponentModel.DataAnnotations;

namespace ClinicQueue.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}