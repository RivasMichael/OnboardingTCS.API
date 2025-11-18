namespace OnboardingTCS.Core.Core.DTOs
{
    public class DocumentoUpdateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
        public string UrlArchivo { get; set; } = string.Empty;
        public string TipoArchivo { get; set; } = string.Empty;
        public long TamanoArchivo { get; set; }
        public bool Obligatorio { get; set; }
        public string SubidoPor { get; set; } = string.Empty;
        public string SubidoPorNombre { get; set; } = string.Empty;
        public bool VisibleTodos { get; set; }
        public int Descargas { get; set; }
        public string Archivo { get; set; } = string.Empty;
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}