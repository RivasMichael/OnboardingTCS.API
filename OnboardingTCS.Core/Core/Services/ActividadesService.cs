using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Services
{
    public class ActividadesService : IActividadesService
    {
        private readonly IActividadesRepository _repository;

        public ActividadesService(IActividadesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ActividadesDto>> GetAllActividadesAsync()
        {
            var actividades = await _repository.GetAllAsync();
            return actividades.Select(a => new ActividadesDto
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Descripcion = a.Descripcion,
                Fecha = a.Fecha,
                Hora = a.Hora,
                Duracion = a.Duracion,
                Tipo = a.Tipo,
                Modalidad = a.Modalidad,
                Lugar = a.Lugar,
                Asignados = a.Asignados,
                Estado = a.Estado,
                CreadoPor = a.CreadoPor,
                CreadoEn = a.CreadoEn
            });
        }

        public async Task<ActividadesDto> GetActividadesByIdAsync(string id)
        {
            var actividad = await _repository.GetByIdAsync(id);
            if (actividad == null) return null;

            return new ActividadesDto
            {
                Id = actividad.Id,
                Titulo = actividad.Titulo,
                Descripcion = actividad.Descripcion,
                Fecha = actividad.Fecha,
                Hora = actividad.Hora,
                Duracion = actividad.Duracion,
                Tipo = actividad.Tipo,
                Modalidad = actividad.Modalidad,
                Lugar = actividad.Lugar,
                Asignados = actividad.Asignados,
                Estado = actividad.Estado,
                CreadoPor = actividad.CreadoPor,
                CreadoEn = actividad.CreadoEn
            };
        }

        public async Task InsertActividadesAsync(ActividadesCreateDto dto)
        {
            var actividad = new Actividades
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Fecha = dto.Fecha,
                Hora = dto.Hora,
                Duracion = dto.Duracion,
                Tipo = dto.Tipo,
                Modalidad = dto.Modalidad,
                Lugar = dto.Lugar,
                Asignados = dto.Asignados,
                Estado = dto.Estado,
                CreadoPor = dto.CreadoPor,
                CreadoEn = DateTime.UtcNow
            };

            await _repository.CreateAsync(actividad);
        }

        public async Task UpdateActividadesAsync(string id, ActividadesUpdateDto dto)
        {
            var actividad = await _repository.GetByIdAsync(id);
            if (actividad == null) return;

            actividad.Titulo = dto.Titulo;
            actividad.Descripcion = dto.Descripcion;
            actividad.Fecha = dto.Fecha;
            actividad.Hora = dto.Hora;
            actividad.Duracion = dto.Duracion;
            actividad.Tipo = dto.Tipo;
            actividad.Modalidad = dto.Modalidad;
            actividad.Lugar = dto.Lugar;
            actividad.Asignados = dto.Asignados;
            actividad.Estado = dto.Estado;

            await _repository.UpdateAsync(id, actividad);
        }

        public async Task DeleteActividadesAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}