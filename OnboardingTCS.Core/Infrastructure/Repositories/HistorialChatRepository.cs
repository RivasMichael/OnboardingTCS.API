using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using System;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class HistorialChatRepository : IHistorialChatRepository
    {
        private readonly IMongoCollection<HistorialChat> _historial;

        public HistorialChatRepository(MongoDbContext context)
        {
            _historial = context.historial_chat;
        }

        public async Task<IEnumerable<HistorialChat>> GetAllAsync()
        {
            // Read raw documents and map to model to tolerate older formats where mensajes were strings
            var docs = await _historial.Find(new BsonDocument()).ToListAsync();
            var result = new List<HistorialChat>();
            foreach (var doc in docs)
            {
                result.Add(MapBsonToHistorialChat(doc.ToBsonDocument()));
            }
            return result;
        }

        public async Task<HistorialChat> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id.Trim());
            var filter = Builders<HistorialChat>.Filter.Eq(h => h.Id, objectId);

            // Try to get raw BsonDocument to map safely
            var collection = _historial.Database.GetCollection<BsonDocument>(_historial.CollectionNamespace.CollectionName);
            var bsonDoc = await collection.Find(new BsonDocument("_id", objectId)).FirstOrDefaultAsync();
            if (bsonDoc == null) return null!;
            return MapBsonToHistorialChat(bsonDoc);
        }

        public async Task CreateAsync(HistorialChat historial)
        {
            await _historial.InsertOneAsync(historial);
        }

        public async Task UpdateAsync(string id, HistorialChat historial)
        {
            var objectId = ObjectId.Parse(id.Trim());
            await _historial.ReplaceOneAsync(h => h.Id == objectId, historial);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id.Trim());
            await _historial.DeleteOneAsync(h => h.Id == objectId);
        }

        private HistorialChat MapBsonToHistorialChat(BsonDocument doc)
        {
            var historial = new HistorialChat();

            if (doc.Contains("_id"))
            {
                try { historial.Id = doc["_id"].AsObjectId; } catch { historial.Id = ObjectId.GenerateNewId(); }
            }

            historial.UsuarioCorreo = doc.Contains("usuario_correo") ? doc["usuario_correo"].AsString : string.Empty;
            historial.Titulo = doc.Contains("titulo") ? doc["titulo"].AsString : string.Empty;
            historial.Mensajes = new List<ChatMessage>();

            if (doc.Contains("mensajes") && doc["mensajes"].IsBsonArray)
            {
                var arr = doc["mensajes"].AsBsonArray;
                foreach (var item in arr)
                {
                    try
                    {
                        if (item.IsString)
                        {
                            // older format: 'usuario: ...' or 'bot: ...'
                            var s = item.AsString;
                            var msg = new ChatMessage { Timestamp = DateTime.UtcNow };
                            if (s.StartsWith("usuario: ", StringComparison.OrdinalIgnoreCase))
                            {
                                msg.Tipo = "usuario";
                                msg.Contenido = s.Substring("usuario: ".Length);
                            }
                            else if (s.StartsWith("bot: ", StringComparison.OrdinalIgnoreCase))
                            {
                                msg.Tipo = "bot";
                                msg.Contenido = s.Substring("bot: ".Length);
                            }
                            else
                            {
                                msg.Tipo = "usuario";
                                msg.Contenido = s;
                            }
                            historial.Mensajes.Add(msg);
                        }
                        else if (item.IsBsonDocument)
                        {
                            var b = item.AsBsonDocument;
                            var msg = new ChatMessage();
                            if (b.Contains("tipo")) msg.Tipo = b["tipo"].AsString;
                            if (b.Contains("contenido")) msg.Contenido = b["contenido"].AsString;
                            if (b.Contains("timestamp"))
                            {
                                try { msg.Timestamp = b["timestamp"].ToUniversalTime(); }
                                catch { msg.Timestamp = DateTime.UtcNow; }
                            }
                            else
                            {
                                msg.Timestamp = DateTime.UtcNow;
                            }
                            historial.Mensajes.Add(msg);
                        }
                    }
                    catch
                    {
                        // ignore malformed entries
                    }
                }
            }

            historial.Favorito = doc.Contains("favorito") ? doc["favorito"].AsBoolean : false;
            historial.UltimaActividadEn = doc.Contains("ultima_actividad_en") ? doc["ultima_actividad_en"].ToUniversalTime() : DateTime.UtcNow;
            historial.TotalMensajes = doc.Contains("total_mensajes") ? doc["total_mensajes"].ToInt32() : historial.Mensajes.Count;

            return historial;
        }
    }
}
