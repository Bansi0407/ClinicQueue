using ClinicQueue.Models;
using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IApiClient _api;
        private readonly IDoctorsCacheService _cache;

        public DoctorService(IApiClient api, IDoctorsCacheService cache)
        {
            _api = api;
            _cache = cache;
        }

        public async Task<List<QueueEntry>> GetQueueAsync()
        {
            var result = await _api.GetAsync<List<QueueEntry>>("/doctor/queue");
            var queue = result ?? new List<QueueEntry>();

            // fill doctor names from cache using doctorId
            ResolveDoctorNames(queue);

            return queue;
        }

        public async Task<bool> AddPrescriptionAsync(string appointmentId,
                                                      PrescriptionVM model)
        {
            var payload = new
            {
                medicines = model.Medicines,
                diagnosis = model.Diagnosis,
                instructions = model.Instructions
            };
            try
            {
                await _api.PostAsync<object>($"/prescriptions/{appointmentId}", payload);
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> AddReportAsync(string appointmentId, ReportVM model)
        {
            var payload = new
            {
                title = model.Title,
                content = model.Content,
                remarks = model.Remarks
            };
            try
            {
                await _api.PostAsync<object>($"/reports/{appointmentId}", payload);
                return true;
            }
            catch { return false; }
        }

        // look up doctor name by doctorId from cache
        private void ResolveDoctorNames(List<QueueEntry> queue)
        {
            var doctors = _cache.GetDoctors();
            if (!doctors.Any()) return;

            foreach (var entry in queue)
            {
                if (!string.IsNullOrEmpty(entry.DoctorName)) continue;
                if (entry.Doctor != null &&
                    !string.IsNullOrEmpty(entry.Doctor.Name)) continue;

                var docId = entry.DisplayDoctorId;
                if (string.IsNullOrEmpty(docId)) continue;

                var doctor = doctors.FirstOrDefault(d =>
                    d.Id.Equals(docId, StringComparison.OrdinalIgnoreCase));

                if (doctor != null)
                    entry.ResolvedDoctorName = doctor.Name;
            }
        }
    }
}