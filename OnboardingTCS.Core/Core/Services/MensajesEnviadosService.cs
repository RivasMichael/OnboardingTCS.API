using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Services
{
    public class MensajesEnviadosService : IMensajesEnviadosService
    {
        private readonly IMensajesEnviadosRepository _repository;

        public MensajesEnviadosService(IMensajesEnviadosRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MensajesEnviadosDto>> GetAllMensajesAsync()
        {
            var mensajes = await _repository.GetAllAsync();
            return mensajes.Select(m => new MensajesEnviadosDto
            {
                Id = m.Id,
                Mensaje = m.Mensaje,
                Destinatario = m.Destinatario,
                Titulo = m.Titulo,
                Contenido = m.Contenido,
                Prioridad = m.Prioridad,
                Categoria = m.Categoria,
                Leido = m.Leido,
                LeidoEn = m.LeidoEn,
                Favorito = m.Favorito,
                EnviadoEn = m.EnviadoEn,
                CreadoEn = m.CreadoEn
            });
        }

        public async Task<MensajesEnviadosDto> GetMensajeByIdAsync(string id)
        {
            var mensaje = await _repository.GetByIdAsync(id);
            if (mensaje == null) return null;

            return new MensajesEnviadosDto
            {
                Id = mensaje.Id,
                Mensaje = mensaje.Mensaje,
                Destinatario = mensaje.Destinatario,
                Titulo = mensaje.Titulo,
                Contenido = mensaje.Contenido,
                Prioridad = mensaje.Prioridad,
                Categoria = mensaje.Categoria,
                Leido = mensaje.Leido,
                LeidoEn = mensaje.LeidoEn,
                Favorito = mensaje.Favorito,
                EnviadoEn = mensaje.EnviadoEn,
                CreadoEn = mensaje.CreadoEn
            };
        }

        public async Task InsertMensajeAsync(MensajesEnviadosCreateDto dto)
        {
            var mensaje = new MensajesEnviados
            {
                Mensaje = dto.Mensaje,
                Destinatario = dto.Destinatario,
                Titulo = dto.Titulo,
                Contenido = dto.Contenido,
                Prioridad = dto.Prioridad,
                Categoria = dto.Categoria,
                Leido = dto.Leido,
                Favorito = dto.Favorito,
                EnviadoEn = dto.EnviadoEn,
                CreadoEn = DateTime.UtcNow
            };

            await _repository.CreateAsync(mensaje);
        }

        public async Task UpdateMensajeAsync(string id, MensajesEnviadosUpdateDto dto)
        {
            var mensaje = await _repository.GetByIdAsync(id);
            if (mensaje == null) return;

            mensaje.Mensaje = dto.Mensaje;
            mensaje.Destinatario = dto.Destinatario;
            mensaje.Titulo = dto.Titulo;
            mensaje.Contenido = dto.Contenido;
            mensaje.Prioridad = dto.Prioridad;
            mensaje.Categoria = dto.Categoria;
            mensaje.Leido = dto.Leido;
            mensaje.Favorito = dto.Favorito;

            await _repository.UpdateAsync(id, mensaje);
        }

        public async Task DeleteMensajeAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}