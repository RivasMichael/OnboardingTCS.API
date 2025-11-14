using Microsoft.Extensions.DependencyInjection;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Repositories;

namespace OnboardingTCS.API.Extensions
{
    public static class ChatModuleExtensions
    {
        public static IServiceCollection AddChatModule(this IServiceCollection services)
        {
            // Registrar aquí el repositorio correcto para historial de chat
            services.AddScoped<IHistorialChatRepository, HistorialChatRepository>();
            return services;
        }
    }
}