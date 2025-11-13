using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MensajesEnviadosController : ControllerBase
    {
        private readonly IMensajesEnviadosRepository _repository;

        public MensajesEnviadosController(IMensajesEnviadosRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MensajesEnviados>>> GetAll()
        {
            var mensajes = await _repository.GetAllAsync();
            return Ok(mensajes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MensajesEnviados>> GetById(string id)
        {
            var mensaje = await _repository.GetByIdAsync(id);
            if (mensaje == null) return NotFound();
            return Ok(mensaje);
        }

        [HttpPost]
        public async Task<ActionResult> Create(MensajesEnviados mensaje)
        {
            await _repository.CreateAsync(mensaje);
            return CreatedAtAction(nameof(GetById), new { id = mensaje.Id }, mensaje);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, MensajesEnviados mensaje)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            mensaje.Id = id;
            await _repository.UpdateAsync(id, mensaje);
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
