using ClinicQueue.Models;
using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApiClient _api;

        public AuthService(IApiClient api)
        {
            _api = api;
        }

        public async Task<LoginResponse?> LoginAsync(LoginVM model)
        {
            var payload = new { email = model.Email, password = model.Password };
            return await _api.PostAsync<LoginResponse>("/auth/login", payload);
        }
    }
}