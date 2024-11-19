using ebay_api.Models;

namespace ebay_api.Services
{
    public class RegistrationResult
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public User? User { get; set; }
    }
}
