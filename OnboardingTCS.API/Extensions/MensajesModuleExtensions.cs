using Microsoft.Extensions.DependencyInjection;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Repositories;

namespace OnboardingTCS.API.Extensions
{
    public static class MensajesModuleExtensions
    {
        public static IServiceCollection AddMensajesModule(this IServiceCollection services)
        {
            services.AddScoped<IMensajesAutomaticosRepository, MensajesAutomaticosRepository>();
            return services;
        }
    }
}
