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
    public class ActividadesController : ControllerBase
    {
        private readonly IActividadesRepository _repository;

        public ActividadesController(IActividadesRepository repository)
        {
            _repository = repository;
        }

        // GET: api/actividades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actividades>>> GetAll()
        {
            var actividades = await _repository.GetAllAsync();
            return Ok(actividades);
        }

        // GET: api/actividades/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Actividades>> GetById(string id)
        {
            var actividad = await _repository.GetByIdAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }
            return Ok(actividad);
        }

        // POST: api/actividades
        [HttpPost]
        public async Task<ActionResult> Create(Actividades actividad)
        {
            await _repository.CreateAsync(actividad);
            return CreatedAtAction(nameof(GetById), new { id = actividad.Id }, actividad);
        }

        // PUT: api/actividades/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Actividades actividad)
        {
            var existingActividad = await _repository.GetByIdAsync(id);
            if (existingActividad == null)
            {
                return NotFound();
            }

            actividad.Id = id;
            await _repository.UpdateAsync(id, actividad);
            return NoContent();
        }

        // DELETE: api/actividades/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingActividad = await _repository.GetByIdAsync(id);
            if (existingActividad == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
