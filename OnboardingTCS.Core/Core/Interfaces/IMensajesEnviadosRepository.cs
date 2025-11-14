using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IMensajesEnviadosRepository
    {
        Task CreateAsync(MensajesEnviados mensaje);
        Task DeleteAsync(string id);
        Task<IEnumerable<MensajesEnviados>> GetAllAsync();
        Task<MensajesEnviados> GetByIdAsync(string id);
        Task UpdateAsync(string id, MensajesEnviados mensaje);
    }
}
