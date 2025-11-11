using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupervisoresController : ControllerBase
    {
        private readonly ISupervisorRepository _repository;

        public SupervisoresController(ISupervisorRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supervisor>>> GetAll()
        {
            var supervisores = await _repository.GetAllAsync();
            return Ok(supervisores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Supervisor>> GetById(string id)
        {
            var supervisor = await _repository.GetByIdAsync(id);
            if (supervisor == null)
            {
                return NotFound();
            }
            return Ok(supervisor);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Supervisor supervisor)
        {
            await _repository.CreateAsync(supervisor);
            return CreatedAtAction(nameof(GetById), new { id = supervisor.Id }, supervisor);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Supervisor supervisor)
        {
            var existingSupervisor = await _repository.GetByIdAsync(id);
            if (existingSupervisor == null)
            {
                return NotFound();
            }
            supervisor.Id = id;
            await _repository.UpdateAsync(id, supervisor);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingSupervisor = await _repository.GetByIdAsync(id);
            if (existingSupervisor == null)
            {
                return NotFound();
            }
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}