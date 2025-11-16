using System.Collections.Generic;

namespace OnboardingTCS.Core.DTOs
{
    public class CursoCreateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public bool Recomendado { get; set; }
        public List<string> RecomendadoPara { get; set; } = new List<string>();
        public string CreadoPor { get; set; } = string.Empty;
    }
}