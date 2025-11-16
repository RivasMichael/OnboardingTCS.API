using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("correo")]
        public string Correo { get; set; } = string.Empty;

        [BsonElement("contrasena")]
        public string Contrasena { get; set; } = string.Empty;

        [BsonElement("rol")]
        public string Rol { get; set; } = string.Empty;

        [BsonElement("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [BsonElement("cargo")]
        public string Cargo { get; set; } = string.Empty;

        [BsonElement("departamento")]
        public string Departamento { get; set; } = string.Empty;

        [BsonElement("codigo_empleado")]
        public string CodigoEmpleado { get; set; } = string.Empty;

        [BsonElement("supervisor_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SupervisorId { get; set; } = string.Empty;

        [BsonElement("supervisor_correo")]
        public string SupervisorCorreo { get; set; } = string.Empty;

        [BsonElement("oficina")]
        public string Oficina { get; set; } = string.Empty;

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;

        [BsonElement("creado_en")]
        public DateTime CreadoEn { get; set; }
    }
}