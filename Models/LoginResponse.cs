namespace ClinicQueue.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public UserInfo User { get; set; } = new();
    }

    public class UserInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string? ClinicName { get; set; }
    }
}