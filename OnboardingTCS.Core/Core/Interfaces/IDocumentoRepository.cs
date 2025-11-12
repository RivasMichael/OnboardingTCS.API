using System.Collections.Generic;
using System.Threading.Tasks;
using OnboardingTCS.Core.Entities;

namespace OnboardingTCS.Core.Interfaces
{
    public interface IDocumentoRepository
    {
        Task<IEnumerable<Documento>> GetAllAsync();
        Task<Documento> GetByIdAsync(string id);
        Task CreateAsync(Documento documento);
        Task UpdateAsync(string id, Documento documento);
        Task DeleteAsync(string id);
    }
}