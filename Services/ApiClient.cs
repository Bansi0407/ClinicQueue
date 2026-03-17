using System.Net.Http.Headers;
using System.Text;
using ClinicQueue.Services.Interfaces;
using Newtonsoft.Json;

namespace ClinicQueue.Services
{
    public class ApiClient : IApiClient
    {
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _ctx;

        public const string TOKEN_KEY = "jwt_token";

        public ApiClient(IHttpClientFactory factory, IHttpContextAccessor ctx)
        {
            _factory = factory;
            _ctx = ctx;
        }

        private HttpClient GetClient()
        {
            var client = _factory.CreateClient("API");
            var token = _ctx.HttpContext?.Session.GetString(TOKEN_KEY);
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private async Task<T?> ReadResponse<T>(HttpResponseMessage res)
        {
            var body = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(body)) return default;
                try
                {
                    return JsonConvert.DeserializeObject<T>(body);
                }
                catch
                {
                    return default;
                }
            }

            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _ctx.HttpContext?.Session.Clear();
                throw new UnauthorizedAccessException("Session expired. Please login again.");
            }

            
            var errorMsg = BuildErrorMessage((int)res.StatusCode, body);
            throw new Exception(errorMsg);
        }

        private static string BuildErrorMessage(int statusCode, string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return $"API Error {statusCode}: No message returned.";

            try
            {
                dynamic? json = JsonConvert.DeserializeObject(body);

                
                var parts = new List<string>();

                if (json?.message != null) parts.Add((string)json.message);
                if (json?.error != null) parts.Add((string)json.error);
                if (json?.title != null) parts.Add((string)json.title);

                
                if (json?.errors != null)
                {
                    string errStr = JsonConvert.SerializeObject(json.errors);
                    parts.Add($"Validation: {errStr}");
                }

                if (parts.Any())
                    return string.Join(" | ", parts);

               
                return body.Length > 400 ? body.Substring(0, 400) : body;
            }
            catch
            {
                return body.Length > 400 ? body.Substring(0, 400) : body;
            }
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var res = await GetClient().GetAsync(url);
            return await ReadResponse<T>(res);
        }

        public async Task<T?> TryGetAsync<T>(string url)
        {
            try
            {
                var res = await GetClient().GetAsync(url);
                if (!res.IsSuccessStatusCode) return default;
                var body = await res.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(body)) return default;
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch
            {
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await GetClient().PostAsync(url, content);
            return await ReadResponse<T>(res);
        }

        public async Task<T?> PatchAsync<T>(string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = content
            };
            var res = await GetClient().SendAsync(req);
            return await ReadResponse<T>(res);
        }
    }
}




/*using System.Net.Http.Headers;
using System.Text;
using ClinicQueue.Services.Interfaces;
using Newtonsoft.Json;

namespace ClinicQueue.Services
{
    public class ApiClient : IApiClient
    {
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _ctx;

        public const string TOKEN_KEY = "jwt_token";

        public ApiClient(IHttpClientFactory factory, IHttpContextAccessor ctx)
        {
            _factory = factory;
            _ctx = ctx;
        }

        private HttpClient GetClient()
        {
            var client = _factory.CreateClient("API");
            var token = _ctx.HttpContext?.Session.GetString(TOKEN_KEY);
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private async Task<T?> ReadResponse<T>(HttpResponseMessage res)
        {
            var body = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(body)) return default;
                try
                {
                    return JsonConvert.DeserializeObject<T>(body);
                }
                catch
                {
                    return default;
                }
            }

            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _ctx.HttpContext?.Session.Clear();
                throw new UnauthorizedAccessException("Session expired. Please login again.");
            }

            var errorMsg = TryExtractErrorMessage(body);
            throw new Exception(errorMsg);
        }

        private static string TryExtractErrorMessage(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return "API returned an error with no message.";

            try
            {
                dynamic? json = JsonConvert.DeserializeObject(body);

                if (json?.message != null) return (string)json.message;
                if (json?.error != null) return (string)json.error;
                if (json?.errors != null) return JsonConvert.SerializeObject(json.errors);

                return body.Length > 300 ? body.Substring(0, 300) : body;
            }
            catch
            {
                return body.Length > 300 ? body.Substring(0, 300) : body;
            }
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var res = await GetClient().GetAsync(url);
            return await ReadResponse<T>(res);
        }

        // safe get — never throws, returns null/empty on any error
        public async Task<T?> TryGetAsync<T>(string url)
        {
            try
            {
                var res = await GetClient().GetAsync(url);
                if (!res.IsSuccessStatusCode) return default;
                var body = await res.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(body)) return default;
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch
            {
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await GetClient().PostAsync(url, content);
            return await ReadResponse<T>(res);
        }

        public async Task<T?> PatchAsync<T>(string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = content
            };
            var res = await GetClient().SendAsync(req);
            return await ReadResponse<T>(res);
        }
    }
}*/
