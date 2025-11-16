using MongoDB.Bson;
using MongoDB.Driver;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using OnboardingTCS.Core.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class HistorialChatRepository : IHistorialChatRepository
    {
        private readonly IMongoCollection<HistorialChat> _historialChats;

        public HistorialChatRepository(MongoDbContext context)
        {
            _historialChats = context.HistorialChats;
        }

        public async Task<IEnumerable<HistorialChat>> GetAllAsync()
        {
            return await _historialChats.Find(_ => true).ToListAsync();
        }

        public async Task<HistorialChat> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _historialChats.Find(h => h.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(HistorialChat historialChat)
        {
            await _historialChats.InsertOneAsync(historialChat);
        }

        public async Task UpdateAsync(string id, HistorialChat historialChat)
        {
            var objectId = ObjectId.Parse(id);
            await _historialChats.ReplaceOneAsync(h => h.Id == objectId.ToString(), historialChat);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _historialChats.DeleteOneAsync(h => h.Id == objectId.ToString());
        }
    }
}