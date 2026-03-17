using System.ComponentModel.DataAnnotations;

namespace ClinicQueue.ViewModels
{
    public class BookAppointmentVM
    {
        [Required(ErrorMessage = "Select a doctor")]
        [Display(Name = "Doctor")]
        public string DoctorId { get; set; } = "";

        [Required(ErrorMessage = "Pick a date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today.AddDays(1);

        [Display(Name = "Time Slot")]
        public string? TimeSlot { get; set; }

        [Display(Name = "Reason")]
        public string? Reason { get; set; }

        public string? Notes { get; set; }
    }
}