using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Interfaces;
using System.Threading.Tasks;
using System.Security.Claims;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/ollama")]
    [Authorize] // ? AGREGAR: Solo usuarios autenticados pueden usar IA
    public class OllamaController : ControllerBase
    {
        private readonly IOllamaService _ollamaService;

        public OllamaController(IOllamaService ollamaService)
        {
            _ollamaService = ollamaService;
        }

        /// <summary>
        /// Endpoint para enviar un prompt a Ollama y obtener una respuesta.
        /// REQUIERE AUTENTICACIÓN - Solo usuarios logueados pueden usar IA.
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

            try
            {
                // Obtener información del usuario del JWT para logs/auditoría
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                
                Console.WriteLine($"[Ollama] Usuario {userName} ({userId}) pregunta: {request.Question.Substring(0, Math.Min(request.Question.Length, 100))}...");

                var response = await _ollamaService.GenerateResponseAsync(request.Question);
                
                Console.WriteLine($"[Ollama] Respuesta generada exitosamente para {userName}");
                
                return Ok(new { 
                    respuesta = response,
                    usuario = userName // Opcional: incluir quién hizo la pregunta
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ollama] Error: {ex.Message}");
                return StatusCode(500, new { error = "Error al procesar la consulta de IA" });
            }
        }

        /// <summary>
        /// [SOLO ADMIN] Endpoint para generar embeddings de texto
        /// </summary>
        /// <param name="request">Texto para generar embeddings</param>
        /// <returns>Array de embeddings</returns>
        [HttpPost("embeddings")]
        [Authorize(Roles = "admin")] // ? Solo admins pueden generar embeddings
        public async Task<IActionResult> GenerateEmbeddings([FromBody] EmbeddingRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("El campo 'text' no puede estar vacío.");
            }

            try
            {
                var embeddings = await _ollamaService.GenerateEmbeddingAsync(request.Text);
                return Ok(new { embeddings });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ollama Embeddings] Error: {ex.Message}");
                return StatusCode(500, new { error = "Error al generar embeddings" });
            }
        }
    }

    public class AskRequestDto
    {
        public string Question { get; set; } = string.Empty;
    }

    public class EmbeddingRequestDto
    {
        public string Text { get; set; } = string.Empty;
    }
}