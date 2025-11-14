using Microsoft.Extensions.DependencyInjection;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Repositories;

namespace OnboardingTCS.API.Extensions
{
    public static class ActividadesModuleExtensions
    {
        public static IServiceCollection AddActividadesModule(this IServiceCollection services)
        {
            services.AddScoped<IActividadesRepository, ActividadesRepository>();
            return services;
        }
    }
}
