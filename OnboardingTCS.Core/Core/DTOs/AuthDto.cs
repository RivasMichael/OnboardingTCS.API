using OnboardingTCS.Core.DTOs;

namespace OnboardingTCS.Core.Core.DTOs
{
    public class SignInRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }

    public class SignInResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public LoginUsuarioDto Usuario { get; set; } = new();
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO minimalista para login - Solo los campos esenciales
    /// </summary>
    public class LoginUsuarioDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO público del usuario - Campos adicionales para otras funciones
    /// </summary>
    public class UsuarioPublicDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string CodigoEmpleado { get; set; } = string.Empty;
        public string Oficina { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        
        // Solo incluir supervisor info si es relevante para el usuario
        public string SupervisorNombre { get; set; } = string.Empty;
        public string SupervisorCorreo { get; set; } = string.Empty;
    }
}