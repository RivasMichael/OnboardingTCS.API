using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface ILikesCursosService
    {
        Task<IEnumerable<LikesCursosDto>> GetAllLikesAsync();
        Task<LikesCursosDto> GetLikeByIdAsync(string id);
        Task InsertLikeAsync(LikesCursosCreateDto dto);
        Task UpdateLikeAsync(string id, LikesCursosUpdateDto dto);
        Task DeleteLikeAsync(string id);
    }
}