using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnboardingTCS.Core.DTOs;
using System.Linq;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _repository;
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioRepository repository, IUsuarioService usuarioService)
        {
            _repository = repository;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// [SOLO ADMIN] Obtener lista completa de usuarios para el panel de administración
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
        {
            try
            {
                var usuarios = await _usuarioService.GetAllUsuariosAsync();
                return Ok(usuarios);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Obtener vista simplificada de usuarios para el panel administrativo
        /// </summary>
        [HttpGet("panel")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsuariosPanel()
        {
            try
            {
                var usuarios = await _usuarioService.GetAllUsuariosAsync();
                
                var usuariosPanel = usuarios.Select(u => new 
                {
                    id = u.Id,
                    nombre = u.Nombre,
                    correo = u.Correo,
                    fechaInicio = u.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    cargo = u.Cargo,
                    departamento = u.Departamento,
                    codigoEmpleado = u.CodigoEmpleado,
                    estado = u.Estado,
                    rol = u.Rol,
                    oficina = u.Oficina
                });

                return Ok(usuariosPanel);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Obtener usuario específico por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UsuarioDto>> GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("El ID del usuario es requerido");

                var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
                if (usuario == null) 
                    return NotFound($"No se encontró un usuario con ID: {id}");
                
                return Ok(usuario);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Crear nuevo usuario en el sistema
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create([FromBody] UsuarioCreateDto usuarioDto)
        {
            try
            {
                if (usuarioDto == null)
                    return BadRequest("Los datos del usuario son requeridos");

                if (string.IsNullOrWhiteSpace(usuarioDto.Nombre))
                    return BadRequest("El nombre es requerido");

                if (string.IsNullOrWhiteSpace(usuarioDto.Correo))
                    return BadRequest("El correo es requerido");

                if (string.IsNullOrWhiteSpace(usuarioDto.Contrasena))
                    return BadRequest("La contraseña es requerida");

                await _usuarioService.InsertUsuarioAsync(usuarioDto);
                return Ok(new { message = "Usuario creado exitosamente" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Actualizar datos de un usuario existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Update(string id, [FromBody] UsuarioUpdateDto usuarioDto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("El ID del usuario es requerido");

                if (usuarioDto == null)
                    return BadRequest("Los datos del usuario son requeridos");

                var existing = await _usuarioService.GetUsuarioByIdAsync(id);
                if (existing == null) 
                    return NotFound($"No se encontró un usuario con ID: {id}");

                await _usuarioService.UpdateUsuarioAsync(id, usuarioDto);
                return Ok(new { message = "Usuario actualizado exitosamente" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Eliminar usuario del sistema
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("El ID del usuario es requerido");

                var existing = await _usuarioService.GetUsuarioByIdAsync(id);
                if (existing == null) 
                    return NotFound($"No se encontró un usuario con ID: {id}");

                await _usuarioService.DeleteUsuarioAsync(id);
                return Ok(new { message = "Usuario eliminado exitosamente" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Obtener detalles completos del usuario
        /// </summary>
        [HttpGet("{id}/detalles")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UsuarioDto>> GetDetalles(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("El ID del usuario es requerido");

                var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
                if (usuario == null) 
                    return NotFound($"No se encontró un usuario con ID: {id}");

                return Ok(usuario);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Obtener perfil completo del usuario para administración
        /// </summary>
        [HttpGet("perfil/{usuarioId}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<PerfilUsuarioDto>> GetPerfil(string usuarioId)
        {
            try
            {
                if (string.IsNullOrEmpty(usuarioId))
                    return BadRequest("El ID del usuario es requerido");

                var perfil = await _usuarioService.GetPerfilUsuarioAsync(usuarioId);
                if (perfil == null) 
                    return NotFound($"No se encontró el usuario con ID: {usuarioId}");
                
                return Ok(perfil);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}