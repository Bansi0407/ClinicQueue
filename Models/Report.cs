namespace ClinicQueue.Models
{
    public class Report
    {
        public string Id { get; set; } = "";
        public string? AppointmentId { get; set; }
        public string DoctorName { get; set; } = "";
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string? Remarks { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}