namespace OnboardingTCS.Core.DTOs
{
    public class LikesCursosCreateDto
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string CursoId { get; set; } = string.Empty;
        public bool MeGusta { get; set; }
        public string UsuarioCorreo { get; set; } = string.Empty;
        public string CursoTitulo { get; set; } = string.Empty;
        public DateTime LikeadoEn { get; set; } = DateTime.UtcNow;
    }
}