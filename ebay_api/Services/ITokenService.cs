using ebay_api.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ebay_api.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(User user);
        Task<ClaimsPrincipal> CreatePrincipalAsync(User user);
    }
}
