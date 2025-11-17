using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Interfaces;
using System.Security.Claims;
using System.Linq;

namespace OnboardingTCS.API.Controllers
{
    public class ChatRequest
    {
        public string Pregunta { get; set; } = string.Empty;
    }

    public class SearchRequest
    {
        public string Pregunta { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IOllamaService _ollamaService;
        private readonly IDocumentoRepository _documentoRepository;

        public ChatController(IOllamaService ollamaService, IDocumentoRepository documentoRepository)
        {
            _ollamaService = ollamaService;
            _documentoRepository = documentoRepository;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Pregunta))
            {
                return BadRequest("La pregunta no puede estar vacía.");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                
                var shortQuestion = request.Pregunta.Length > 100 ? request.Pregunta.Substring(0, 100) + "..." : request.Pregunta;
                Console.WriteLine($"[Chat] Usuario {userName} ({userId}) pregunta: {shortQuestion}");

                var response = await _ollamaService.GenerateResponseAsync(request.Pregunta);
                
                Console.WriteLine($"[Chat] Respuesta generada exitosamente para {userName}");
                
                return Ok(new { 
                    respuesta = response,
                    usuario = userName,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Chat] Error: {ex.Message}");
                return StatusCode(500, new { error = "Error al procesar la consulta de chat" });
            }
        }

        /// <summary>
        /// Endpoint para búsqueda inteligente en documentos PDF subidos
        /// </summary>
        [HttpPost("search-ell")]
        public async Task<IActionResult> SearchInDocuments([FromBody] SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Pregunta))
            {
                return BadRequest("La pregunta no puede estar vacía.");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                
                Console.WriteLine($"[ChatSearch] Usuario {userName} ({userId}) busca: {request.Pregunta}");

                // Obtener todos los documentos
                var documentos = await _documentoRepository.GetAllAsync();
                
                if (!documentos.Any())
                {
                    return Ok(new { 
                        respuesta = "No hay documentos PDF disponibles en el sistema.",
                        usuario = userName,
                        timestamp = DateTime.UtcNow,
                        documentos_encontrados = 0
                    });
                }

                // Crear contexto con información de los documentos
                var contextoDocumentos = string.Join("\n\n", documentos.Select(d => 
                    $"DOCUMENTO: {d.Titulo}\n" +
                    $"DESCRIPCIÓN: {d.Descripcion}\n" +
                    $"CATEGORÍA: {d.Categoria}\n" +
                    $"CONTENIDO: {(d.Archivo?.Length > 1000 ? d.Archivo.Substring(0, 1000) + "..." : d.Archivo)}"
                ));

                // Crear prompt para búsqueda
                var prompt = $@"Basándote en los siguientes documentos PDF del sistema de onboarding:

{contextoDocumentos}

Usuario pregunta: {request.Pregunta}

Por favor proporciona una respuesta útil basada en el contenido de estos documentos. Si la pregunta es sobre un resumen general, incluye información relevante de todos los documentos.";

                var response = await _ollamaService.GenerateResponseAsync(prompt);
                
                Console.WriteLine($"[ChatSearch] Búsqueda completada para {userName}");
                
                return Ok(new { 
                    respuesta = response,
                    usuario = userName,
                    timestamp = DateTime.UtcNow,
                    documentos_encontrados = documentos.Count(),
                    documentos_consultados = documentos.Select(d => new { 
                        titulo = d.Titulo,
                        categoria = d.Categoria
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChatSearch] Error: {ex.Message}");
                return StatusCode(500, new { error = "Error al procesar la búsqueda en documentos" });
            }
        }
    }
}