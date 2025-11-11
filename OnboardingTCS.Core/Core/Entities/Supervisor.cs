using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    public class Supervisor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("correo")]
        public string Correo { get; set; } = string.Empty;

        [BsonElement("cargo")]
        public string Cargo { get; set; } = string.Empty;

        [BsonElement("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [BsonElement("horario")]
        public string Horario { get; set; } = string.Empty;

        [BsonElement("mensaje_bienvenida")]
        public string MensajeBienvenida { get; set; } = string.Empty;

        [BsonElement("departamento")]
        public string Departamento { get; set; } = string.Empty;

        [BsonElement("foto_perfil")]
        public string FotoPerfil { get; set; } = string.Empty;

        [BsonElement("creado_en")]
        public DateTime CreadoEn { get; set; }
    }
}