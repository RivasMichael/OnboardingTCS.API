using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Data;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class DocumentoRepository : IDocumentoRepository
    {
        private readonly IMongoCollection<Documento> _documentos;

        public DocumentoRepository(MongoDbContext context)
        {
            _documentos = context.Documentos;
        }

        public async Task<IEnumerable<Documento>> GetAllAsync()
        {
            return await _documentos.Find(_ => true).ToListAsync();
        }

        public async Task<Documento> GetByIdAsync(string id)
        {
            return await _documentos.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Documento documento)
        {
            await _documentos.InsertOneAsync(documento);
        }

        public async Task UpdateAsync(string id, Documento documento)
        {
            await _documentos.ReplaceOneAsync(d => d.Id == id, documento);
        }

        public async Task DeleteAsync(string id)
        {
            await _documentos.DeleteOneAsync(d => d.Id == id);
        }
    }
}