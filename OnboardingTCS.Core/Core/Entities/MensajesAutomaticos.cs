using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    [BsonIgnoreExtraElements]
    public class MensajesAutomaticos
    {
        // Remove BsonRepresentation(BsonType.ObjectId) because some documents use non-ObjectId _id values
        [BsonId]
        public ObjectId Id { get; set; }


        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("contenido")]
        public string Contenido { get; set; } = string.Empty;

        [BsonElement("prioridad")]
        public string Prioridad { get; set; } = string.Empty;

        [BsonElement("categoria")]
        public string Categoria { get; set; } = string.Empty;

        [BsonElement("tipo_disparo")]
        public string TipoDisparo { get; set; } = string.Empty;

        [BsonElement("dias_antes_inicio")]
        public int DiasAntesInicio { get; set; }

        [BsonElement("rol_objetivo")]
        public string RolObjetivo { get; set; } = string.Empty;

        [BsonElement("creado_por")]
        public string CreadoPor { get; set; } = string.Empty;

        [BsonElement("activo")]
        public bool Activo { get; set; }

        [BsonElement("creado_en")]
        public DateTime CreadoEn { get; set; }

        // Campo que existe en la colección MongoDB pero no estaba definido en la entidad
        [BsonElement("inactividad_dias")]
        public int? InactividadDias { get; set; }
    }
}
