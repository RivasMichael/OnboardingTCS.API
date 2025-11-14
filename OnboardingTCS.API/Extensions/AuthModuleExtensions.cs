using Microsoft.Extensions.DependencyInjection;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Repositories;

namespace OnboardingTCS.API.Extensions
{
    public static class AuthModuleExtensions
    {
        public static IServiceCollection AddAuthModule(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            return services;
        }
    }
}