using System.ComponentModel.DataAnnotations;

namespace ClinicQueue.ViewModels
{
    public class ReportVM
    {
        public string AppointmentId { get; set; } = "";

        [Required(ErrorMessage = "Title required")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Content required")]
        public string Content { get; set; } = "";

        public string? Remarks { get; set; }
    }
}