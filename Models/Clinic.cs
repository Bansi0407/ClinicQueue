namespace ClinicQueue.Models
{
    public class Clinic
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
    }
}