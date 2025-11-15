namespace OnboardingTCS.Core.DTOs
{
    public class LikesCursosUpdateDto
    {
        public bool MeGusta { get; set; }
        public string UsuarioCorreo { get; set; } = string.Empty;
        public string CursoTitulo { get; set; } = string.Empty;
        public DateTime LikeadoEn { get; set; } = DateTime.UtcNow;
    }
}