namespace ClinicQueue.Services.Interfaces
{
    public interface IApiClient
    {
        Task<T?> GetAsync<T>(string url);
        Task<T?> PostAsync<T>(string url, object data);
        Task<T?> PatchAsync<T>(string url, object data);

        Task<T?> TryGetAsync<T>(string url);
    }
}