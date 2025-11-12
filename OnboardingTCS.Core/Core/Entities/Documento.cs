using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    public class Documento
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [BsonElement("categoria")]
        public string Categoria { get; set; } = string.Empty;

        [BsonElement("nombre_archivo")]
        public string NombreArchivo { get; set; } = string.Empty;

        [BsonElement("url_archivo")]
        public string UrlArchivo { get; set; } = string.Empty;

        [BsonElement("tipo_archivo")]
        public string TipoArchivo { get; set; } = string.Empty;

        [BsonElement("tamano_archivo")]
        public long TamanoArchivo { get; set; }

        [BsonElement("obligatorio")]
        public bool Obligatorio { get; set; }

        [BsonElement("subido_por")]
        public string SubidoPor { get; set; } = string.Empty;

        [BsonElement("subido_por_nombre")]
        public string SubidoPorNombre { get; set; } = string.Empty;

        [BsonElement("visible_todos")]
        public bool VisibleTodos { get; set; }

        [BsonElement("descargas")]
        public int Descargas { get; set; }

        [BsonElement("creado_en")]
        public DateTime CreadoEn { get; set; }
    }
}