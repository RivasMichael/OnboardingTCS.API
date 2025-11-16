using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface ICursoRepository
    {
        Task<IEnumerable<Curso>> GetAllAsync();
        Task<Curso> GetByIdAsync(string id);
        Task<IEnumerable<Curso>> GetByCategoriaAsync(string categoria);
        Task<IEnumerable<Curso>> GetByNivelAsync(string nivel);
        Task<IEnumerable<Curso>> GetByCategoriaAndNivelAsync(string categoria, string nivel);
        Task<IEnumerable<string>> GetCategoriasAsync();
        Task<IEnumerable<string>> GetNivelesAsync();
        Task CreateAsync(Curso curso);
        Task UpdateAsync(string id, Curso curso);
        Task DeleteAsync(string id);
    }
}