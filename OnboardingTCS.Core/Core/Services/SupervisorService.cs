using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Services
{
    public class SupervisorService
    {
        private readonly ISupervisorRepository _repository;

        public SupervisorService(ISupervisorRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Supervisor>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Supervisor> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(SupervisorDto supervisorDto)
        {
            var supervisor = new Supervisor
            {
                Nombre = supervisorDto.Nombre,
                Correo = supervisorDto.Correo,
                Cargo = supervisorDto.Cargo,
                Telefono = supervisorDto.Telefono,
                Horario = supervisorDto.Horario,
                MensajeBienvenida = supervisorDto.MensajeBienvenida,
                Departamento = supervisorDto.Departamento,
                FotoPerfil = supervisorDto.FotoPerfil
            };

            await _repository.CreateAsync(supervisor);
        }

        public async Task UpdateAsync(string id, SupervisorDto supervisorDto)
        {
            var existingSupervisor = await _repository.GetByIdAsync(id);
            if (existingSupervisor == null)
                return;

            existingSupervisor.Nombre = supervisorDto.Nombre;
            existingSupervisor.Correo = supervisorDto.Correo;
            existingSupervisor.Cargo = supervisorDto.Cargo;
            existingSupervisor.Telefono = supervisorDto.Telefono;
            existingSupervisor.Horario = supervisorDto.Horario;
            existingSupervisor.MensajeBienvenida = supervisorDto.MensajeBienvenida;
            existingSupervisor.Departamento = supervisorDto.Departamento;
            existingSupervisor.FotoPerfil = supervisorDto.FotoPerfil;

            await _repository.UpdateAsync(id, existingSupervisor);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}