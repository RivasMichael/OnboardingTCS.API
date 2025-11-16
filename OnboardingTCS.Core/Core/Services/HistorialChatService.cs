using OnboardingTCS.Core.Core.DTOs;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Services
{
    public class HistorialChatService : IHistorialChatService
    {
        private readonly IHistorialChatRepository _repository;

        public HistorialChatService(IHistorialChatRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<HistorialChatDto>> GetAllHistorialChatAsync()
        {
            var historiales = await _repository.GetAllAsync();
            return historiales.Select(h => new HistorialChatDto
            {
                Id = h.Id,
                UsuarioCorreo = h.UsuarioCorreo,
                Titulo = h.Titulo,
                Mensajes = h.Mensajes.Select(m => new MensajeChatDto
                {
                    Tipo = m.Tipo,
                    Contenido = m.Contenido,
                    Timestamp = m.Timestamp
                }).ToList(),
                Favorito = h.Favorito,
                UltimaActividadEn = h.UltimaActividadEn,
                TotalMensajes = h.TotalMensajes
            });
        }

        public async Task<HistorialChatDto> GetHistorialChatByIdAsync(string id)
        {
            var historial = await _repository.GetByIdAsync(id);
            if (historial == null) return null;

            return new HistorialChatDto
            {
                Id = historial.Id,
                UsuarioCorreo = historial.UsuarioCorreo,
                Titulo = historial.Titulo,
                Mensajes = historial.Mensajes.Select(m => new MensajeChatDto
                {
                    Tipo = m.Tipo,
                    Contenido = m.Contenido,
                    Timestamp = m.Timestamp
                }).ToList(),
                Favorito = historial.Favorito,
                UltimaActividadEn = historial.UltimaActividadEn,
                TotalMensajes = historial.TotalMensajes
            };
        }

        public async Task<IEnumerable<HistorialChatDto>> GetHistorialChatByUsuarioAsync(string usuarioCorreo)
        {
            var historiales = await _repository.GetAllAsync();
            var historialesFiltrados = historiales.Where(h => h.UsuarioCorreo == usuarioCorreo);
            
            return historialesFiltrados.Select(h => new HistorialChatDto
            {
                Id = h.Id,
                UsuarioCorreo = h.UsuarioCorreo,
                Titulo = h.Titulo,
                Mensajes = h.Mensajes.Select(m => new MensajeChatDto
                {
                    Tipo = m.Tipo,
                    Contenido = m.Contenido,
                    Timestamp = m.Timestamp
                }).ToList(),
                Favorito = h.Favorito,
                UltimaActividadEn = h.UltimaActividadEn,
                TotalMensajes = h.TotalMensajes
            });
        }

        public async Task CreateHistorialChatAsync(HistorialChatCreateDto dto)
        {
            var historial = new HistorialChat
            {
                UsuarioCorreo = dto.UsuarioCorreo,
                Titulo = dto.Titulo,
                Mensajes = dto.Mensajes?.Select(m => new MensajeChat
                {
                    Tipo = m.Tipo,
                    Contenido = m.Contenido,
                    Timestamp = m.Timestamp
                }).ToList() ?? new List<MensajeChat>(),
                Favorito = dto.Favorito,
                UltimaActividadEn = DateTime.UtcNow,
                TotalMensajes = dto.Mensajes?.Count ?? 0
            };

            await _repository.CreateAsync(historial);
        }

        public async Task UpdateHistorialChatAsync(string id, HistorialChatUpdateDto dto)
        {
            var historial = await _repository.GetByIdAsync(id);
            if (historial == null) return;

            historial.UsuarioCorreo = dto.UsuarioCorreo;
            historial.Titulo = dto.Titulo;
            historial.Mensajes = dto.Mensajes?.Select(m => new MensajeChat
            {
                Tipo = m.Tipo,
                Contenido = m.Contenido,
                Timestamp = m.Timestamp
            }).ToList() ?? new List<MensajeChat>();
            historial.Favorito = dto.Favorito;
            historial.UltimaActividadEn = DateTime.UtcNow;
            historial.TotalMensajes = dto.Mensajes?.Count ?? 0;

            await _repository.UpdateAsync(id, historial);
        }

        public async Task DeleteHistorialChatAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}