using Newtonsoft.Json;

namespace ClinicQueue.Models
{
    public class Appointment
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "";

        [JsonProperty("patientId")]
        public string? PatientId { get; set; }

        [JsonProperty("patientName")]
        public string PatientName { get; set; } = "";

        [JsonProperty("doctorId")]
        public string? DoctorId { get; set; }

        // api may return doctorName directly or inside doctor object
        [JsonProperty("doctorName")]
        public string DoctorName { get; set; } = "";

        // nested doctor object from some apis
        [JsonProperty("doctor")]
        public DoctorInfo? Doctor { get; set; }

        // api may send date as appointmentDate or date
        [JsonProperty("date")]
        public string? DateRaw { get; set; }

        [JsonProperty("appointmentDate")]
        public string? AppointmentDateRaw { get; set; }

        [JsonProperty("timeSlot")]
        public string? TimeSlot { get; set; }

        [JsonProperty("slot")]
        public string? Slot { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = "scheduled";

        [JsonProperty("reason")]
        public string? Reason { get; set; }

        [JsonProperty("notes")]
        public string? Notes { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        // helper — returns correct date from whichever field has data
        public DateTime Date
        {
            get
            {
                // try appointmentDate first
                if (!string.IsNullOrEmpty(AppointmentDateRaw))
                {
                    if (DateTime.TryParse(AppointmentDateRaw, out var d1))
                        return d1;
                }
                // try date field
                if (!string.IsNullOrEmpty(DateRaw))
                {
                    if (DateTime.TryParse(DateRaw, out var d2))
                        return d2;
                }
                return DateTime.MinValue;
            }
        }

        // helper — returns doctor name from whichever field has data
        public string DisplayDoctorName
        {
            get
            {
                if (!string.IsNullOrEmpty(DoctorName))
                    return DoctorName;

                if (Doctor != null && !string.IsNullOrEmpty(Doctor.Name))
                    return Doctor.Name;

                return "—";
            }
        }

        // helper — returns time slot from whichever field has data
        public string DisplayTimeSlot
        {
            get
            {
                if (!string.IsNullOrEmpty(TimeSlot)) return TimeSlot;
                if (!string.IsNullOrEmpty(Slot)) return Slot;
                return "—";
            }
        }
    }

    // nested doctor object some apis return
    public class DoctorInfo
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }
    }
}