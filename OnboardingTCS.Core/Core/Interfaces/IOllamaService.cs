using System.Threading.Tasks;

namespace OnboardingTCS.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Ollama.
    /// </summary>
    public interface IOllamaService
    {
        /// <summary>
        /// Envía un prompt a Ollama y devuelve la respuesta generada.
        /// </summary>
        /// <param name="prompt">El texto o pregunta que se enviará a Ollama.</param>
        /// <returns>La respuesta generada por Ollama.</returns>
        Task<string> GenerateResponseAsync(string prompt);

        /// <summary>
        /// Genera embeddings a partir de un texto dado.
        /// </summary>
        /// <param name="text">El texto del cual se generarán los embeddings.</param>
        /// <returns>Un arreglo de flotantes representando los embeddings.</returns>
        Task<float[]> GenerateEmbeddingAsync(string text);
    }
}