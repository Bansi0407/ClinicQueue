using ClinicQueue.Models;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginVM model);
    }
}