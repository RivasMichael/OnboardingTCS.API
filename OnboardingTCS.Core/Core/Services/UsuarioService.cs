using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Core.DTOs;
using OnboardingTCS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace OnboardingTCS.Core.Core.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IJWTService _jwtService;

        public UsuarioService(IUsuarioRepository repository, IJWTService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync()
        {
            var usuarios = await _repository.GetAllAsync();
            return usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Correo = u.Correo,
                Rol = u.Rol,
                FechaInicio = u.FechaInicio,
                Cargo = u.Cargo,
                Departamento = u.Departamento,
                CodigoEmpleado = u.CodigoEmpleado,
                SupervisorId = u.SupervisorId,
                SupervisorCorreo = u.SupervisorCorreo,
                Oficina = u.Oficina,
                Estado = u.Estado,
                CreadoEn = u.CreadoEn
            });
        }

        public async Task<UsuarioDto> GetUsuarioByIdAsync(string id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return null;

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                FechaInicio = usuario.FechaInicio,
                Cargo = usuario.Cargo,
                Departamento = usuario.Departamento,
                CodigoEmpleado = usuario.CodigoEmpleado,
                SupervisorId = usuario.SupervisorId,
                SupervisorCorreo = usuario.SupervisorCorreo,
                Oficina = usuario.Oficina,
                Estado = usuario.Estado,
                CreadoEn = usuario.CreadoEn
            };
        }

        public async Task InsertUsuarioAsync(UsuarioCreateDto dto)
        {
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Contrasena = HashPassword(dto.Contrasena), // Hash the password with BCrypt
                Rol = dto.Rol,
                FechaInicio = dto.FechaInicio,
                Cargo = dto.Cargo,
                Departamento = dto.Departamento,
                CodigoEmpleado = dto.CodigoEmpleado,
                SupervisorId = dto.SupervisorId,
                SupervisorCorreo = dto.SupervisorCorreo,
                Oficina = dto.Oficina,
                Estado = dto.Estado,
                CreadoEn = DateTime.UtcNow
            };

            await _repository.CreateAsync(usuario);
        }

        public async Task UpdateUsuarioAsync(string id, UsuarioUpdateDto dto)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return;

            usuario.Nombre = dto.Nombre;
            usuario.Correo = dto.Correo;
            
            // Only hash password if it's being changed
            if (!string.IsNullOrEmpty(dto.Contrasena))
            {
                usuario.Contrasena = HashPassword(dto.Contrasena);
            }
            
            usuario.Rol = dto.Rol;
            usuario.FechaInicio = dto.FechaInicio;
            usuario.Cargo = dto.Cargo;
            usuario.Departamento = dto.Departamento;
            usuario.CodigoEmpleado = dto.CodigoEmpleado;
            usuario.SupervisorId = dto.SupervisorId;
            usuario.SupervisorCorreo = dto.SupervisorCorreo;
            usuario.Oficina = dto.Oficina;
            usuario.Estado = dto.Estado;

            await _repository.UpdateAsync(id, usuario);
        }

        public async Task DeleteUsuarioAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<PerfilUsuarioDto> GetPerfilUsuarioAsync(string id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return null;

            // Formatear la fecha en español
            var cultura = new CultureInfo("es-ES");
            var fechaTexto = usuario.FechaInicio.ToString("d 'de' MMMM 'de' yyyy", cultura);

            var perfil = new PerfilUsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Cargo = usuario.Cargo,
                Departamento = usuario.Departamento,
                CodigoEmpleado = usuario.CodigoEmpleado,
                FechaInicioTexto = fechaTexto,
                Estado = usuario.Estado,
                Oficina = usuario.Oficina,
                Rol = usuario.Rol
            };

            // Obtener información del supervisor si existe
            if (!string.IsNullOrEmpty(usuario.SupervisorId))
            {
                var supervisor = await GetSupervisorInfoAsync(usuario.SupervisorId);
                perfil.Supervisor = supervisor;
            }

            return perfil;
        }

        // Método helper para obtener información del supervisor
        private async Task<SupervisorInfoDto?> GetSupervisorInfoAsync(string supervisorId)
        {
            try
            {
                // Buscar en la colección de usuarios que tenga rol "admin" o "supervisor" 
                // y que coincida con el ID del supervisor
                var supervisorUsuario = await _repository.GetByIdAsync(supervisorId);
                
                if (supervisorUsuario != null && (supervisorUsuario.Rol == "admin" || supervisorUsuario.Rol == "supervisor"))
                {
                    return new SupervisorInfoDto
                    {
                        Id = supervisorUsuario.Id,
                        Nombre = supervisorUsuario.Nombre,
                        Correo = supervisorUsuario.Correo,
                        Departamento = supervisorUsuario.Departamento,
                        MensajeBienvenida = "¡Hola! Soy tu supervisor. Estoy aquí para apoyarte en tu proceso de onboarding y cualquier duda que tengas.",
                        Telefono = "+51 999 888 777",
                        HorarioAtencion = "Lunes a Viernes, 9:00 AM - 6:00 PM"
                    };
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        // Authentication methods
        public async Task<SignInResponse?> SignInAsync(SignInRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Contrasena))
                return null;

            // Add debugging logs
            Console.WriteLine($"DEBUG: Buscando usuario con correo: '{request.Correo}'");
            
            var usuario = await _repository.GetByEmailAsync(request.Correo);
            
            if (usuario == null)
            {
                Console.WriteLine($"DEBUG: Usuario no encontrado con correo: '{request.Correo}'");
                return null;
            }
            
            Console.WriteLine($"DEBUG: Usuario encontrado. Correo en BD: '{usuario.Correo}', Contraseña en BD: '{usuario.Contrasena}'");
            Console.WriteLine($"DEBUG: Contraseña enviada: '{request.Contrasena}'");
            
            var passwordValid = await ValidatePasswordAsync(request.Correo, request.Contrasena);
            Console.WriteLine($"DEBUG: Validación de contraseña: {passwordValid}");
            
            if (!passwordValid)
                return null;

            var token = _jwtService.GenerateJwtToken(usuario);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var usuarioDto = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                Cargo = usuario.Cargo,
                Departamento = usuario.Departamento,
                CodigoEmpleado = usuario.CodigoEmpleado,
                SupervisorId = usuario.SupervisorId,
                SupervisorCorreo = usuario.SupervisorCorreo,
                Oficina = usuario.Oficina,
                Estado = usuario.Estado,
                FechaInicio = usuario.FechaInicio,
                CreadoEn = usuario.CreadoEn
            };

            return new SignInResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24), // This should match JWT settings
                Usuario = usuarioDto
            };
        }

        public async Task<SignInResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            // For simplicity, we'll just validate the refresh token format
            // In a real app, you'd store refresh tokens in the database
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return null;

            // Here you would typically:
            // 1. Validate the refresh token exists in the database
            // 2. Check if it's not expired
            // 3. Get the associated user
            // 4. Generate new tokens
            
            // For now, returning null as refresh token storage is not implemented
            await Task.CompletedTask;
            return null;
        }

        public async Task<bool> ValidatePasswordAsync(string email, string password)
        {
            var usuario = await _repository.GetByEmailAsync(email);
            if (usuario == null)
                return false;

            return VerifyPassword(password, usuario.Contrasena);
        }

        #region Password Hashing Helpers
        
        private string HashPassword(string password)
        {
            // Use BCrypt for secure password hashing
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            try
            {
                // Handle both BCrypt hashes and plain text passwords for compatibility
                if (storedPassword.StartsWith("$2a$") || storedPassword.StartsWith("$2b$") || storedPassword.StartsWith("$2y$"))
                {
                    // This is a BCrypt hash - verify with BCrypt
                    Console.WriteLine($"DEBUG: Verificando con BCrypt. Hash: {storedPassword}");
                    return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);
                }
                else
                {
                    // This is plain text - direct comparison (for backward compatibility)
                    Console.WriteLine($"DEBUG: Verificando con texto plano. Stored: '{storedPassword}', Input: '{inputPassword}'");
                    return string.Equals(inputPassword.Trim(), storedPassword.Trim(), StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Error al verificar contraseña: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}