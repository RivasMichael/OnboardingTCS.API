using Microsoft.Extensions.DependencyInjection;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Repositories;

namespace OnboardingTCS.API.Extensions
{
    public static class SupervisorModuleExtensions
    {
        public static IServiceCollection AddSupervisoresModule(this IServiceCollection services)
        {
            services.AddScoped<ISupervisorRepository, SupervisorRepository>();
            return services;
        }
    }
}
