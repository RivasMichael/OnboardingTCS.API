using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IActividadesService
    {
        Task<IEnumerable<ActividadesDto>> GetAllActividadesAsync();
        Task<ActividadesDto> GetActividadesByIdAsync(string id);
        Task InsertActividadesAsync(ActividadesCreateDto dto);
        Task UpdateActividadesAsync(string id, ActividadesUpdateDto dto);
        Task DeleteActividadesAsync(string id);
    }
}