using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var usuario = await _authRepo.GetUserByEmailAsync(loginRequest.Correo);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            if (usuario.ContrasenaHash != loginRequest.Contrasena)
            {
                return Unauthorized(new { message = "Contraseña incorrecta." });
            }

            var response = new LoginResponse
            {
                Mensaje = "Login exitoso",
                Token = "simulated.jwt.token." + usuario.Id,
                Usuario = usuario
            };
            return Ok(response);
        }
    }
}
