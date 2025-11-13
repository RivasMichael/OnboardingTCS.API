using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MensajesAutomaticosController : ControllerBase
    {
        private readonly IMensajesAutomaticosRepository _repository;

        public MensajesAutomaticosController(IMensajesAutomaticosRepository repository)
        {
            _repository = repository;
        }

        // GET: api/mensajesautomaticos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MensajesAutomaticos>>> GetAll()
        {
            var mensajes = await _repository.GetAllAsync();
            return Ok(mensajes);
        }

        // GET: api/mensajesautomaticos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MensajesAutomaticos>> GetById(string id)
        {
            var mensaje = await _repository.GetByIdAsync(id);
            if (mensaje == null)
            {
                return NotFound();
            }
            return Ok(mensaje);
        }

        // POST: api/mensajesautomaticos
        [HttpPost]
        public async Task<ActionResult> Create(MensajesAutomaticos mensaje)
        {
            await _repository.CreateAsync(mensaje);
            return CreatedAtAction(nameof(GetById), new { id = mensaje.Id }, mensaje);
        }

        // PUT: api/mensajesautomaticos/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, MensajesAutomaticos mensaje)
        {
            var existingMensaje = await _repository.GetByIdAsync(id);
            if (existingMensaje == null)
            {
                return NotFound();
            }

            mensaje.Id = ObjectId.Parse(id);  // Convierte string a ObjectId
            await _repository.UpdateAsync(id, mensaje);
            return NoContent();
        }

        // DELETE: api/mensajesautomaticos/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingMensaje = await _repository.GetByIdAsync(id);
            if (existingMensaje == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
