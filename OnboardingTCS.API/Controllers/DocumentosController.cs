using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentoRepository _repository;

        public DocumentosController(IDocumentoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Documento>>> GetAll()
        {
            var documentos = await _repository.GetAllAsync();
            return Ok(documentos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Documento>> GetById(string id)
        {
            var documento = await _repository.GetByIdAsync(id);
            if (documento == null)
            {
                return NotFound();
            }
            return Ok(documento);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Documento documento)
        {
            await _repository.CreateAsync(documento);
            return CreatedAtAction(nameof(GetById), new { id = documento.Id }, documento);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Documento documento)
        {
            var existingDocumento = await _repository.GetByIdAsync(id);
            if (existingDocumento == null)
            {
                return NotFound();
            }
            documento.Id = id;
            await _repository.UpdateAsync(id, documento);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingDocumento = await _repository.GetByIdAsync(id);
            if (existingDocumento == null)
            {
                return NotFound();
            }
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}