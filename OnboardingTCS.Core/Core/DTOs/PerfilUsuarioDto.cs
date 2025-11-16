using System;
using System.Collections.Generic;
using OnboardingTCS.Core.Entities;

namespace OnboardingTCS.Core.DTOs
{
    public class PerfilUsuarioDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string CodigoEmpleado { get; set; } = string.Empty;
        public string FechaInicioTexto { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Oficina { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        
        // Información del supervisor
        public SupervisorInfoDto? Supervisor { get; set; }
    }

    public class SupervisorInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string MensajeBienvenida { get; set; } = "¡Hola! Soy tu supervisor. Estoy aquí para apoyarte en tu proceso de onboarding.";
        public string Telefono { get; set; } = "+51 999 888 777";
        public string HorarioAtencion { get; set; } = "Lunes a Viernes, 9:00 AM - 6:00 PM";
    }
}