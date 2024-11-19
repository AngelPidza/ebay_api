using ebay_api.Models;
using ebay_api.Repositories;
using Microsoft.Identity.Client;

namespace ebay_api.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AccountService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return new AuthenticationResult { ErrorMessage = "Invalid email or password." };

            var principal = await _tokenService.CreatePrincipalAsync(user);
            return new AuthenticationResult { Succeeded = true, Principal = principal };
        }

        public async Task<RegistrationResult> RegisterAsync(string username, string email, string password)
        {
            if (await _userRepository.GetUserByEmailAsync(email) != null)
                return new RegistrationResult { ErrorMessage = "Email already registered." };

            var user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = HashPassword(password),
                RegistrationDate = DateTime.UtcNow
            };

            await _userRepository.CreateUserAsync(user);
            return new RegistrationResult { Succeeded = true };
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // En una aplicación real, usarías una función de hash segura.
            // Este es solo un ejemplo simple.
            return hashedPassword == HashPassword(password);
        }

        private string HashPassword(string password)
        {
            // En una aplicación real, usarías una función de hash segura.
            // Este es solo un ejemplo simple.
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
