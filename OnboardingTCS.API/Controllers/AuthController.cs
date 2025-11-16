using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Core.DTOs;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using System;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Iniciar sesión con email y contraseña
        /// </summary>
        /// <param name="request">Datos de inicio de sesión</param>
        /// <returns>Token JWT y información del usuario</returns>
        [HttpPost("signin")]
        public async Task<ActionResult<SignInResponse>> SignIn([FromBody] SignInRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Los datos de inicio de sesión son requeridos");

                if (string.IsNullOrWhiteSpace(request.Correo))
                    return BadRequest("El correo electrónico es requerido");

                if (string.IsNullOrWhiteSpace(request.Contrasena))
                    return BadRequest("La contraseña es requerida");

                var result = await _usuarioService.SignInAsync(request);
                
                if (result == null)
                    return Unauthorized("Credenciales inválidas");

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Renovar token usando refresh token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>Nuevos tokens</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult<SignInResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                    return BadRequest("Refresh token es requerido");

                var result = await _usuarioService.RefreshTokenAsync(request);
                
                if (result == null)
                    return Unauthorized("Refresh token inválido");

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtener información del usuario autenticado y su rol
        /// </summary>
        /// <returns>Información del usuario actual con permisos</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserRoleInfo>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Token inválido");

                var usuario = await _usuarioService.GetUsuarioByIdAsync(userId);
                
                if (usuario == null)
                    return NotFound("Usuario no encontrado");

                var userInfo = new UserRoleInfo 
                { 
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Rol = usuario.Rol,
                    Cargo = usuario.Cargo,
                    Departamento = usuario.Departamento,
                    Estado = usuario.Estado,
                    EsAdmin = usuario.Rol == "admin",
                    EsEmployee = usuario.Rol == "employee",
                    EsSupervisor = usuario.Rol == "supervisor"
                };

                return Ok(userInfo);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Cerrar sesión (invalidar token)
        /// </summary>
        /// <returns>Confirmación de cierre de sesión</returns>
        [HttpPost("signout")]
        [Authorize]
        public async Task<ActionResult> SignOut()
        {
            try
            {
                // En una implementación completa, aquí agregarías el token a una lista negra
                // o eliminarías el refresh token de la base de datos
                await Task.CompletedTask;
                
                return Ok(new { message = "Sesión cerrada exitosamente" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// ENDPOINT TEMPORAL: Resetear contraseña de usuario (solo para desarrollo)
        /// </summary>
        /// <param name="request">Email y nueva contraseña</param>
        /// <returns>Confirmación de cambio</returns>
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Correo))
                    return BadRequest("El correo es requerido");

                if (string.IsNullOrWhiteSpace(request.NuevaContrasena))
                    return BadRequest("La nueva contraseña es requerida");

                var usuarios = await _usuarioService.GetAllUsuariosAsync();
                var usuario = usuarios.FirstOrDefault(u => u.Correo.Equals(request.Correo, StringComparison.OrdinalIgnoreCase));
                
                if (usuario == null)
                    return NotFound("Usuario no encontrado");

                // Crear un DTO de actualización con solo la contraseña
                var updateDto = new UsuarioUpdateDto
                {
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Contrasena = request.NuevaContrasena, // Esta se hasheará automáticamente
                    Rol = usuario.Rol,
                    FechaInicio = usuario.FechaInicio,
                    Cargo = usuario.Cargo,
                    Departamento = usuario.Departamento,
                    CodigoEmpleado = usuario.CodigoEmpleado,
                    SupervisorId = usuario.SupervisorId,
                    SupervisorCorreo = usuario.SupervisorCorreo,
                    Oficina = usuario.Oficina,
                    Estado = usuario.Estado
                };

                await _usuarioService.UpdateUsuarioAsync(usuario.Id, updateDto);

                return Ok(new { message = $"Contraseña actualizada para {request.Correo}" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        public class ResetPasswordRequest
        {
            public string Correo { get; set; } = string.Empty;
            public string NuevaContrasena { get; set; } = string.Empty;
        }
    }

    /// <summary>
    /// Información del usuario con permisos de rol
    /// </summary>
    public class UserRoleInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool EsAdmin { get; set; }
        public bool EsEmployee { get; set; }
        public bool EsSupervisor { get; set; }
    }
}