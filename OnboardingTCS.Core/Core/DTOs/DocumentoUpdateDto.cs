namespace OnboardingTCS.Core.DTOs
{
    public class DocumentoUpdateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public bool Obligatorio { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string UrlArchivo { get; set; } = string.Empty;
        public string TipoArchivo { get; set; } = string.Empty;
        public long TamanoArchivo { get; set; }
        public string SubidoPor { get; set; } = string.Empty;
        public string SubidoPorNombre { get; set; } = string.Empty;
    }
}