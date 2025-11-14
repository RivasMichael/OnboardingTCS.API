namespace OnboardingTCS.Core.DTOs
{
    public class HistorialChatCreateDto
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
    }
}