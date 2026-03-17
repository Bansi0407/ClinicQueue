using System.ComponentModel.DataAnnotations;

namespace ClinicQueue.ViewModels
{
    public class PrescriptionVM
    {
        public string AppointmentId { get; set; } = "";

        [Required(ErrorMessage = "Medicines required")]
        [Display(Name = "Medicines & Dosage")]
        public string Medicines { get; set; } = "";

        public string? Diagnosis { get; set; }

        [Display(Name = "Instructions")]
        public string? Instructions { get; set; }
    }
}