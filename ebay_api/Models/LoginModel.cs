using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ebay_api.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
