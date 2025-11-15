using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Services
{
    public class MensajesAutomaticosService : IMensajesAutomaticosService
    {
        private readonly IMensajesAutomaticosRepository _repository;

        public MensajesAutomaticosService(IMensajesAutomaticosRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MensajesAutomaticosDto>> GetAllMensajesAsync()
        {
            var mensajes = await _repository.GetAllAsync();
            return mensajes.Select(m => new MensajesAutomaticosDto
            {
                Id = m.Id.ToString(),
                Titulo = m.Titulo,
                Contenido = m.Contenido,
                Prioridad = m.Prioridad,
                Categoria = m.Categoria,
                TipoDisparo = m.TipoDisparo,
                DiasAntesInicio = m.DiasAntesInicio,
                RolObjetivo = m.RolObjetivo,
                CreadoPor = m.CreadoPor,
                Activo = m.Activo,
                CreadoEn = m.CreadoEn,
                InactividadDias = m.InactividadDias
            });
        }

        public async Task<MensajesAutomaticosDto> GetMensajeByIdAsync(string id)
        {
            var mensaje = await _repository.GetByIdAsync(id);
            if (mensaje == null) return null;

            return new MensajesAutomaticosDto
            {
                Id = mensaje.Id.ToString(),
                Titulo = mensaje.Titulo,
                Contenido = mensaje.Contenido,
                Prioridad = mensaje.Prioridad,
                Categoria = mensaje.Categoria,
                TipoDisparo = mensaje.TipoDisparo,
                DiasAntesInicio = mensaje.DiasAntesInicio,
                RolObjetivo = mensaje.RolObjetivo,
                CreadoPor = mensaje.CreadoPor,
                Activo = mensaje.Activo,
                CreadoEn = mensaje.CreadoEn,
                InactividadDias = mensaje.InactividadDias
            };
        }

        public async Task InsertMensajeAsync(MensajesAutomaticosCreateDto dto)
        {
            var mensaje = new MensajesAutomaticos
            {
                Titulo = dto.Titulo,
                Contenido = dto.Contenido,
                Prioridad = dto.Prioridad,
                Categoria = dto.Categoria,
                TipoDisparo = dto.TipoDisparo,
                DiasAntesInicio = dto.DiasAntesInicio,
                RolObjetivo = dto.RolObjetivo,
                CreadoPor = dto.CreadoPor,
                Activo = dto.Activo,
                CreadoEn = DateTime.UtcNow,
                InactividadDias = dto.InactividadDias
            };

            await _repository.CreateAsync(mensaje);
        }

        public async Task UpdateMensajeAsync(string id, MensajesAutomaticosUpdateDto dto)
        {
            var mensaje = await _repository.GetByIdAsync(id);
            if (mensaje == null) return;

            mensaje.Titulo = dto.Titulo;
            mensaje.Contenido = dto.Contenido;
            mensaje.Prioridad = dto.Prioridad;
            mensaje.Categoria = dto.Categoria;
            mensaje.TipoDisparo = dto.TipoDisparo;
            mensaje.DiasAntesInicio = dto.DiasAntesInicio;
            mensaje.RolObjetivo = dto.RolObjetivo;
            mensaje.Activo = dto.Activo;
            mensaje.InactividadDias = dto.InactividadDias;

            await _repository.UpdateAsync(id, mensaje);
        }

        public async Task DeleteMensajeAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}