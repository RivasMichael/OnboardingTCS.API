using OnboardingTCS.Core.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Interfaces
{
    public interface IDocumentoService
    {
        Task<IEnumerable<DocumentoDto>> GetAllDocumentosAsync();
        Task<DocumentoDto> GetDocumentoByIdAsync(string id);
        Task InsertDocumentoAsync(DocumentoCreateDto dto);
        Task UpdateDocumentoAsync(string id, DocumentoUpdateDto dto);
        Task DeleteDocumentoAsync(string id);
    }
}