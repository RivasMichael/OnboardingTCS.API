using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class HistorialChatController : ControllerBase
    {
        private readonly IHistorialChatRepository _chatRepo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MongoDbContext _dbContext;

        public HistorialChatController(IHistorialChatRepository chatRepo, IHttpClientFactory httpClientFactory, MongoDbContext dbContext)
        {
            _chatRepo = chatRepo;
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
        }

        // DTOs used only by this controller
        public class TituloRequest { public string Titulo { get; set; } = string.Empty; }
        public class ContenidoRequest { public string Contenido { get; set; } = string.Empty; }
        public class OllamaResponse { public string? response { get; set; } public string? Response { get; set; } }

        // GET: /api/chat/{correo}
        [HttpGet("{correo}")]
        public async Task<IActionResult> GetConversations(string correo)
        {
            var all = await _chatRepo.GetAllAsync();
            var conversations = all.Where(h => string.Equals(h.UsuarioCorreo?.Trim(), correo?.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(conversations);
        }

        // POST: /api/chat/{correo}
        [HttpPost("{correo}")]
        public async Task<IActionResult> CreateConversation(string correo, [FromBody] TituloRequest request)
        {
            var historial = new HistorialChat
            {
                Id = ObjectId.GenerateNewId(),
                UsuarioCorreo = correo,
                Titulo = request?.Titulo ?? "Sin título",
                Mensajes = new List<ChatMessage>(),
                Favorito = false,
                UltimaActividadEn = DateTime.UtcNow,
                TotalMensajes = 0
            };

            await _chatRepo.CreateAsync(historial);
            return Created($"/api/chat/{correo}/{historial.Id}", historial);
        }

        // POST: /api/chat/{correo}/{chatId}/mensajes
        [HttpPost("{correo}/{chatId}/mensajes")]
        public async Task<IActionResult> PostMessage(string correo, string chatId, [FromBody] ContenidoRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Contenido))
                return BadRequest(new { message = "Contenido requerido." });

            // 1. Obtener conversación
            var historial = await _chatRepo.GetByIdAsync(chatId);
            if (historial == null) return NotFound(new { message = "No se encontró la conversación." });
            if (!string.Equals(historial.UsuarioCorreo?.Trim(), correo?.Trim(), StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "El correo no corresponde a la conversación." });

            // 2. Guardar mensaje de usuario as ChatMessage
            var userMessage = request.Contenido.Trim();
            var userMsgObj = new ChatMessage
            {
                Tipo = "usuario",
                Contenido = userMessage,
                Timestamp = DateTime.UtcNow
            };
            historial.Mensajes.Add(userMsgObj);
            historial.TotalMensajes = historial.Mensajes.Count;
            historial.UltimaActividadEn = DateTime.UtcNow;
            await _chatRepo.UpdateAsync(chatId, historial);

            // 3. Preparar prompt para Ollama.
            string contexto = "No tengo información sobre eso.";
            try
            {
                var collection = _dbContext.mensajes_automaticos;
                var filtro = Builders<MensajesAutomaticos>.Filter.Eq("titulo", "Políticas");
                var doc = await collection.Find(filtro).FirstOrDefaultAsync();
                if (doc != null && !string.IsNullOrWhiteSpace(doc.Contenido))
                {
                    contexto = doc.Contenido;
                }
            }
            catch
            {
                // ignore and keep default contexto
            }

            string prompt = $"Basándote únicamente en este contexto: '{contexto}'. Responde: '{userMessage}'";

            // 4. Llamar a Ollama
            var httpClient = _httpClientFactory.CreateClient();
            var ollamaUrl = "http://localhost:11434/api/generate";
            var ollamaRequest = new { model = "tinyllama", prompt = prompt, stream = false };
            var requestContent = new StringContent(JsonSerializer.Serialize(ollamaRequest), Encoding.UTF8, "application/json");

            OllamaResponse? ollamaResponse;
            try
            {
                var response = await httpClient.PostAsync(ollamaUrl, requestContent);
                if (!response.IsSuccessStatusCode)
                    return Problem("Error llamando a Ollama.");

                var text = await response.Content.ReadAsStringAsync();
                try { ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(text); }
                catch { ollamaResponse = new OllamaResponse { response = text }; }
            }
            catch (Exception ex)
            {
                return Problem($"No se pudo conectar a Ollama. Error: {ex.Message}");
            }

            string respuestaIA = ollamaResponse?.response ?? ollamaResponse?.Response ?? "No se pudo generar una respuesta.";

            // 5. Guardar mensaje del bot as ChatMessage
            var botMsgObj = new ChatMessage
            {
                Tipo = "bot",
                Contenido = respuestaIA,
                Timestamp = DateTime.UtcNow
            };
            historial.Mensajes.Add(botMsgObj);
            historial.TotalMensajes = historial.Mensajes.Count;
            historial.UltimaActividadEn = DateTime.UtcNow;
            await _chatRepo.UpdateAsync(chatId, historial);

            // 6. Devolver la respuesta al frontend
            return Ok(new { tipo = "bot", contenido = respuestaIA });
        }

        // --- ¡NUEVO! ESTA ES LA "CARRETERA" PUT QUE FALTABA ---
        [HttpPut("{correo}/{chatId}")]
        public async Task<IActionResult> ActualizarChat(string correo, string chatId, [FromBody] TituloRequest request)
        {
            var historial = await _chatRepo.GetByIdAsync(chatId);
            if (historial == null)
                return NotFound(new { message = "No se encontró el chat." });

            // Chequeo de seguridad
            if (!string.Equals(historial.UsuarioCorreo?.Trim(), correo?.Trim(), StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "El correo no corresponde a la conversación." });

            // Actualizar el título y la fecha
            historial.Titulo = request.Titulo;
            historial.UltimaActividadEn = DateTime.UtcNow;

            await _chatRepo.UpdateAsync(chatId, historial);
            return NoContent(); // Éxito (Respuesta estándar para PUT)
        }

        // --- ¡NUEVO! ESTA ES LA "CARRETERA" DELETE QUE FALTABA ---
        // --- ¡NUEVO! ESTA ES LA "CARRETERA" DELETE (CORREGIDA) ---
        [HttpDelete("{correo}/{chatId}")]
        public async Task<IActionResult> BorrarChat(string correo, string chatId)
        {
            var historial = await _chatRepo.GetByIdAsync(chatId);
            if (historial == null)
                return NotFound(new { message = "No se encontró el chat." });

            // Chequeo de seguridad
            if (!string.Equals(historial.UsuarioCorreo?.Trim(), correo?.Trim(), StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "El correo no corresponde a la conversación." });


            // --- ¡ESTA ES LA CORRECCIÓN PARA EL ERROR CS1660! ---

            // 1. Convertir el string "chatId" a un ObjectId real
            if (!ObjectId.TryParse(chatId, out ObjectId objectId))
            {
                return BadRequest("El formato del Id del chat no es válido.");
            }

            // 2. Usar el 'objectId' (no el 'chatId') en el filtro
            var filter = Builders<HistorialChat>.Filter.Eq(h => h.Id, objectId);
            var result = await _dbContext.historial_chat.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return NotFound("No se encontró el chat para borrar (usando dbContext).");
            }

            return NoContent(); // Éxito
        }
        }
    }