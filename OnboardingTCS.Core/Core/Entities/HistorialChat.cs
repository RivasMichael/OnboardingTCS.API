using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    public class HistorialChat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("usuario_correo")]
        public string UsuarioCorreo { get; set; } = string.Empty;

        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("mensajes")]
        public List<MensajeChat> Mensajes { get; set; } = new List<MensajeChat>();

        [BsonElement("favorito")]
        public bool Favorito { get; set; }

        [BsonElement("ultima_actividad_en")]
        public DateTime UltimaActividadEn { get; set; }

        [BsonElement("total_mensajes")]
        public int TotalMensajes { get; set; }
    }

    public class MensajeChat
    {
        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [BsonElement("contenido")]
        public string Contenido { get; set; } = string.Empty;

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}