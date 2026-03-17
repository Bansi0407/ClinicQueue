using ClinicQueue.Models;

namespace ClinicQueue.Services.Interfaces
{
    public interface IDoctorsCacheService
    {
        void Store(List<User> users);
        List<User> GetDoctors();
        bool HasDoctors();
    }
}