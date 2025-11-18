namespace OnboardingTCS.Core.Core.DTOs
{
    /// <summary>
    /// DTO para listar documentos con campos reducidos para mejor rendimiento
    /// </summary>
    public class DocumentoListDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public long TamanoArchivo { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}