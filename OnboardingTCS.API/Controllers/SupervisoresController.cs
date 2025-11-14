using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupervisoresController : ControllerBase
    {
        private readonly ISupervisorService _service;

        public SupervisoresController(ISupervisorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupervisorDto>>> GetAll()
        {
            var supervisors = await _service.GetAllSupervisorsAsync();
            return Ok(supervisors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SupervisorDto>> GetById(string id)
        {
            var supervisor = await _service.GetSupervisorByIdAsync(id);
            if (supervisor == null) return NotFound();
            return Ok(supervisor);
        }

        [HttpPost]
        public async Task<ActionResult> Create(SupervisorCreateDto dto)
        {
            await _service.InsertSupervisorAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, SupervisorUpdateDto dto)
        {
            await _service.UpdateSupervisorAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteSupervisorAsync(id);
            return NoContent();
        }
    }
}