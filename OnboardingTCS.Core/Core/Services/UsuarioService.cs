using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Usuario> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Usuario usuario)
        {
            await _repository.CreateAsync(usuario);
        }

        public async Task UpdateAsync(string id, Usuario usuario)
        {
            var existingUsuario = await _repository.GetByIdAsync(id);
            if (existingUsuario == null)
                return;

            usuario.Id = id;
            await _repository.UpdateAsync(id, usuario);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _repository.GetByEmailAsync(email);
        }
    }
}