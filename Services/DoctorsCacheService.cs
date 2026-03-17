using ClinicQueue.Models;
using ClinicQueue.Services.Interfaces;

namespace ClinicQueue.Services
{
    // singleton — lives for whole app lifetime
    public class DoctorsCacheService : IDoctorsCacheService
    {
        private List<User> _doctors = new();

        // called when admin loads users list
        public void Store(List<User> users)
        {
            _doctors = users
                .Where(u => u.Role.Equals("doctor", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<User> GetDoctors() => _doctors;

        public bool HasDoctors() => _doctors.Any();
    }
}