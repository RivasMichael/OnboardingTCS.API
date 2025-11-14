using System.Threading.Tasks;
using OnboardingTCS.Core.Entities;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<Usuario?> GetUserByEmailAsync(string correo);
    }
}
