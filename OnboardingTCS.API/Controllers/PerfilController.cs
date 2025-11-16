using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ? Require authentication for all endpoints
    public class PerfilController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public PerfilController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado (desde JWT)
        /// </summary>
        /// <returns>Perfil del usuario actual</returns>
        [HttpGet("mi-perfil")]
        [Authorize(Roles = "employee,admin")]
        public async Task<ActionResult<PerfilUsuarioDto>> GetMiPerfil()
        {
            try
            {
                // Extract user ID from JWT token (more secure)
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Token inválido");

                var perfil = await _usuarioService.GetPerfilUsuarioAsync(userId);
                
                if (perfil == null)
                    return NotFound("Perfil no encontrado");

                return Ok(perfil);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Obtiene el perfil de cualquier usuario por ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Perfil del usuario especificado</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")] // ? Only admins can view other users' profiles
        public async Task<ActionResult<PerfilUsuarioDto>> GetPerfil(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("El ID del usuario es requerido");

                var perfil = await _usuarioService.GetPerfilUsuarioAsync(id);
                
                if (perfil == null)
                    return NotFound($"No se encontró un usuario con ID: {id}");

                return Ok(perfil);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Obtiene el perfil de un usuario por correo
        /// </summary>
        /// <param name="correo">Correo electrónico del usuario</param>
        /// <returns>Perfil del usuario</returns>
        [HttpGet("por-correo/{correo}")]
        [Authorize(Roles = "admin")] // ? Only admins can search by email
        public async Task<ActionResult<PerfilUsuarioDto>> GetPerfilPorCorreo(string correo)
        {
            try
            {
                if (string.IsNullOrEmpty(correo))
                    return BadRequest("El correo electrónico es requerido");

                var usuarios = await _usuarioService.GetAllUsuariosAsync();
                var usuario = usuarios.FirstOrDefault(u => u.Correo.Equals(correo, System.StringComparison.OrdinalIgnoreCase));
                
                if (usuario == null)
                    return NotFound($"No se encontró un usuario con correo: {correo}");

                var perfil = await _usuarioService.GetPerfilUsuarioAsync(usuario.Id);
                return Ok(perfil);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información del supervisor del usuario autenticado
        /// </summary>
        /// <returns>Información completa de contacto del supervisor</returns>
        [HttpGet("mi-supervisor")]
        [Authorize(Roles = "employee")] // ? Only employees need supervisor info
        public async Task<ActionResult<SupervisorDto>> GetMiSupervisor()
        {
            try
            {
                // Extract user ID from JWT token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Token inválido");

                // Get current user info
                var usuario = await _usuarioService.GetUsuarioByIdAsync(userId);
                
                if (usuario == null)
                    return NotFound("Usuario no encontrado");

                if (string.IsNullOrEmpty(usuario.SupervisorId))
                    return NotFound("No tienes un supervisor asignado");

                // Get supervisor info from Supervisor collection
                var supervisorService = HttpContext.RequestServices.GetRequiredService<ISupervisorService>();
                var supervisor = await supervisorService.GetSupervisorByIdAsync(usuario.SupervisorId);
                
                if (supervisor != null)
                {
                    // Return complete supervisor info from Supervisor collection
                    return Ok(supervisor);
                }
                
                // Fallback: Get supervisor info from Usuario collection
                var supervisorUsuario = await _usuarioService.GetUsuarioByIdAsync(usuario.SupervisorId);
                
                if (supervisorUsuario == null)
                    return NotFound("Información del supervisor no encontrada");

                // Map Usuario to SupervisorDto format
                var supervisorInfo = new SupervisorDto
                {
                    Id = supervisorUsuario.Id,
                    Nombre = supervisorUsuario.Nombre,
                    Correo = supervisorUsuario.Correo,
                    Cargo = supervisorUsuario.Cargo ?? "",
                    Telefono = "+51 999 888 777", // Default value
                    Horario = "Lunes a Viernes, 9:00 AM - 6:00 PM", // Default value
                    MensajeBienvenida = "¡Hola! Soy tu supervisor. Estoy aquí para apoyarte en tu proceso de onboarding y resolver cualquier duda que tengas.",
                    Departamento = supervisorUsuario.Departamento ?? "",
                    FotoPerfil = "" // Default empty
                };

                return Ok(supervisorInfo);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}