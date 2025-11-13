using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    public class LikesCursos
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("usuario_correo")]
        public string UsuarioCorreo { get; set; } = string.Empty;

        [BsonElement("curso_titulo")]
        public string CursoTitulo { get; set; } = string.Empty;

        [BsonElement("likeado_en")]
        public DateTime LikeadoEn { get; set; }
    }
}
