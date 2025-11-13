using System;

namespace OnboardingTCS.Core.DTOs
{
    public class MensajesEnviadosDto
    {
        public string Id { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public bool Leido { get; set; }
        public DateTime? LeidoEn { get; set; }
        public bool Favorito { get; set; }
        public DateTime EnviadoEn { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}
