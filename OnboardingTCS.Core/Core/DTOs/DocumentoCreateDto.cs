namespace OnboardingTCS.Core.DTOs
{
    public class DocumentoCreateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
        public string TipoArchivo { get; set; } = string.Empty;
        public long TamanoArchivo { get; set; }
        public bool Obligatorio { get; set; }
        public string SubidoPor { get; set; } = string.Empty;
        public string SubidoPorNombre { get; set; } = string.Empty;
        public string UrlArchivo { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }
    }
}