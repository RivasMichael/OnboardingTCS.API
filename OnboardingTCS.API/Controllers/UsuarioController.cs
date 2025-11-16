using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnboardingTCS.Core.DTOs;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
        {
            var usuarios = await _repository.GetAllAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetById(string id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Usuario usuario)
        {
            await _repository.CreateAsync(usuario);
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Usuario usuario)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            usuario.Id = id;
            await _repository.UpdateAsync(id, usuario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/detalles")]
        public async Task<ActionResult<UsuarioDto>> GetDetalles(string id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            var usuarioDto = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Cargo = usuario.Cargo,
                MensajeBienvenida = "¡Bienvenido/a al equipo! Estoy aquí para apoyarte en tu proceso de integración. No dudes en contactarme si tienes alguna pregunta.",
                Telefono = "+51 999 888 777",
                HorarioAtencion = "Lunes a Viernes, 9:00 AM - 6:00 PM"
            };

            return Ok(usuarioDto);
        }

        [HttpGet("perfil/{usuarioId}")]
        public async Task<ActionResult<PerfilUsuarioDto>> GetPerfil(string usuarioId)
        {
            var perfil = await _usuarioService.GetPerfilUsuarioAsync(usuarioId);
            if (perfil == null) 
                return NotFound($"No se encontró el usuario con ID: {usuarioId}");
            
            return Ok(perfil);
        }
    }
}