using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    public class Actividades
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [BsonElement("fecha")]
        public DateTime Fecha { get; set; }

        [BsonElement("hora")]
        public string Hora { get; set; } = string.Empty;

        [BsonElement("duracion")]
        public string Duracion { get; set; } = string.Empty;

        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [BsonElement("modalidad")]
        public string Modalidad { get; set; } = string.Empty;

        [BsonElement("lugar")]
        public string Lugar { get; set; } = string.Empty;

        [BsonElement("asignados")]
        public List<string> Asignados { get; set; } = new List<string>();

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;

        [BsonElement("creado_por")]
        public string CreadoPor { get; set; } = string.Empty;

        [BsonElement("creado_en")]
        public DateTime CreadoEn { get; set; }
    }
}
