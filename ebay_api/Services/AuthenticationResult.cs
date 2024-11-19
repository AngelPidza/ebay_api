namespace ebay_api.Services
{
    public class AuthenticationResult
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public System.Security.Claims.ClaimsPrincipal? Principal { get; set; }
    }
}
