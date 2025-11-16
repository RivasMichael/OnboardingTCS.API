using System;
using System.Collections.Generic;

namespace OnboardingTCS.Core.DTOs
{
    public class HistorialChatUpdateDto
    {
        public string UsuarioCorreo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public List<MensajeChatUpdateDto>? Mensajes { get; set; }
        public bool Favorito { get; set; }
    }

    public class MensajeChatUpdateDto
    {
        public string Tipo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}