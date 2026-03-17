using ClinicQueue.Models;
using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services
{
    public class AdminService : IAdminService
    {
        private readonly IApiClient _api;
        private readonly IDoctorsCacheService _cache;

        public AdminService(IApiClient api, IDoctorsCacheService cache)
        {
            _api = api;
            _cache = cache;
        }

        public async Task<Clinic?> GetClinicAsync()
        {
            return await _api.GetAsync<Clinic>("/admin/clinic");
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var result = await _api.GetAsync<List<User>>("/admin/users");
            var users = result ?? new List<User>();

            // store doctors in cache so patients can use it
            if (users.Any())
                _cache.Store(users);

            return users;
        }

        public async Task<bool> CreateUserAsync(CreateUserVM model)
        {
            var payload = new
            {
                name = model.Name,
                email = model.Email,
                password = model.Password,
                role = model.Role,
                phone = model.Phone
            };

            await _api.PostAsync<object>("/admin/users", payload);

            // refresh cache after new user created
            try
            {
                var updated = await _api.GetAsync<List<User>>("/admin/users");
                if (updated != null && updated.Any())
                    _cache.Store(updated);
            }
            catch
            {
                // cache refresh failed — not critical
            }

            return true;
        }
    }
}




/*using ClinicQueue.Models;
using ClinicQueue.Services.Interfaces;
using ClinicQueue.ViewModels;

namespace ClinicQueue.Services
{
    public class AdminService : IAdminService
    {
        private readonly IApiClient _api;

        public AdminService(IApiClient api)
        {
            _api = api;
        }

        public async Task<Clinic?> GetClinicAsync()
        {
            return await _api.GetAsync<Clinic>("/admin/clinic");
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var result = await _api.GetAsync<List<User>>("/admin/users");
            return result ?? new List<User>();
        }

        public async Task<bool> CreateUserAsync(CreateUserVM model)
        {
            var payload = new
            {
                name = model.Name,
                email = model.Email,
                password = model.Password,
                role = model.Role,
                phone = model.Phone
            };
            try
            {
                var result = await _api.PostAsync<object>("/admin/users", payload);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}*/

