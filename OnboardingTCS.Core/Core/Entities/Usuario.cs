using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    [BsonIgnoreExtraElements]
    public class Usuario
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("correo")]
        public string Correo { get; set; } = string.Empty;

        [BsonElement("contrasenaHash")]
        public string ContrasenaHash { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}
