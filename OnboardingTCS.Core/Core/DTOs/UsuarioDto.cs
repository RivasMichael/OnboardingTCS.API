using System;

namespace OnboardingTCS.Core.DTOs
{
    public class UsuarioDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string CodigoEmpleado { get; set; } = string.Empty;
        public string SupervisorCorreo { get; set; } = string.Empty;
        public string Oficina { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }
    }
}