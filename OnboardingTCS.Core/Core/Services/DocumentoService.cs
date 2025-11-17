using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.Core.DTOs;
using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Services
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IDocumentoRepository _repository;

        public DocumentoService(IDocumentoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DocumentoDto>> GetAllDocumentosAsync()
        {
            var documentos = await _repository.GetAllAsync();
            return documentos.Select(d => new DocumentoDto
            {
                Id = d.Id,
                Titulo = d.Titulo,
                Descripcion = d.Descripcion,
                Categoria = d.Categoria,
                NombreArchivo = d.NombreArchivo,
                UrlArchivo = d.UrlArchivo,
                TipoArchivo = d.TipoArchivo,
                TamanoArchivo = d.TamanoArchivo,
                Obligatorio = d.Obligatorio,
                SubidoPor = d.SubidoPor,
                SubidoPorNombre = d.SubidoPorNombre,
                VisibleTodos = d.VisibleTodos,
                Descargas = d.Descargas,
                CreadoEn = d.CreadoEn,
                Archivo = d.Archivo,
                Embedding = d.Embedding
            });
        }

        public async Task<DocumentoDto> GetDocumentoByIdAsync(string id)
        {
            var documento = await _repository.GetByIdAsync(id);
            if (documento == null) return null;

            return new DocumentoDto
            {
                Id = documento.Id,
                Titulo = documento.Titulo,
                Descripcion = documento.Descripcion,
                Categoria = documento.Categoria,
                NombreArchivo = documento.NombreArchivo,
                UrlArchivo = documento.UrlArchivo,
                TipoArchivo = documento.TipoArchivo,
                TamanoArchivo = documento.TamanoArchivo,
                Obligatorio = documento.Obligatorio,
                SubidoPor = documento.SubidoPor,
                SubidoPorNombre = documento.SubidoPorNombre,
                VisibleTodos = documento.VisibleTodos,
                Descargas = documento.Descargas,
                CreadoEn = documento.CreadoEn,
                Archivo = documento.Archivo,
                Embedding = documento.Embedding
            };
        }

        public async Task InsertDocumentoAsync(DocumentoCreateDto dto)
        {
            var documento = new Documento
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Categoria = dto.Categoria,
                NombreArchivo = dto.NombreArchivo,
                UrlArchivo = dto.UrlArchivo,
                TipoArchivo = dto.TipoArchivo,
                TamanoArchivo = dto.TamanoArchivo,
                Obligatorio = dto.Obligatorio,
                SubidoPor = dto.SubidoPor,
                SubidoPorNombre = dto.SubidoPorNombre,
                VisibleTodos = dto.VisibleTodos,
                Descargas = dto.Descargas,
                CreadoEn = DateTime.UtcNow,
                Archivo = dto.Archivo,
                Embedding = dto.Embedding
            };

            await _repository.CreateAsync(documento);
        }

        public async Task UpdateDocumentoAsync(string id, DocumentoUpdateDto dto)
        {
            var documento = await _repository.GetByIdAsync(id);
            if (documento == null) return;

            documento.Titulo = dto.Titulo;
            documento.Descripcion = dto.Descripcion;
            documento.Categoria = dto.Categoria;
            documento.NombreArchivo = dto.NombreArchivo;
            documento.UrlArchivo = dto.UrlArchivo;
            documento.TipoArchivo = dto.TipoArchivo;
            documento.TamanoArchivo = dto.TamanoArchivo;
            documento.Obligatorio = dto.Obligatorio;
            documento.SubidoPor = dto.SubidoPor;
            documento.SubidoPorNombre = dto.SubidoPorNombre;
            documento.VisibleTodos = dto.VisibleTodos;
            documento.Descargas = dto.Descargas;
            documento.Archivo = dto.Archivo;
            documento.Embedding = dto.Embedding;

            await _repository.UpdateAsync(id, documento);
        }

        public async Task DeleteDocumentoAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}