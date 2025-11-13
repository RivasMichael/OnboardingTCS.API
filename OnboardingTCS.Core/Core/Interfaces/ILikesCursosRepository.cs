using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface ILikesCursosRepository
    {
        Task CreateAsync(LikesCursos like);
        Task DeleteAsync(string id);
        Task<IEnumerable<LikesCursos>> GetAllAsync();
        Task<LikesCursos> GetByIdAsync(string id);
        Task UpdateAsync(string id, LikesCursos like);
    }
}
