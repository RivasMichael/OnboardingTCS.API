using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync()
        {
            var usuarios = await _repository.GetAllAsync();
            return usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Correo = u.Correo,
                Rol = u.Rol,
                FechaInicio = u.FechaInicio,
                Cargo = u.Cargo,
                Departamento = u.Departamento,
                CodigoEmpleado = u.CodigoEmpleado,
                SupervisorCorreo = u.SupervisorCorreo,
                Oficina = u.Oficina,
                Estado = u.Estado,
                CreadoEn = u.CreadoEn
            });
        }

        public async Task<UsuarioDto> GetUsuarioByIdAsync(string id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return null;

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                FechaInicio = usuario.FechaInicio,
                Cargo = usuario.Cargo,
                Departamento = usuario.Departamento,
                CodigoEmpleado = usuario.CodigoEmpleado,
                SupervisorCorreo = usuario.SupervisorCorreo,
                Oficina = usuario.Oficina,
                Estado = usuario.Estado,
                CreadoEn = usuario.CreadoEn
            };
        }

        public async Task InsertUsuarioAsync(UsuarioCreateDto dto)
        {
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Contrasena = dto.Contrasena,
                Rol = dto.Rol,
                FechaInicio = dto.FechaInicio,
                Cargo = dto.Cargo,
                Departamento = dto.Departamento,
                CodigoEmpleado = dto.CodigoEmpleado,
                SupervisorCorreo = dto.SupervisorCorreo,
                Oficina = dto.Oficina,
                Estado = dto.Estado,
                CreadoEn = DateTime.UtcNow
            };

            await _repository.CreateAsync(usuario);
        }

        public async Task UpdateUsuarioAsync(string id, UsuarioUpdateDto dto)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return;

            usuario.Nombre = dto.Nombre;
            usuario.Correo = dto.Correo;
            usuario.Contrasena = dto.Contrasena;
            usuario.Rol = dto.Rol;
            usuario.FechaInicio = dto.FechaInicio;
            usuario.Cargo = dto.Cargo;
            usuario.Departamento = dto.Departamento;
            usuario.CodigoEmpleado = dto.CodigoEmpleado;
            usuario.SupervisorCorreo = dto.SupervisorCorreo;
            usuario.Oficina = dto.Oficina;
            usuario.Estado = dto.Estado;

            await _repository.UpdateAsync(id, usuario);
        }

        public async Task DeleteUsuarioAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}