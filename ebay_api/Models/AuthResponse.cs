namespace ebay_api.Models
{
    public class AuthResponse
    {
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? Token { get; set; }
        public UserDto? User { get; set; }
    }
}
