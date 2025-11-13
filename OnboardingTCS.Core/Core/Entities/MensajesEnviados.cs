using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    public class MensajesEnviados
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [BsonElement("destinatario")]
        public string Destinatario { get; set; } = string.Empty;

        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("contenido")]
        public string Contenido { get; set; } = string.Empty;

        [BsonElement("prioridad")]
        public string Prioridad { get; set; } = string.Empty;

        [BsonElement("categoria")]
        public string Categoria { get; set; } = string.Empty;

        [BsonElement("leido")]
        public bool Leido { get; set; }

        [BsonElement("leido_en")]
        public DateTime? LeidoEn { get; set; }

        [BsonElement("favorito")]
        public bool Favorito { get; set; }

        [BsonElement("enviado_en")]
        public DateTime EnviadoEn { get; set; }

        [BsonElement("creado_en")]
        public DateTime CreadoEn { get; set; }
    }
}
