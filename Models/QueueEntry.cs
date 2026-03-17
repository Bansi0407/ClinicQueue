using Newtonsoft.Json;

namespace ClinicQueue.Models
{
    public class QueueEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "";

        [JsonProperty("appointmentId")]
        public string? AppointmentId { get; set; }

        // patient fields
        [JsonProperty("patientName")]
        public string PatientName { get; set; } = "";

        [JsonProperty("patient")]
        public PersonInfo? Patient { get; set; }

        [JsonProperty("patientId")]
        public string? PatientId { get; set; }

        // doctor fields
        [JsonProperty("doctorName")]
        public string? DoctorName { get; set; }

        [JsonProperty("doctor")]
        public PersonInfo? Doctor { get; set; }

        [JsonProperty("doctorId")]
        public string? DoctorId { get; set; }

        // token fields
        [JsonProperty("tokenNumber")]
        public int TokenNumber { get; set; }

        [JsonProperty("token")]
        public int? Token { get; set; }

        [JsonProperty("queueNumber")]
        public int? QueueNumber { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = "waiting";

        [JsonProperty("checkInTime")]
        public DateTime? CheckInTime { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("date")]
        public string? Date { get; set; }

        [JsonProperty("appointment")]
        public AppointmentRef? Appointment { get; set; }

        // this is set manually after fetching — filled from cache
        public string? ResolvedDoctorName { get; set; }

        // helper — patient name
        public string DisplayPatientName
        {
            get
            {
                if (!string.IsNullOrEmpty(PatientName))
                    return PatientName;

                if (Patient != null && !string.IsNullOrEmpty(Patient.Name))
                    return Patient.Name;

                if (Appointment?.Patient != null &&
                    !string.IsNullOrEmpty(Appointment.Patient.Name))
                    return Appointment.Patient.Name;

                return "Unknown";
            }
        }

        // helper — doctor name (tries all fields + resolved from cache)
        public string DisplayDoctorName
        {
            get
            {
                // first check resolved name set from cache
                if (!string.IsNullOrEmpty(ResolvedDoctorName))
                    return ResolvedDoctorName;

                if (!string.IsNullOrEmpty(DoctorName))
                    return DoctorName;

                if (Doctor != null && !string.IsNullOrEmpty(Doctor.Name))
                    return Doctor.Name;

                if (Appointment?.Doctor != null &&
                    !string.IsNullOrEmpty(Appointment.Doctor.Name))
                    return Appointment.Doctor.Name;

                return "—";
            }
        }

        // helper — token number
        public int DisplayToken
        {
            get
            {
                if (TokenNumber > 0) return TokenNumber;
                if (Token.HasValue && Token > 0) return Token.Value;
                if (QueueNumber.HasValue && QueueNumber > 0) return QueueNumber.Value;
                return 0;
            }
        }

        // helper — appointment id
        public string DisplayAppointmentId
        {
            get
            {
                if (!string.IsNullOrEmpty(AppointmentId))
                    return AppointmentId;

                if (!string.IsNullOrEmpty(Appointment?.Id))
                    return Appointment.Id;

                return "";
            }
        }

        // helper — doctor id from any field
        public string DisplayDoctorId
        {
            get
            {
                if (!string.IsNullOrEmpty(DoctorId))
                    return DoctorId;

                if (!string.IsNullOrEmpty(Doctor?.Id))
                    return Doctor.Id;

                if (!string.IsNullOrEmpty(Appointment?.Doctor?.Id))
                    return Appointment.Doctor.Id;

                return "";
            }
        }

        // helper — check in time
        public string DisplayCheckInTime
        {
            get
            {
                if (CheckInTime.HasValue)
                    return CheckInTime.Value.ToString("hh:mm tt");

                if (CreatedAt.HasValue)
                    return CreatedAt.Value.ToString("hh:mm tt");

                return "—";
            }
        }
    }

    public class PersonInfo
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }
    }

    public class AppointmentRef
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("patient")]
        public PersonInfo? Patient { get; set; }

        [JsonProperty("doctor")]
        public PersonInfo? Doctor { get; set; }

        [JsonProperty("doctorId")]
        public string? DoctorId { get; set; }

        [JsonProperty("patientId")]
        public string? PatientId { get; set; }

        [JsonProperty("timeSlot")]
        public string? TimeSlot { get; set; }

        [JsonProperty("reason")]
        public string? Reason { get; set; }
    }
}