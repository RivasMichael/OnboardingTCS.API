using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Core.Services;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.Linq;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActividadesController : ControllerBase
    {
        private readonly IActividadesService _service;

        public ActividadesController(IActividadesService service)
        {
            _service = service;
        }

        [HttpGet("mis-actividades")]
        public async Task<ActionResult> GetMisActividades([FromQuery] string? estado = null)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    return BadRequest("No se pudo obtener el email del usuario");
                }

                var todasActividades = await _service.GetAllActividadesAsync();

                var misActividades = todasActividades
                    .Where(a => a.Asignados != null && a.Asignados.Contains(userEmail, StringComparer.OrdinalIgnoreCase))
                    .Where(a => string.IsNullOrEmpty(estado) || a.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(a => a.Fecha)
                    .Select(a => new {
                        id = a.Id,
                        titulo = a.Titulo,
                        descripcion = a.Descripcion,
                        fecha = a.Fecha.ToString("yyyy-MM-dd"),
                        hora = a.Hora,
                        duracion = a.Duracion,
                        tipo = a.Tipo,
                        modalidad = a.Modalidad,
                        lugar = a.Lugar,
                        estado = a.Estado,
                        creado_por = a.CreadoPor,
                        es_urgente = a.Fecha.Date <= DateTime.Today.AddDays(2) && a.Estado == "pendiente"
                    })
                    .ToList();

                return Ok(new {
                    actividades = misActividades,
                    resumen = new {
                        total = misActividades.Count,
                        pendientes = misActividades.Count(a => a.estado == "pendiente"),
                        completadas = misActividades.Count(a => a.estado == "completada"),
                        urgentes = misActividades.Count(a => a.es_urgente)
                    },
                    usuario = userName
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener actividades del usuario" });
            }
        }

        [HttpPost("{id}/asignar")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> AsignarActividad(string id, [FromBody] AsignarActividadRequest request)
        {
            try
            {
                if (request.UsuariosEmails == null || !request.UsuariosEmails.Any())
                {
                    return BadRequest("Debe especificar al menos un usuario para asignar");
                }

                var actividad = await _service.GetActividadesByIdAsync(id);
                if (actividad == null)
                {
                    return NotFound("Actividad no encontrada");
                }

                var nuevosAsignados = actividad.Asignados?.ToList() ?? new List<string>();
                foreach (var email in request.UsuariosEmails)
                {
                    if (!nuevosAsignados.Contains(email, StringComparer.OrdinalIgnoreCase))
                    {
                        nuevosAsignados.Add(email);
                    }
                }

                var updateDto = new ActividadesUpdateDto
                {
                    Titulo = actividad.Titulo,
                    Descripcion = actividad.Descripcion,
                    Fecha = actividad.Fecha,
                    Hora = actividad.Hora,
                    Duracion = actividad.Duracion,
                    Tipo = actividad.Tipo,
                    Modalidad = actividad.Modalidad,
                    Lugar = actividad.Lugar,
                    Asignados = nuevosAsignados,
                    Estado = actividad.Estado,
                    CreadoPor = actividad.CreadoPor,
                    CreadoEn = actividad.CreadoEn
                };

                await _service.UpdateActividadesAsync(id, updateDto);

                return Ok(new {
                    mensaje = "Actividad asignada exitosamente",
                    actividad_id = id,
                    usuarios_asignados = nuevosAsignados
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al asignar actividad" });
            }
        }

        [HttpPatch("{id}/completar")]
        [Authorize] // Permitir acceso a usuarios autenticados
        public async Task<ActionResult> CompletarActividad(string id)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;

                var actividad = await _service.GetActividadesByIdAsync(id);
                if (actividad == null)
                {
                    return NotFound("Actividad no encontrada");
                }

                // Verificar que el usuario esté asignado a la actividad
                if (actividad.Asignados == null || !actividad.Asignados.Contains(userEmail, StringComparer.OrdinalIgnoreCase))
                {
                    return Forbid("No tienes permiso para modificar esta actividad");
                }

                // Actualizar el estado de la actividad a "Completada"
                var updateDto = new ActividadesUpdateDto
                {
                    Titulo = actividad.Titulo,
                    Descripcion = actividad.Descripcion,
                    Fecha = actividad.Fecha,
                    Hora = actividad.Hora,
                    Duracion = actividad.Duracion,
                    Tipo = actividad.Tipo,
                    Modalidad = actividad.Modalidad,
                    Lugar = actividad.Lugar,
                    Asignados = actividad.Asignados,
                    Estado = "completada",
                    CreadoPor = actividad.CreadoPor,
                    CreadoEn = actividad.CreadoEn
                };

                await _service.UpdateActividadesAsync(id, updateDto);

                return Ok(new {
                    mensaje = "Actividad marcada como completada",
                    actividad_id = id,
                    completada_por = userName
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al completar actividad" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create([FromBody] ActividadesCreateDto dto)
        {
            await _service.InsertActividadesAsync(dto);
            return Ok(new { mensaje = "Actividad creada exitosamente" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Update(string id, ActividadesUpdateDto dto)
        {
            await _service.UpdateActividadesAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteActividadesAsync(id);
            return NoContent();
        }
    }

    public class AsignarActividadRequest
    {
        public List<string> UsuariosEmails { get; set; } = new List<string>();
    }
}
