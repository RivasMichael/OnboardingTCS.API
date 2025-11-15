using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IMensajesAutomaticosService
    {
        Task<IEnumerable<MensajesAutomaticosDto>> GetAllMensajesAsync();
        Task<MensajesAutomaticosDto> GetMensajeByIdAsync(string id);
        Task InsertMensajeAsync(MensajesAutomaticosCreateDto dto);
        Task UpdateMensajeAsync(string id, MensajesAutomaticosUpdateDto dto);
        Task DeleteMensajeAsync(string id);
    }
}