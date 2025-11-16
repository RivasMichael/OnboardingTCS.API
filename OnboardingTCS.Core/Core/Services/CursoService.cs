using OnboardingTCS.Core.Core.DTOs;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingTCS.Core.Core.Services
{
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _repository;

        public CursoService(ICursoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CursoListDto>> GetAllCursosAsync()
        {
            var cursos = await _repository.GetAllAsync();
            return cursos.Select(c => new CursoListDto
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                Duracion = c.Duracion,
                Nivel = c.Nivel,
                Categoria = c.Categoria,
                Instructor = c.Instructor,
                Link = c.Link,
                Inscritos = c.Inscritos
            });
        }

        public async Task<IEnumerable<CursoSimpleDto>> GetCursosSimpleAsync(string categoria = null, string nivel = null)
        {
            IEnumerable<Curso> cursos;

            // Si no se especifican filtros, obtener todos
            if (string.IsNullOrEmpty(categoria) || categoria.ToLower() == "todas")
            {
                if (string.IsNullOrEmpty(nivel) || nivel.ToLower() == "todos")
                {
                    cursos = await _repository.GetAllAsync();
                }
                else
                {
                    cursos = await _repository.GetByNivelAsync(nivel);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(nivel) || nivel.ToLower() == "todos")
                {
                    cursos = await _repository.GetByCategoriaAsync(categoria);
                }
                else
                {
                    cursos = await _repository.GetByCategoriaAndNivelAsync(categoria, nivel);
                }
            }

            return cursos.Where(c => c.Activo).Select(c => new CursoSimpleDto
            {
                Nivel = c.Nivel,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                Duracion = c.Duracion,
                Categoria = c.Categoria,
                Instructor = c.Instructor,
                Link = c.Link
            });
        }

        public async Task<IEnumerable<CursoCompletDto>> GetAllCursosCompletosAsync()
        {
            var cursos = await _repository.GetAllAsync();
            return cursos.Where(c => c.Activo).Select(c => new CursoCompletDto
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                Duracion = c.Duracion,
                Nivel = c.Nivel,
                Categoria = c.Categoria,
                Instructor = c.Instructor,
                Link = c.Link,
                Recomendado = c.Recomendado,
                RecomendadoPara = c.RecomendadoPara ?? new List<string>(),
                Inscritos = c.Inscritos,
                Activo = c.Activo
            });
        }

        public async Task<IEnumerable<CursoListDto>> GetCursosFiltradosAsync(string categoria = null, string nivel = null)
        {
            IEnumerable<Curso> cursos;

            // Si no se especifican filtros, obtener todos
            if (string.IsNullOrEmpty(categoria) || categoria.ToLower() == "todas")
            {
                if (string.IsNullOrEmpty(nivel) || nivel.ToLower() == "todos")
                {
                    cursos = await _repository.GetAllAsync();
                }
                else
                {
                    cursos = await _repository.GetByNivelAsync(nivel);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(nivel) || nivel.ToLower() == "todos")
                {
                    cursos = await _repository.GetByCategoriaAsync(categoria);
                }
                else
                {
                    cursos = await _repository.GetByCategoriaAndNivelAsync(categoria, nivel);
                }
            }

            return cursos.Where(c => c.Activo).Select(c => new CursoListDto
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                Duracion = c.Duracion,
                Nivel = c.Nivel,
                Categoria = c.Categoria,
                Instructor = c.Instructor,
                Link = c.Link,
                Inscritos = c.Inscritos
            });
        }

        public async Task<IEnumerable<CursoCompletDto>> GetCursosCompletosFiltraodosAsync(string categoria = null, string nivel = null)
        {
            IEnumerable<Curso> cursos;

            // Si no se especifican filtros, obtener todos
            if (string.IsNullOrEmpty(categoria) || categoria.ToLower() == "todas")
            {
                if (string.IsNullOrEmpty(nivel) || nivel.ToLower() == "todos")
                {
                    cursos = await _repository.GetAllAsync();
                }
                else
                {
                    cursos = await _repository.GetByNivelAsync(nivel);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(nivel) || nivel.ToLower() == "todos")
                {
                    cursos = await _repository.GetByCategoriaAsync(categoria);
                }
                else
                {
                    cursos = await _repository.GetByCategoriaAndNivelAsync(categoria, nivel);
                }
            }

            return cursos.Where(c => c.Activo).Select(c => new CursoCompletDto
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                Duracion = c.Duracion,
                Nivel = c.Nivel,
                Categoria = c.Categoria,
                Instructor = c.Instructor,
                Link = c.Link,
                Recomendado = c.Recomendado,
                RecomendadoPara = c.RecomendadoPara ?? new List<string>(),
                Inscritos = c.Inscritos,
                Activo = c.Activo
            });
        }

        public async Task<IEnumerable<string>> GetCategoriasAsync()
        {
            var categorias = await _repository.GetCategoriasAsync();
            return categorias.ToList();
        }

        public async Task<IEnumerable<string>> GetNivelesAsync()
        {
            var niveles = await _repository.GetNivelesAsync();
            return niveles.ToList();
        }

        public async Task<CursoDetailDto> GetCursoByIdAsync(string id)
        {
            var curso = await _repository.GetByIdAsync(id);
            if (curso == null) return null;

            return new CursoDetailDto
            {
                Id = curso.Id,
                Titulo = curso.Titulo,
                Descripcion = curso.Descripcion,
                Duracion = curso.Duracion,
                Nivel = curso.Nivel,
                Categoria = curso.Categoria,
                Instructor = curso.Instructor,
                Link = curso.Link,
                Recomendado = curso.Recomendado,
                RecomendadoPara = curso.RecomendadoPara,
                CreadoPor = curso.CreadoPor,
                Activo = curso.Activo,
                Inscritos = curso.Inscritos
            };
        }

        public async Task CreateCursoAsync(CursoCreateDto dto)
        {
            var curso = new Curso
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Duracion = dto.Duracion,
                Nivel = dto.Nivel,
                Categoria = dto.Categoria,
                Instructor = dto.Instructor,
                Link = dto.Link,
                Recomendado = dto.Recomendado,
                RecomendadoPara = dto.RecomendadoPara,
                Activo = true,
                Inscritos = 0
            };

            await _repository.CreateAsync(curso);
        }

        public async Task UpdateCursoAsync(string id, CursoUpdateDto dto)
        {
            var curso = await _repository.GetByIdAsync(id);
            if (curso == null) return;

            curso.Titulo = dto.Titulo;
            curso.Descripcion = dto.Descripcion;
            curso.Duracion = dto.Duracion;
            curso.Nivel = dto.Nivel;
            curso.Categoria = dto.Categoria;
            curso.Instructor = dto.Instructor;
            curso.Link = dto.Link;
            curso.Recomendado = dto.Recomendado;
            curso.RecomendadoPara = dto.RecomendadoPara;
            curso.Activo = dto.Activo;

            await _repository.UpdateAsync(id, curso);
        }

        public async Task DeleteCursoAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}