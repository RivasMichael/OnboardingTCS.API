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
    public class LikesCursosRepository : ILikesCursosRepository
    {
        private readonly IMongoCollection<LikesCursos> _likesCursos;

        public LikesCursosRepository(MongoDbContext context)
        {
            _likesCursos = context.likes_cursos;
        }

        public async Task<IEnumerable<LikesCursos>> GetAllAsync()
        {
            return await _likesCursos.Find(_ => true).ToListAsync();
        }

        public async Task<LikesCursos> GetByIdAsync(string id)
        {
            return await _likesCursos.Find(l => l.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(LikesCursos like)
        {
            await _likesCursos.InsertOneAsync(like);
        }

        public async Task UpdateAsync(string id, LikesCursos like)
        {
            await _likesCursos.ReplaceOneAsync(l => l.Id == id, like);
        }

        public async Task DeleteAsync(string id)
        {
            await _likesCursos.DeleteOneAsync(l => l.Id == id);
        }
    }
}
