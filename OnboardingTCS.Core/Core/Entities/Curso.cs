using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    public class Curso
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [BsonElement("duracion")]
        public string Duracion { get; set; } = string.Empty;

        [BsonElement("nivel")]
        public string Nivel { get; set; } = string.Empty;

        [BsonElement("categoria")]
        public string Categoria { get; set; } = string.Empty;

        [BsonElement("instructor")]
        public string Instructor { get; set; } = string.Empty;

        [BsonElement("link")]
        public string Link { get; set; } = string.Empty;

        [BsonElement("recomendado")]
        public bool Recomendado { get; set; }

        [BsonElement("recomendado_para")]
        public List<string> RecomendadoPara { get; set; } = new List<string>();

        [BsonElement("creado_por")]
        public string CreadoPor { get; set; } = string.Empty;

        [BsonElement("activo")]
        public bool Activo { get; set; }

        [BsonElement("inscritos")]
        public int Inscritos { get; set; }
    }
}