using MongoDB.Driver;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class ActividadesRepository : IActividadesRepository
    {
        private readonly IMongoCollection<Actividades> _actividades;

        public ActividadesRepository(MongoDbContext context)
        {
            _actividades = context.actividades;
        }

        public async Task<IEnumerable<Actividades>> GetAllAsync()
        {
            return await _actividades.Find(_ => true).ToListAsync();
        }

        public async Task<Actividades> GetByIdAsync(string id)
        {
            return await _actividades.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Actividades actividad)
        {
            await _actividades.InsertOneAsync(actividad);
        }

        public async Task UpdateAsync(string id, Actividades actividad)
        {
            await _actividades.ReplaceOneAsync(a => a.Id == id, actividad);
        }

        public async Task DeleteAsync(string id)
        {
            await _actividades.DeleteOneAsync(a => a.Id == id);
        }
    }
}
