using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface ISupervisorService
    {
        Task<IEnumerable<SupervisorDto>> GetAllSupervisorsAsync();
        Task<SupervisorDto> GetSupervisorByIdAsync(string id);
        Task InsertSupervisorAsync(SupervisorCreateDto dto);
        Task UpdateSupervisorAsync(string id, SupervisorUpdateDto dto);
        Task DeleteSupervisorAsync(string id);
    }
}