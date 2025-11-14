using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursoController : ControllerBase
    {
        private readonly CursoRepository _repository;

        public CursoController(CursoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Curso>>> GetAll()
        {
            var cursos = await _repository.GetAllAsync();
            return Ok(cursos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Curso>> GetById(string id)
        {
            var curso = await _repository.GetByIdAsync(id);
            if (curso == null) return NotFound();
            return Ok(curso);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Curso curso)
        {
            await _repository.CreateAsync(curso);
            return CreatedAtAction(nameof(GetById), new { id = curso.Id }, curso);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Curso curso)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            curso.Id = id;
            await _repository.UpdateAsync(id, curso);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}