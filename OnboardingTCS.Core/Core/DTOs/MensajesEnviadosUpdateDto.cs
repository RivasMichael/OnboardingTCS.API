namespace OnboardingTCS.Core.DTOs
{
    public class MensajesEnviadosUpdateDto
    {
        public string Mensaje { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public bool Leido { get; set; }
        public bool Favorito { get; set; }
    }
}