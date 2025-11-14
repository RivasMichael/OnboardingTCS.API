namespace OnboardingTCS.Core.DTOs
{
    public class MensajesEnviadosCreateDto
    {
        public string RemitenteId { get; set; } = string.Empty;
        public string DestinatarioId { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
    }
}