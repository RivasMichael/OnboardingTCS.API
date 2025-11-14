using OnboardingTCS.Core.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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
                var payload = new { model = "gemma3:4b", prompt, stream = false };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OllamaResponse>(responseContent, _jsonOpts);

                return string.IsNullOrWhiteSpace(result?.Response)
                    ? "No se pudo obtener una respuesta."
                    : result.Response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al llamar al servicio Ollama: {ex.Message}");
                throw new Exception("Error al llamar al servicio Ollama", ex);
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