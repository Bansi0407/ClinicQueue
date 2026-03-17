using ClinicQueue.Models;
using ClinicQueue.Services.Interfaces;

namespace ClinicQueue.Services
{
    public class ReceptionService : IReceptionService
    {
        private readonly IApiClient _api;
        private readonly IDoctorsCacheService _cache;

        public ReceptionService(IApiClient api, IDoctorsCacheService cache)
        {
            _api = api;
            _cache = cache;
        }

        public async Task<List<QueueEntry>> GetDailyQueueAsync(string date)
        {
            var result = await _api.GetAsync<List<QueueEntry>>($"/queue?date={date}");
            var queue = result ?? new List<QueueEntry>();

            // fill doctor names from cache using doctorId
            ResolveDoctorNames(queue);

            return queue;
        }

        public async Task<bool> UpdateStatusAsync(string queueId, string status)
        {
            try
            {
                await _api.PatchAsync<object>($"/queue/{queueId}", new { status });
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
                // skip if already has doctor name
                if (!string.IsNullOrEmpty(entry.DoctorName)) continue;
                if (entry.Doctor != null &&
                    !string.IsNullOrEmpty(entry.Doctor.Name)) continue;

                // find doctor id from any field
                var docId = entry.DisplayDoctorId;
                if (string.IsNullOrEmpty(docId)) continue;

                // look up in cache
                var doctor = doctors.FirstOrDefault(d =>
                    d.Id.Equals(docId, StringComparison.OrdinalIgnoreCase));

                if (doctor != null)
                    entry.ResolvedDoctorName = doctor.Name;
            }
        }
    }
}