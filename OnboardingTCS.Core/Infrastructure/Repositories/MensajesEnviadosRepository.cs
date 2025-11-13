using MongoDB.Driver;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class MensajesEnviadosRepository : IMensajesEnviadosRepository
    {
        private readonly IMongoCollection<MensajesEnviados> _mensajesEnviados;

        public MensajesEnviadosRepository(MongoDbContext context)
        {
            _mensajesEnviados = context.mensajes_enviados;
        }

        public async Task<IEnumerable<MensajesEnviados>> GetAllAsync()
        {
            return await _mensajesEnviados.Find(_ => true).ToListAsync();
        }

        public async Task<MensajesEnviados> GetByIdAsync(string id)
        {
            return await _mensajesEnviados.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(MensajesEnviados mensaje)
        {
            await _mensajesEnviados.InsertOneAsync(mensaje);
        }

        public async Task UpdateAsync(string id, MensajesEnviados mensaje)
        {
            await _mensajesEnviados.ReplaceOneAsync(m => m.Id == id, mensaje);
        }

        public async Task DeleteAsync(string id)
        {
            await _mensajesEnviados.DeleteOneAsync(m => m.Id == id);
        }
    }
}
