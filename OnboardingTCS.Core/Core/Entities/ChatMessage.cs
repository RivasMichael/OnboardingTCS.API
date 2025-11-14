using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnboardingTCS.Core.Entities
{
    public class ChatMessage
    {
        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty; // 'usuario' | 'bot'

        [BsonElement("contenido")]
        public string Contenido { get; set; } = string.Empty;

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
