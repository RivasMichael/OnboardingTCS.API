using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync();
        Task<UsuarioDto> GetUsuarioByIdAsync(string id);
        Task<PerfilUsuarioDto> GetPerfilUsuarioAsync(string id);
        Task InsertUsuarioAsync(UsuarioCreateDto dto);
        Task UpdateUsuarioAsync(string id, UsuarioUpdateDto dto);
        Task DeleteUsuarioAsync(string id);
        
        // Métodos de autenticación
        Task<SignInResponse?> SignInAsync(SignInRequest request);
        Task<SignInResponse?> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> ValidatePasswordAsync(string email, string password);
    }
}