namespace OnboardingTCS.Core.DTOs
{
    public class DocumentoDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Archivo { get; set; } = string.Empty;
        public bool Requerido { get; set; }
    }
}