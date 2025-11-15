using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using OnboardingTCS.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace OnboardingTCS.Core.Core.Services
{
    public class LikesCursosService : ILikesCursosService
    {
        private readonly ILikesCursosRepository _repository;

        public LikesCursosService(ILikesCursosRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LikesCursosDto>> GetAllLikesAsync()
        {
            var likes = await _repository.GetAllAsync();
            return likes.Select(l => new LikesCursosDto
            {
                Id = l.Id,
                UsuarioCorreo = l.UsuarioCorreo,
                CursoTitulo = l.CursoTitulo,
                LikeadoEn = l.LikeadoEn
            });
        }

        public async Task<LikesCursosDto> GetLikeByIdAsync(string id)
        {
            var like = await _repository.GetByIdAsync(id);
            if (like == null) return null;

            return new LikesCursosDto
            {
                Id = like.Id,
                UsuarioCorreo = like.UsuarioCorreo,
                CursoTitulo = like.CursoTitulo,
                LikeadoEn = like.LikeadoEn
            };
        }

        public async Task InsertLikeAsync(LikesCursosCreateDto dto)
        {
            var like = new LikesCursos
            {
                UsuarioCorreo = dto.UsuarioCorreo,
                CursoTitulo = dto.CursoTitulo,
                LikeadoEn = dto.LikeadoEn
            };

            await _repository.CreateAsync(like);
        }

        public async Task UpdateLikeAsync(string id, LikesCursosUpdateDto dto)
        {
            var like = await _repository.GetByIdAsync(id);
            if (like == null) return;

            like.UsuarioCorreo = dto.UsuarioCorreo;
            like.CursoTitulo = dto.CursoTitulo;
            like.LikeadoEn = dto.LikeadoEn;

            await _repository.UpdateAsync(id, like);
        }

        public async Task DeleteLikeAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}