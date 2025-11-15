using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IMensajesEnviadosService
    {
        Task<IEnumerable<MensajesEnviadosDto>> GetAllMensajesAsync();
        Task<MensajesEnviadosDto> GetMensajeByIdAsync(string id);
        Task InsertMensajeAsync(MensajesEnviadosCreateDto dto);
        Task UpdateMensajeAsync(string id, MensajesEnviadosUpdateDto dto);
        Task DeleteMensajeAsync(string id);
    }
}