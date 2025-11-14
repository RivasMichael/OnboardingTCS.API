using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// --- ¡NUEVO! Añade estos 'usings' ---
using System.Text.Json;
using System.Text.Json.Serialization;
// --- Fin de los 'usings' nuevos ---

namespace OnboardingTCS.Core.Entities
{
    [BsonIgnoreExtraElements]
    public class HistorialChat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Esto es para MongoDB
        [JsonConverter(typeof(ObjectIdConverter))] // <-- ¡ESTA ES LA LÍNEA MÁGICA! (para JSON)
        public ObjectId Id { get; set; }

        [BsonElement("usuario_correo")]
        public string UsuarioCorreo { get; set; } = string.Empty;

        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("mensajes")]
        public List<ChatMessage> Mensajes { get; set; } = new();

        [BsonElement("favorito")]
        public bool Favorito { get; set; }

        [BsonElement("ultima_actividad_en")]
        public DateTime UltimaActividadEn { get; set; }

        [BsonElement("total_mensajes")]
        public int TotalMensajes { get; set; }
    }


    // --- ¡NUEVO! Pega esta clase "Mágica" al final de tu archivo ---
    // (Dentro del 'namespace', pero fuera de la clase 'HistorialChat')
    //
    // Esta clase le enseña a C# cómo
    // convertir un ObjectId a un string y viceversa.
    public class ObjectIdConverter : JsonConverter<ObjectId>
    {
        public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            if (ObjectId.TryParse(stringValue, out var objectId))
            {
                return objectId;
            }
            return ObjectId.Empty; // O maneja el error
        }

        public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
