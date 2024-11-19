using Microsoft.Identity.Client;

namespace ebay_api.Services
{
    public interface IAccountService
    {
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<RegistrationResult> RegisterAsync(string username, string email, string password);

    }
}
