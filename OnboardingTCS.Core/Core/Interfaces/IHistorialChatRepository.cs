using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IHistorialChatRepository
    {
        Task<IEnumerable<HistorialChat>> GetAllAsync();
        Task<HistorialChat> GetByIdAsync(string id);
        Task CreateAsync(HistorialChat historialChat);
        Task UpdateAsync(string id, HistorialChat historialChat);
        Task DeleteAsync(string id);
    }
}