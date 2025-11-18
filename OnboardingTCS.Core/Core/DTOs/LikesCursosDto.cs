using System;

namespace OnboardingTCS.Core.Core.DTOs
{
    public class LikesCursosDto
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioCorreo { get; set; } = string.Empty;
        public string CursoTitulo { get; set; } = string.Empty;
        public DateTime LikeadoEn { get; set; }
    }
}
