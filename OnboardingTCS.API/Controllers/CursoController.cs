using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints
    public class CursoController : ControllerBase
    {
        private readonly ICursoService _cursoService;

        public CursoController(ICursoService cursoService)
        {
            _cursoService = cursoService;
        }

        /// <summary>
        /// Obtiene cursos con solo los campos esenciales: nivel, titulo, descripcion, duracion, categoria, instructor, link
        /// Soporta filtrado opcional por categoría y/o nivel
        /// </summary>
        /// <param name="categoria">Categoría del curso (opcional). Usar "Todas" para no filtrar</param>
        /// <param name="nivel">Nivel del curso (opcional). Usar "Todos" para no filtrar</param>
        /// <returns>Lista de cursos con campos esenciales</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CursoSimpleDto>>> GetCursos(
            [FromQuery] string categoria = null, 
            [FromQuery] string nivel = null)
        {
            try
            {
                var cursos = await _cursoService.GetCursosSimpleAsync(categoria, nivel);
                return Ok(cursos);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todas las categorías disponibles
        /// </summary>
        /// <returns>Lista de categorías únicas</returns>
        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategorias()
        {
            try
            {
                var categorias = await _cursoService.GetCategoriasAsync();
                return Ok(categorias);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todos los niveles disponibles
        /// </summary>
        /// <returns>Lista de niveles únicos</returns>
        [HttpGet("niveles")]
        public async Task<ActionResult<IEnumerable<string>>> GetNiveles()
        {
            try
            {
                var niveles = await _cursoService.GetNivelesAsync();
                return Ok(niveles);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}