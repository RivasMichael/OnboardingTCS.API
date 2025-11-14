using System.Collections.Generic;
using System.Threading.Tasks;
using OnboardingTCS.Core.Entities;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IHistorialChatRepository
    {
        Task<IEnumerable<HistorialChat>> GetAllAsync();
        Task<HistorialChat> GetByIdAsync(string id);
        Task CreateAsync(HistorialChat historial);
        Task UpdateAsync(string id, HistorialChat historial);
        Task DeleteAsync(string id);
    }
}
