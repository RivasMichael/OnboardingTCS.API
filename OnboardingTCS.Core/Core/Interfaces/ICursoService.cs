using OnboardingTCS.Core.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Interfaces
{
    public interface ICursoService
    {
        Task<IEnumerable<CursoListDto>> GetAllCursosAsync();
        Task<IEnumerable<CursoSimpleDto>> GetCursosSimpleAsync(string categoria = null, string nivel = null);
        Task<IEnumerable<CursoCompletDto>> GetAllCursosCompletosAsync();
        Task<CursoDetailDto> GetCursoByIdAsync(string id);
        Task<IEnumerable<CursoListDto>> GetCursosFiltradosAsync(string categoria = null, string nivel = null);
        Task<IEnumerable<CursoCompletDto>> GetCursosCompletosFiltraodosAsync(string categoria = null, string nivel = null);
        Task<IEnumerable<string>> GetCategoriasAsync();
        Task<IEnumerable<string>> GetNivelesAsync();
        Task CreateCursoAsync(CursoCreateDto dto);
        Task UpdateCursoAsync(string id, CursoUpdateDto dto);
        Task DeleteCursoAsync(string id);
    }
}