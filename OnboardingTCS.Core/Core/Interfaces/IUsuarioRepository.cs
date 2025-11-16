using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario> GetByIdAsync(string id);
        Task<Usuario> GetByEmailAsync(string email);
        Task CreateAsync(Usuario usuario);
        Task UpdateAsync(string id, Usuario usuario);
        Task DeleteAsync(string id);
    }
}