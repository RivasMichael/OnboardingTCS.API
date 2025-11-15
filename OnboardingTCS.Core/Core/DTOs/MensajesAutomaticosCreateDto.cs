namespace OnboardingTCS.Core.DTOs
{
    public class MensajesAutomaticosCreateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string TipoDisparo { get; set; } = string.Empty;
        public int DiasAntesInicio { get; set; }
        public string RolObjetivo { get; set; } = string.Empty;
        public string CreadoPor { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int? InactividadDias { get; set; }
    }
}