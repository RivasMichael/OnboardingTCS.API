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
    }
}