using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistorialChatController : ControllerBase
    {
        private readonly HistorialChatRepository _repository;

        public HistorialChatController(HistorialChatRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialChat>>> GetAll()
        {
            var chats = await _repository.GetAllAsync();
            return Ok(chats);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialChat>> GetById(string id)
        {
            var chat = await _repository.GetByIdAsync(id);
            if (chat == null) return NotFound();
            return Ok(chat);
        }

        [HttpPost]
        public async Task<ActionResult> Create(HistorialChat chat)
        {
            await _repository.CreateAsync(chat);
            return CreatedAtAction(nameof(GetById), new { id = chat.Id }, chat);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, HistorialChat chat)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            chat.Id = id;
            await _repository.UpdateAsync(id, chat);
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