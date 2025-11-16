using MongoDB.Bson;
using MongoDB.Driver;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using OnboardingTCS.Core.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class CursoRepository : ICursoRepository
    {
        private readonly IMongoCollection<Curso> _cursos;
        private readonly ILogger<CursoRepository>? _logger;

        public CursoRepository(MongoDbContext context, ILogger<CursoRepository>? logger = null)
        {
            _cursos = context.Cursos;
            _logger = logger;
        }

        public async Task<IEnumerable<Curso>> GetAllAsync()
        {
            try
            {
                _logger?.LogInformation("Obteniendo todos los cursos");
                
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                return await _cursos.Find(_ => true).ToListAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener todos los cursos");
                throw new TimeoutException("Error de timeout al conectar con la base de datos", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener todos los cursos");
                throw new InvalidOperationException("Error al acceder a la base de datos", ex);
            }
        }

        public async Task<Curso> GetByIdAsync(string id)
        {
            try
            {
                _logger?.LogInformation("Obteniendo curso por ID: {CursoId}", id);
                
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede ser nulo o vacío", nameof(id));
                }

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var objectId = ObjectId.Parse(id);
                return await _cursos.Find(c => c.Id == objectId.ToString()).FirstOrDefaultAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener curso por ID: {CursoId}", id);
                throw new TimeoutException($"Error de timeout al buscar curso con ID: {id}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener curso por ID: {CursoId}", id);
                throw new InvalidOperationException($"Error al buscar curso con ID: {id}", ex);
            }
        }

        public async Task<IEnumerable<Curso>> GetByCategoriaAsync(string categoria)
        {
            try
            {
                _logger?.LogInformation("Obteniendo cursos por categoría: {Categoria}", categoria);
                
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var filter = Builders<Curso>.Filter.Eq(c => c.Categoria, categoria);
                return await _cursos.Find(filter).ToListAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener cursos por categoría: {Categoria}", categoria);
                throw new TimeoutException($"Error de timeout al buscar cursos de categoría: {categoria}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener cursos por categoría: {Categoria}", categoria);
                throw new InvalidOperationException($"Error al buscar cursos de categoría: {categoria}", ex);
            }
        }

        public async Task<IEnumerable<Curso>> GetByNivelAsync(string nivel)
        {
            try
            {
                _logger?.LogInformation("Obteniendo cursos por nivel: {Nivel}", nivel);
                
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var filter = Builders<Curso>.Filter.Eq(c => c.Nivel, nivel);
                return await _cursos.Find(filter).ToListAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener cursos por nivel: {Nivel}", nivel);
                throw new TimeoutException($"Error de timeout al buscar cursos de nivel: {nivel}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener cursos por nivel: {Nivel}", nivel);
                throw new InvalidOperationException($"Error al buscar cursos de nivel: {nivel}", ex);
            }
        }

        public async Task<IEnumerable<Curso>> GetByCategoriaAndNivelAsync(string categoria, string nivel)
        {
            try
            {
                _logger?.LogInformation("Obteniendo cursos por categoría: {Categoria} y nivel: {Nivel}", categoria, nivel);
                
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var builder = Builders<Curso>.Filter;
                var filters = new List<FilterDefinition<Curso>>();

                if (!string.IsNullOrEmpty(categoria) && categoria.ToLower() != "todas")
                {
                    filters.Add(builder.Eq(c => c.Categoria, categoria));
                }

                if (!string.IsNullOrEmpty(nivel) && nivel.ToLower() != "todos")
                {
                    filters.Add(builder.Eq(c => c.Nivel, nivel));
                }

                var combinedFilter = filters.Any() ? builder.And(filters) : builder.Empty;
                return await _cursos.Find(combinedFilter).ToListAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener cursos por categoría: {Categoria} y nivel: {Nivel}", categoria, nivel);
                throw new TimeoutException($"Error de timeout al buscar cursos de categoría: {categoria} y nivel: {nivel}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener cursos por categoría: {Categoria} y nivel: {Nivel}", categoria, nivel);
                throw new InvalidOperationException($"Error al buscar cursos de categoría: {categoria} y nivel: {nivel}", ex);
            }
        }

        public async Task<IEnumerable<string>> GetCategoriasAsync()
        {
            try
            {
                _logger?.LogInformation("Obteniendo todas las categorías");
                
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var categorias = await _cursos.Distinct<string>("categoria", FilterDefinition<Curso>.Empty).ToListAsync(cancellationTokenSource.Token);
                return categorias.Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener categorías");
                throw new TimeoutException("Error de timeout al obtener categorías", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener categorías");
                throw new InvalidOperationException("Error al obtener categorías", ex);
            }
        }

        public async Task<IEnumerable<string>> GetNivelesAsync()
        {
            try
            {
                _logger?.LogInformation("Obteniendo todos los niveles");
                
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var niveles = await _cursos.Distinct<string>("nivel", FilterDefinition<Curso>.Empty).ToListAsync(cancellationTokenSource.Token);
                return niveles.Where(n => !string.IsNullOrEmpty(n)).Distinct().OrderBy(n => n);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener niveles");
                throw new TimeoutException("Error de timeout al obtener niveles", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener niveles");
                throw new InvalidOperationException("Error al obtener niveles", ex);
            }
        }

        public async Task CreateAsync(Curso curso)
        {
            try
            {
                _logger?.LogInformation("Creando nuevo curso: {CursoTitulo}", curso?.Titulo);
                
                if (curso == null)
                {
                    throw new ArgumentNullException(nameof(curso));
                }

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                await _cursos.InsertOneAsync(curso, cancellationToken: cancellationTokenSource.Token);
                
                _logger?.LogInformation("Curso creado exitosamente con ID: {CursoId}", curso.Id);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al crear curso: {CursoTitulo}", curso?.Titulo);
                throw new TimeoutException("Error de timeout al crear curso", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al crear curso: {CursoTitulo}", curso?.Titulo);
                throw new InvalidOperationException("Error al crear curso en la base de datos", ex);
            }
        }

        public async Task UpdateAsync(string id, Curso curso)
        {
            try
            {
                _logger?.LogInformation("Actualizando curso: {CursoId}", id);
                
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede ser nulo o vacío", nameof(id));
                }
                
                if (curso == null)
                {
                    throw new ArgumentNullException(nameof(curso));
                }

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var objectId = ObjectId.Parse(id);
                var result = await _cursos.ReplaceOneAsync(c => c.Id == objectId.ToString(), curso, cancellationToken: cancellationTokenSource.Token);
                
                if (result.MatchedCount == 0)
                {
                    _logger?.LogWarning("No se encontró curso para actualizar con ID: {CursoId}", id);
                    throw new InvalidOperationException($"No se encontró curso con ID: {id}");
                }
                
                _logger?.LogInformation("Curso actualizado exitosamente: {CursoId}", id);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al actualizar curso: {CursoId}", id);
                throw new TimeoutException($"Error de timeout al actualizar curso con ID: {id}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al actualizar curso: {CursoId}", id);
                throw new InvalidOperationException($"Error al actualizar curso con ID: {id}", ex);
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                _logger?.LogInformation("Eliminando curso: {CursoId}", id);
                
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede ser nulo o vacío", nameof(id));
                }

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var objectId = ObjectId.Parse(id);
                var result = await _cursos.DeleteOneAsync(c => c.Id == objectId.ToString(), cancellationToken: cancellationTokenSource.Token);
                
                if (result.DeletedCount == 0)
                {
                    _logger?.LogWarning("No se encontró curso para eliminar con ID: {CursoId}", id);
                    throw new InvalidOperationException($"No se encontró curso con ID: {id}");
                }
                
                _logger?.LogInformation("Curso eliminado exitosamente: {CursoId}", id);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al eliminar curso: {CursoId}", id);
                throw new TimeoutException($"Error de timeout al eliminar curso con ID: {id}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al eliminar curso: {CursoId}", id);
                throw new InvalidOperationException($"Error al eliminar curso con ID: {id}", ex);
            }
        }
    }
}