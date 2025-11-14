using System.Threading.Tasks;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using MongoDB.Driver;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IMongoCollection<Usuario> _usuarios;

        public AuthRepository(MongoDbContext context)
        {
            _usuarios = context.usuarios;
        }

        public async Task<Usuario?> GetUserByEmailAsync(string correo)
        {
            var filter = Builders<Usuario>.Filter.Eq(u => u.Correo, correo);
            return await _usuarios.Find(filter).FirstOrDefaultAsync();
        }
    }
}
