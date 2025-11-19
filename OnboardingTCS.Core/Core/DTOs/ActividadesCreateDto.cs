namespace OnboardingTCS.Core.DTOs
{
    public class ActividadesCreateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Hora { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty; // Cambiado a string para coincidir con el servicio
        public string Tipo { get; set; } = string.Empty;
        public string Modalidad { get; set; } = string.Empty;
        public string Lugar { get; set; } = string.Empty;
        public List<string> Asignados { get; set; } = new List<string>();
        public string Estado { get; set; } = string.Empty;
        public string CreadoPor { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; } // ? Agregado para fechas
    }
}