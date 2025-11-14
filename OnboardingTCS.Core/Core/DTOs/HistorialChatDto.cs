using System;
using System.Collections.Generic;

namespace OnboardingTCS.Core.DTOs
{
    public class HistorialChatDto
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioCorreo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public List<MensajeChatDto> Mensajes { get; set; } = new List<MensajeChatDto>();
        public bool Favorito { get; set; }
        public DateTime UltimaActividadEn { get; set; }
        public int TotalMensajes { get; set; }
    }

    public class MensajeChatDto
    {
        public string Tipo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}