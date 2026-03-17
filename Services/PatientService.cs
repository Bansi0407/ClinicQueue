using ClinicQueue.Models;
using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services
{
    public class PatientService : IPatientService
    {
        private readonly IApiClient _api;
        private readonly IDoctorsCacheService _cache;

        public PatientService(IApiClient api, IDoctorsCacheService cache)
        {
            _api = api;
            _cache = cache;
        }

        public async Task<bool> BookAppointmentAsync(BookAppointmentVM model, string patientId)
        {
            // send all possible field names so api can find what it needs
            var payload = new
            {
                patientId = patientId,
                doctorId = model.DoctorId,
                date = model.Date.ToString("yyyy-MM-dd"),
                appointmentDate = model.Date.ToString("yyyy-MM-dd"),
                timeSlot = model.TimeSlot,
                slot = model.TimeSlot,
                reason = model.Reason,
                notes = model.Notes
            };

            await _api.PostAsync<object>("/appointments", payload);
            return true;
        }

        public async Task<List<Appointment>> GetMyAppointmentsAsync()
        {
            var result = await _api.GetAsync<List<Appointment>>("/appointments/my");
            return result ?? new List<Appointment>();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(string id)
        {
            return await _api.GetAsync<Appointment>($"/appointments/{id}");
        }

        public async Task<List<Prescription>> GetMyPrescriptionsAsync()
        {
            var result = await _api.GetAsync<List<Prescription>>("/prescriptions/my");
            return result ?? new List<Prescription>();
        }

        public async Task<List<Report>> GetMyReportsAsync()
        {
            var result = await _api.GetAsync<List<Report>>("/reports/my");
            return result ?? new List<Report>();
        }

        public async Task<List<User>> GetDoctorsAsync()
        {
            // try 1 — dedicated doctors endpoint
            var try1 = await _api.TryGetAsync<List<User>>("/doctors");
            if (try1 != null && try1.Any())
                return try1;

            // try 2 — users with role filter
            var try2 = await _api.TryGetAsync<List<User>>("/users?role=doctor");
            if (try2 != null && try2.Any())
                return try2;

            // try 3 — admin/users (works if patient has permission)
            var try3 = await _api.TryGetAsync<List<User>>("/admin/users");
            if (try3 != null && try3.Any())
                return try3
                    .Where(u => u.Role.Equals("doctor", StringComparison.OrdinalIgnoreCase))
                    .ToList();

            // try 4 — plain users
            var try4 = await _api.TryGetAsync<List<User>>("/users");
            if (try4 != null && try4.Any())
                return try4
                    .Where(u => u.Role.Equals("doctor", StringComparison.OrdinalIgnoreCase))
                    .ToList();

            // try 5 — from singleton cache (admin must visit Users page first)
            if (_cache.HasDoctors())
                return _cache.GetDoctors();

            return new List<User>();
        }
    }
}




/*using ClinicQueue.Models;
using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services
{
    public class PatientService : IPatientService
    {
        private readonly IApiClient _api;

        public PatientService(IApiClient api)
        {
            _api = api;
        }

        public async Task<bool> BookAppointmentAsync(BookAppointmentVM model)
        {
            var payload = new
            {
                doctorId = model.DoctorId,
                date = model.Date.ToString("yyyy-MM-dd"),
                timeSlot = model.TimeSlot,
                reason = model.Reason,
                notes = model.Notes
            };
            try
            {
                await _api.PostAsync<object>("/appointments", payload);
                return true;
            }
            catch { return false; }
        }

        public async Task<List<Appointment>> GetMyAppointmentsAsync()
        {
            var result = await _api.GetAsync<List<Appointment>>("/appointments/my");
            return result ?? new List<Appointment>();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(string id)
        {
            return await _api.GetAsync<Appointment>($"/appointments/{id}");
        }

        public async Task<List<Prescription>> GetMyPrescriptionsAsync()
        {
            var result = await _api.GetAsync<List<Prescription>>("/prescriptions/my");
            return result ?? new List<Prescription>();
        }

        public async Task<List<Report>> GetMyReportsAsync()
        {
            var result = await _api.GetAsync<List<Report>>("/reports/my");
            return result ?? new List<Report>();
        }

        // get all users then filter doctors
        public async Task<List<User>> GetDoctorsAsync()
        {
            var users = await _api.GetAsync<List<User>>("/admin/users");
            if (users == null) return new List<User>();
            return users.Where(u => u.Role.ToLower() == "doctor").ToList();
        }
    }
}*/


