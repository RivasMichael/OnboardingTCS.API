using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Core.Services;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActividadesController : ControllerBase
    {
        private readonly IActividadesService _service;

        public ActividadesController(IActividadesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActividadesDto>>> GetAll()
        {
            var actividades = await _service.GetAllActividadesAsync();
            return Ok(actividades);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActividadesDto>> GetById(string id)
        {
            var actividad = await _service.GetActividadesByIdAsync(id);
            if (actividad == null) return NotFound();
            return Ok(actividad);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ActividadesCreateDto dto)
        {
            var actividad = new ActividadesDto
            {
                Id = Guid.NewGuid().ToString(),
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Fecha = dto.Fecha,
                Hora = dto.Hora,
                Duracion = dto.Duracion,
                Tipo = dto.Tipo,
                Modalidad = dto.Modalidad,
                Lugar = dto.Lugar
            };
            await _service.InsertActividadesAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = actividad.Id }, actividad);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, ActividadesUpdateDto dto)
        {
            await _service.UpdateActividadesAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteActividadesAsync(id);
            return NoContent();
        }
    }
}
