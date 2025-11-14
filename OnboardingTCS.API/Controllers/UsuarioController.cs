using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _repository;
        private readonly IConfiguration _configuration;

        public UsuarioController(UsuarioRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
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

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest("Correo y contraseña son requeridos.");

            var usuario = await _repository.GetByEmailAsync(request.Correo);
            if (usuario == null || !VerifyPassword(request.Contrasena, usuario.Contrasena))
                return Unauthorized("Credenciales inválidas.");

            var token = GenerateJwtToken(usuario);
            return Ok(new { Token = token });
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            // Aquí puedes usar BCrypt para comparar contraseñas encriptadas
            return inputPassword == storedPassword; // Simplificado para ejemplo
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var claims = new[]
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, usuario.Id),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
                new System.Security.Claims.Claim("rol", usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class SignInRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}