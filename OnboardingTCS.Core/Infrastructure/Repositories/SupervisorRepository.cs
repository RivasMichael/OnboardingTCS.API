using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Data;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class SupervisorRepository : ISupervisorRepository
    {
        private readonly IMongoCollection<Supervisor> _supervisores;

        public SupervisorRepository(MongoDbContext context)
        {
            _supervisores = context.Supervisores;
        }

        public async Task<IEnumerable<Supervisor>> GetAllAsync()
        {
            return await _supervisores.Find(_ => true).ToListAsync();
        }

        public async Task<Supervisor> GetByIdAsync(string id)
        {
            return await _supervisores.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Supervisor supervisor)
        {
            await _supervisores.InsertOneAsync(supervisor);
        }

        public async Task UpdateAsync(string id, Supervisor supervisor)
        {
            await _supervisores.ReplaceOneAsync(s => s.Id == id, supervisor);
        }

        public async Task DeleteAsync(string id)
        {
            await _supervisores.DeleteOneAsync(s => s.Id == id);
        }
    }
}