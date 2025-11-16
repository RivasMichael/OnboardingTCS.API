using OnboardingTCS.Core.Entities;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IActividadesRepository
    {
        Task CreateAsync(Actividades actividad);
        Task DeleteAsync(string id);
        Task<IEnumerable<Actividades>> GetAllAsync();
        Task<Actividades> GetByIdAsync(string id);
        Task<IEnumerable<Actividades>> GetByUsuarioAsync(string usuarioCorreo);
        Task UpdateAsync(string id, Actividades actividad);
    }
}