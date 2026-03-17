using System.ComponentModel.DataAnnotations;

namespace ClinicQueue.ViewModels
{
    public class CreateUserVM
    {
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(6, ErrorMessage = "Min 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required]
        public string Role { get; set; } = "";

        public string? Phone { get; set; }
    }
}