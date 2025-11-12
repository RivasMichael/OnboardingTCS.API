using System.Collections.Generic;
using System.Threading.Tasks;
using OnboardingTCS.Core.Entities;

namespace OnboardingTCS.Core.Interfaces
{
    public interface ISupervisorRepository
    {
        Task<IEnumerable<Supervisor>> GetAllAsync();
        Task<Supervisor> GetByIdAsync(string id);
        Task CreateAsync(Supervisor supervisor);
        Task UpdateAsync(string id, Supervisor supervisor);
        Task DeleteAsync(string id);
    }
}