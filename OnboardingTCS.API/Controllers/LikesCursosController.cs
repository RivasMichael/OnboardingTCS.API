using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesCursosController : ControllerBase
    {
        private readonly ILikesCursosRepository _repository;

        public LikesCursosController(ILikesCursosRepository repository)
        {
            _repository = repository;
        }

        // GET: api/likescursos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikesCursos>>> GetAll()
        {
            var likes = await _repository.GetAllAsync();
            return Ok(likes);
        }

        // GET: api/likescursos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LikesCursos>> GetById(string id)
        {
            var like = await _repository.GetByIdAsync(id);
            if (like == null)
            {
                return NotFound();
            }
            return Ok(like);
        }

        // POST: api/likescursos
        [HttpPost]
        public async Task<ActionResult> Create(LikesCursos like)
        {
            await _repository.CreateAsync(like);
            return CreatedAtAction(nameof(GetById), new { id = like.Id }, like);
        }

        // PUT: api/likescursos/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, LikesCursos like)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            like.Id = id;
            await _repository.UpdateAsync(id, like);
            return NoContent();
        }

        // DELETE: api/likescursos/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
