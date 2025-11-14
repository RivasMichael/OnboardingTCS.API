namespace OnboardingTCS.Core.DTOs
{
    public class CursoCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int DuracionHoras { get; set; }
    }
}