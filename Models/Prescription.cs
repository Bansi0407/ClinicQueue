namespace ClinicQueue.Models
{
    public class Prescription
    {
        public string Id { get; set; } = "";
        public string? AppointmentId { get; set; }
        public string DoctorName { get; set; } = "";
        public string Medicines { get; set; } = "";
        public string? Diagnosis { get; set; }
        public string? Instructions { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}