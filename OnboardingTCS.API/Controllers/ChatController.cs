using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.Core.Interfaces;
using System.Security.Claims;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos los usuarios autenticados pueden chatear
    public class ChatController : ControllerBase
    {
        private readonly IOllamaService _ollamaService;
        private readonly IHistorialChatService _historialService;
        private readonly IDocumentoRepository _documentoRepository;

        public ChatController(IOllamaService ollamaService, IHistorialChatService historialService, IDocumentoRepository documentoRepository)
        {
            _ollamaService = ollamaService;
            _historialService = historialService;
            _documentoRepository = documentoRepository;
        }

        /// <summary>
        /// [TODOS] Chat general - pregunta al asistente de IA con contexto de documentos
        /// </summary>
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Pregunta))
            {
                return BadRequest("La pregunta no puede estar vacía.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            try
            {
                // Buscar en documentos relevantes para dar contexto
                var documentos = await _documentoRepository.GetAllAsync();
                var contextoDocumentos = string.Empty;

                // Tomar los primeros documentos como contexto (puedes mejorarlo con búsqueda semántica)
                var docsRelevantes = documentos.Take(3);
                if (docsRelevantes.Any())
                {
                    contextoDocumentos = string.Join("\n\n", docsRelevantes.Select(d => 
                        $"Documento: {d.Titulo}\nContenido: {d.Archivo?.Substring(0, Math.Min(d.Archivo.Length, 500))}..."));
                }

                // Crear prompt con contexto
                var prompt = $@"Eres un asistente de onboarding de TCS. Ayuda a responder preguntas sobre el proceso de integración.

Contexto de documentos disponibles:
{contextoDocumentos}

Pregunta del usuario: {request.Pregunta}

Responde de manera amable, profesional y basándote en el contexto proporcionado. Si no tienes información suficiente, indícalo claramente.";

                Console.WriteLine($"[Chat] {userName} pregunta: {request.Pregunta}");

                var respuesta = await _ollamaService.GenerateResponseAsync(prompt);

                // Guardar en historial de chat si se implementa
                // await _historialService.GuardarInteraccionAsync(userId, request.Pregunta, respuesta);

                return Ok(new { 
                    respuesta,
                    usuario = userName,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Chat] Error: {ex.Message}");
                return StatusCode(500, new { error = "Error al procesar la pregunta. Intenta nuevamente." });
            }
        }

        /// <summary>
        /// [TODOS] Chat específico sobre un documento
        /// </summary>
        [HttpPost("ask-document/{documentId}")]
        public async Task<IActionResult> AskAboutDocument(string documentId, [FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Pregunta))
            {
                return BadRequest("La pregunta no puede estar vacía.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            var documento = await _documentoRepository.GetByIdAsync(documentId);
            if (documento == null)
            {
                return NotFound("Documento no encontrado.");
            }

            try
            {
                var prompt = $@"Basándote únicamente en el siguiente documento sobre onboarding:

Documento: {documento.Titulo}
Contenido: {documento.Archivo}

Pregunta: {request.Pregunta}

Responde únicamente basándote en la información del documento. Si la pregunta no se puede responder con el contenido del documento, indícalo claramente.";

                Console.WriteLine($"[ChatDoc] {userName} pregunta sobre '{documento.Titulo}': {request.Pregunta}");

                var respuesta = await _ollamaService.GenerateResponseAsync(prompt);

                return Ok(new { 
                    respuesta,
                    documento = documento.Titulo,
                    usuario = userName,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChatDoc] Error: {ex.Message}");
                return StatusCode(500, new { error = "Error al procesar la pregunta sobre el documento." });
            }
        }

        public class ChatRequest
        {
            public string Pregunta { get; set; } = string.Empty;
        }
    }
}