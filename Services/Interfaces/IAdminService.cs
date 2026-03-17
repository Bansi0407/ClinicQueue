using ClinicQueue.Models;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services.Interfaces
{
    public interface IAdminService
    {
        Task<Clinic?> GetClinicAsync();
        Task<List<User>> GetUsersAsync();
        Task<bool> CreateUserAsync(CreateUserVM model);
    }
}