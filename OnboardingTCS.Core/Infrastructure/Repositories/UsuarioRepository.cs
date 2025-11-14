using MongoDB.Bson;
using MongoDB.Driver;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class UsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _usuarios;

        public UsuarioRepository(MongoDbContext context)
        {
            _usuarios = context.Usuarios;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _usuarios.Find(_ => true).ToListAsync();
        }

        public async Task<Usuario> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _usuarios.Find(u => u.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _usuarios.Find(u => u.Correo == email).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Usuario usuario)
        {
            await _usuarios.InsertOneAsync(usuario);
        }

        public async Task UpdateAsync(string id, Usuario usuario)
        {
            var objectId = ObjectId.Parse(id);
            await _usuarios.ReplaceOneAsync(u => u.Id == objectId.ToString(), usuario);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _usuarios.DeleteOneAsync(u => u.Id == objectId.ToString());
        }
    }
}