using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Services
{
    public class SupervisorService : ISupervisorService
    {
        private readonly ISupervisorRepository _repository;

        public SupervisorService(ISupervisorRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SupervisorDto>> GetAllSupervisorsAsync()
        {
            var supervisors = await _repository.GetAllAsync();
            return supervisors.Select(s => new SupervisorDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Correo = s.Correo,
                Cargo = s.Cargo,
                Telefono = s.Telefono,
                Horario = s.Horario,
                MensajeBienvenida = s.MensajeBienvenida,
                Departamento = s.Departamento,
                FotoPerfil = s.FotoPerfil
            });
        }

        public async Task<SupervisorDto> GetSupervisorByIdAsync(string id)
        {
            var supervisor = await _repository.GetByIdAsync(id);
            if (supervisor == null) return null;

            return new SupervisorDto
            {
                Id = supervisor.Id,
                Nombre = supervisor.Nombre,
                Correo = supervisor.Correo,
                Cargo = supervisor.Cargo,
                Telefono = supervisor.Telefono,
                Horario = supervisor.Horario,
                MensajeBienvenida = supervisor.MensajeBienvenida,
                Departamento = supervisor.Departamento,
                FotoPerfil = supervisor.FotoPerfil
            };
        }

        public async Task InsertSupervisorAsync(SupervisorCreateDto dto)
        {
            var supervisor = new Supervisor
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Departamento = dto.Departamento
            };

            await _repository.CreateAsync(supervisor);
        }

        public async Task UpdateSupervisorAsync(string id, SupervisorUpdateDto dto)
        {
            var supervisor = await _repository.GetByIdAsync(id);
            if (supervisor == null) return;

            supervisor.Nombre = dto.Nombre;
            supervisor.Correo = dto.Correo;
            supervisor.Departamento = dto.Departamento;

            await _repository.UpdateAsync(id, supervisor);
        }

        public async Task DeleteSupervisorAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Supervisor>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Supervisor> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Supervisor supervisor)
        {
            await _repository.CreateAsync(supervisor);
        }

        public async Task UpdateAsync(string id, Supervisor supervisor)
        {
            await _repository.UpdateAsync(id, supervisor);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}