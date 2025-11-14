using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Interfaces;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/ollama")]
    public class OllamaController : ControllerBase
    {
        private readonly IOllamaService _ollamaService;

        public OllamaController(IOllamaService ollamaService)
        {
            _ollamaService = ollamaService;
        }

        /// <summary>
        /// Endpoint para enviar un prompt a Ollama y obtener una respuesta.
        /// </summary>
        /// <param name="request">El objeto que contiene la pregunta que se enviará a Ollama.</param>
        /// <returns>La respuesta generada por Ollama.</returns>
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("El campo 'question' no puede estar vacío.");
            }

            var response = await _ollamaService.GenerateResponseAsync(request.Question);
            return Ok(new { respuesta = response });
        }
    }

    public class AskRequestDto
    {
        public string Question { get; set; } = string.Empty;
    }
}