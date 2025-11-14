using MongoDB.Bson;
using MongoDB.Driver;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class CursoRepository
    {
        private readonly IMongoCollection<Curso> _cursos;

        public CursoRepository(MongoDbContext context)
        {
            _cursos = context.Cursos;
        }

        public async Task<IEnumerable<Curso>> GetAllAsync()
        {
            return await _cursos.Find(_ => true).ToListAsync();
        }

        public async Task<Curso> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _cursos.Find(c => c.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Curso curso)
        {
            await _cursos.InsertOneAsync(curso);
        }

        public async Task UpdateAsync(string id, Curso curso)
        {
            var objectId = ObjectId.Parse(id);
            await _cursos.ReplaceOneAsync(c => c.Id == objectId.ToString(), curso);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _cursos.DeleteOneAsync(c => c.Id == objectId.ToString());
        }
    }
}