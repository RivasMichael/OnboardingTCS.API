using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync();
        Task<UsuarioDto> GetUsuarioByIdAsync(string id);
        Task InsertUsuarioAsync(UsuarioCreateDto dto);
        Task UpdateUsuarioAsync(string id, UsuarioUpdateDto dto);
        Task DeleteUsuarioAsync(string id);
    }
}