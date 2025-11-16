using OnboardingTCS.Core.Core.DTOs;
using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Interfaces
{
    public interface IHistorialChatService
    {
        Task<IEnumerable<HistorialChatDto>> GetAllHistorialChatAsync();
        Task<HistorialChatDto> GetHistorialChatByIdAsync(string id);
        Task<IEnumerable<HistorialChatDto>> GetHistorialChatByUsuarioAsync(string usuarioCorreo);
        Task CreateHistorialChatAsync(HistorialChatCreateDto dto);
        Task UpdateHistorialChatAsync(string id, HistorialChatUpdateDto dto);
        Task DeleteHistorialChatAsync(string id);
    }
}