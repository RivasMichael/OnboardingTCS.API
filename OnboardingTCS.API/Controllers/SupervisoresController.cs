using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupervisoresController : ControllerBase
    {
        private readonly SupervisorService _service;

        public SupervisoresController(SupervisorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supervisor>>> GetAll()
        {
            var supervisores = await _service.GetAllAsync();
            return Ok(supervisores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Supervisor>> GetById(string id)
        {
            var supervisor = await _service.GetByIdAsync(id);
            if (supervisor == null)
            {
                return NotFound();
            }
            return Ok(supervisor);
        }

        [HttpPost]
        public async Task<ActionResult> Create(SupervisorDto supervisorDto)
        {
            await _service.CreateAsync(supervisorDto);
            return CreatedAtAction(nameof(GetById), new { id = supervisorDto.Correo }, supervisorDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, SupervisorDto supervisorDto)
        {
            await _service.UpdateAsync(id, supervisorDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}