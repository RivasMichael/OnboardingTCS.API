using OnboardingTCS.Core.Entities;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IMensajesAutomaticosRepository
    {
        Task CreateAsync(MensajesAutomaticos mensaje);
        Task DeleteAsync(string id);
        Task<IEnumerable<MensajesAutomaticos>> GetAllAsync();
        Task<MensajesAutomaticos> GetByIdAsync(string id);
        Task UpdateAsync(string id, MensajesAutomaticos mensaje);
    }
}