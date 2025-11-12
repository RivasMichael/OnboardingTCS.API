using MongoDB.Bson;
using MongoDB.Driver;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class MensajesAutomaticosRepository : IMensajesAutomaticosRepository

    {
        private readonly IMongoCollection<MensajesAutomaticos> _mensajesAutomaticos;

        public MensajesAutomaticosRepository(MongoDbContext context)
        {
            // Inicializa la colección de MongoDB
            _mensajesAutomaticos = context.mensajes_automaticos;
        }

        public async Task<IEnumerable<MensajesAutomaticos>> GetAllAsync()
        {
            // Obtiene todos los documentos de la colección
            return await _mensajesAutomaticos.Find(_ => true).ToListAsync();
        }

        public async Task<MensajesAutomaticos> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id.Trim()); // Quita espacios en blanco
            return await _mensajesAutomaticos.Find(m => m.Id == objectId).FirstOrDefaultAsync();
        }


        public async Task CreateAsync(MensajesAutomaticos mensaje)
        {
            // Inserta un nuevo documento
            await _mensajesAutomaticos.InsertOneAsync(mensaje);
        }

        public async Task UpdateAsync(string id, MensajesAutomaticos mensaje)
        {
            var objectId = ObjectId.Parse(id.Trim());
            await _mensajesAutomaticos.ReplaceOneAsync(m => m.Id == objectId, mensaje);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id.Trim());
            await _mensajesAutomaticos.DeleteOneAsync(m => m.Id == objectId);
        }
    }
}
