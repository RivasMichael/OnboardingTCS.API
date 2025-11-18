using OnboardingTCS.Core.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OnboardingTCS.Core.Infrastructure.Services
{
    /// <summary>micahel
    /// Servicio para interactuar con la API de Ollama.
    /// </summary>
    public class OllamaService : IOllamaService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Envía un prompt a Ollama y devuelve la respuesta generada.
        /// </summary>
        /// <param name="prompt">El texto o pregunta que se enviará a Ollama.</param>
        /// <returns>La respuesta generada por Ollama.</returns>
        public async Task<string> GenerateResponseAsync(string prompt)
        {
            try
            {
                Console.WriteLine($"[OllamaService] Iniciando generación de respuesta");
                Console.WriteLine($"[OllamaService] HttpClient.Timeout configurado: {_httpClient.Timeout}");
                Console.WriteLine($"[OllamaService] Prompt: {prompt.Substring(0, Math.Min(prompt.Length, 100))}...");
                
                var payload = new { model = "gemma3:4b", prompt, stream = false };
                var jsonPayload = JsonSerializer.Serialize(payload);
                
                Console.WriteLine($"[OllamaService] Payload JSON: {jsonPayload.Substring(0, Math.Min(jsonPayload.Length, 200))}...");
                
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                Console.WriteLine($"[OllamaService] Enviando petición a http://localhost:11434/api/generate");
                
                // ?? Crear un CancellationToken personalizado con 20 minutos para modelo grande
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(20));
                
                using var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content, cts.Token);
                
                Console.WriteLine($"[OllamaService] Status de respuesta: {response.StatusCode}");
                Console.WriteLine($"[OllamaService] Headers de respuesta: {string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[OllamaService] Error de Ollama: {errorContent}");
                    throw new Exception($"Error de Ollama API: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[OllamaService] Respuesta de Ollama (primeros 200 chars): {responseContent.Substring(0, Math.Min(responseContent.Length, 200))}...");
                
                var result = JsonSerializer.Deserialize<OllamaResponse>(responseContent, _jsonOpts);

                if (result == null)
                {
                    Console.WriteLine($"[OllamaService] Error: No se pudo deserializar la respuesta");
                    return "Error: No se pudo procesar la respuesta de Ollama.";
                }

                Console.WriteLine($"[OllamaService] Respuesta deserializada exitosamente");
                
                return string.IsNullOrWhiteSpace(result.Response)
                    ? "No se pudo obtener una respuesta."
                    : result.Response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[OllamaService] HttpRequestException: {ex.Message}");
                Console.WriteLine($"[OllamaService] HttpRequestException StackTrace: {ex.StackTrace}");
                throw new Exception($"Error de conectividad con Ollama: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                Console.WriteLine($"[OllamaService] TaskCanceledException (timeout): {ex.Message}");
                throw new Exception("Timeout al conectar con Ollama - el modelo gemma3:4b está tardando demasiado en responder. Esto es normal en la primera ejecución.", ex);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"[OllamaService] TaskCanceledException (cancelado): {ex.Message}");
                throw new Exception("La operación fue cancelada. Esto puede indicar que Ollama no está respondiendo correctamente.", ex);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[OllamaService] JsonException: {ex.Message}");
                throw new Exception($"Error al procesar respuesta JSON de Ollama: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OllamaService] Exception general: {ex.Message}");
                Console.WriteLine($"[OllamaService] Exception tipo: {ex.GetType().Name}");
                Console.WriteLine($"[OllamaService] Exception StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var payload = new { model = "nomic-embed-text", text };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync("http://localhost:11434/api/embeddings", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<EmbeddingResponse>(responseContent, _jsonOpts);

            return result?.Embedding ?? throw new Exception("No se pudo generar el embedding.");
        }
    }

    /// <summary>
    /// Representa un fragmento de la respuesta de Ollama.
    /// </summary>
    public class OllamaResponseFragment
    {
        [JsonPropertyName("model")] public string Model { get; set; }
        [JsonPropertyName("response")] public string Response { get; set; }
        [JsonPropertyName("done")] public bool Done { get; set; }
        [JsonPropertyName("done_reason")] public string DoneReason { get; set; }
        [JsonPropertyName("context")] public List<int> Context { get; set; }
    }

    public class OllamaResponse
    {
        [JsonPropertyName("model")] public string Model { get; set; }
        [JsonPropertyName("response")] public string Response { get; set; }
        [JsonPropertyName("done")] public bool Done { get; set; }
        [JsonPropertyName("done_reason")] public string DoneReason { get; set; }
        [JsonPropertyName("context")] public List<int> Context { get; set; }
    }

    public class EmbeddingResponse
    {
        [JsonPropertyName("embedding")] public float[] Embedding { get; set; }
    }
}