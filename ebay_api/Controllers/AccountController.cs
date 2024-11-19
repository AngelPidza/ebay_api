using ebay_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ebay_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration)
        : ControllerBase
    {
        private readonly SignInManager<User> _signInManager = signInManager;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "El email ya está registrado"
                    });
                }

                var user = new User
                {
                    UserName = model.Username,  // Usa UserName de IdentityUser
                    Email = model.Email,
                    RegistrationDate = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = GenerateJwtToken(user);

                    return Ok(new
                    {
                        Status = "Success",
                        Message = "Usuario creado exitosamente!",
                        Token = token
                    });
                }

                var errors = result.Errors.Select(e => new
                {
                    Code = e.Code,
                    Description = TranslateError(e.Description)
                });

                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Error al crear el usuario",
                    Errors = errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Error interno del servidor al registrar el usuario",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginModel model)
        {
            try
            {
                // Buscar usuario por correo electrónico
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Unauthorized(new AuthResponse
                    {
                        Status = "Error",
                        Message = "Usuario o contraseña incorrectos"
                    });
                }

                // Verificar si la cuenta está bloqueada
                if (await userManager.IsLockedOutAsync(user))
                {
                    return Unauthorized(new AuthResponse
                    {
                        Status = "Error",
                        Message = "La cuenta está bloqueada. Intenta nuevamente más tarde."
                    });
                }

                // Validar la contraseña
                var isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
                if (!isPasswordValid)
                {
                    // Registrar un intento fallido y bloquear si es necesario
                    await userManager.AccessFailedAsync(user);

                    return Unauthorized(new AuthResponse
                    {
                        Status = "Error",
                        Message = "Usuario o contraseña incorrectos"
                    });
                }
  
                // Restablecer contador de intentos fallidos si la contraseña es correcta
                await userManager.ResetAccessFailedCountAsync(user);

                // Generar token JWT
                var token = GenerateJwtToken(user);

                return Ok(new AuthResponse
                {
                    Message = "Inicio correcto",
                    Status = "Success",
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        RegistrationDate = user.RegistrationDate
                    }
                });
            }
            catch (Exception ex)
            {
                // Loguear el error para depuración
                Console.WriteLine($"Error: {ex.Message}");

                return StatusCode(500, new AuthResponse
                {
                    Status = "Error",
                    Message = "Error interno del servidor"
                });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("uid", user.Id),
                new Claim("registration_date", user.RegistrationDate.ToString("o"))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string TranslateError(string error)
        {
            return error switch
            {
                var e when e.Contains("PasswordRequiresNonAlphanumeric") =>
                    "La contraseña debe contener al menos un carácter especial",
                var e when e.Contains("PasswordRequiresDigit") =>
                    "La contraseña debe contener al menos un número",
                var e when e.Contains("PasswordRequiresUpper") =>
                    "La contraseña debe contener al menos una letra mayúscula",
                var e when e.Contains("PasswordRequiresLower") =>
                    "La contraseña debe contener al menos una letra minúscula",
                var e when e.Contains("PasswordTooShort") =>
                    "La contraseña debe tener al menos 6 caracteres",
                var e when e.Contains("DuplicateUserName") =>
                    "El nombre de usuario ya está en uso",
                var e when e.Contains("DuplicateEmail") =>
                    "El email ya está registrado",
                _ => error
            };
        }
    }
}