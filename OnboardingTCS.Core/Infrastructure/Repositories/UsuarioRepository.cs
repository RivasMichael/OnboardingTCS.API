using MongoDB.Bson;
using MongoDB.Driver;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Infrastructure.Data;
using OnboardingTCS.Core.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Text.RegularExpressions;

namespace OnboardingTCS.Core.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _usuarios;
        private readonly ILogger<UsuarioRepository>? _logger;

        public UsuarioRepository(MongoDbContext context, ILogger<UsuarioRepository>? logger = null)
        {
            _usuarios = context.Usuarios;
            _logger = logger;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            try
            {
                _logger?.LogInformation("Obteniendo todos los usuarios");
                
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                return await _usuarios.Find(_ => true).ToListAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener todos los usuarios");
                throw new TimeoutException("Error de timeout al conectar con la base de datos", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener todos los usuarios");
                throw new InvalidOperationException("Error al acceder a la base de datos", ex);
            }
        }

        public async Task<Usuario> GetByIdAsync(string id)
        {
            try
            {
                _logger?.LogInformation("Obteniendo usuario por ID: {UserId}", id);
                
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede ser nulo o vacío", nameof(id));
                }

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                return await _usuarios.Find(u => u.Id == id).FirstOrDefaultAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener usuario por ID: {UserId}", id);
                throw new TimeoutException($"Error de timeout al buscar usuario con ID: {id}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener usuario por ID: {UserId}", id);
                throw new InvalidOperationException($"Error al buscar usuario con ID: {id}", ex);
            }
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            try
            {
                _logger?.LogInformation("Obteniendo usuario por email: {Email}", email);
                
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("El email no puede ser nulo o vacío", nameof(email));
                }

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                
                // Use case-insensitive comparison for MongoDB
                var filter = Builders<Usuario>.Filter.Regex(u => u.Correo, new BsonRegularExpression($"^{Regex.Escape(email)}$", "i"));
                return await _usuarios.Find(filter).FirstOrDefaultAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al obtener usuario por email: {Email}", email);
                throw new TimeoutException($"Error de timeout al buscar usuario con email: {email}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al obtener usuario por email: {Email}", email);
                throw new InvalidOperationException($"Error al buscar usuario con email: {email}", ex);
            }
        }

        public async Task CreateAsync(Usuario usuario)
        {
            try
            {
                _logger?.LogInformation("Creando nuevo usuario: {UserEmail}", usuario?.Correo);
                
                if (usuario == null)
                {
                    throw new ArgumentNullException(nameof(usuario));
                }

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                await _usuarios.InsertOneAsync(usuario, cancellationToken: cancellationTokenSource.Token);
                
                _logger?.LogInformation("Usuario creado exitosamente con ID: {UserId}", usuario.Id);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al crear usuario: {UserEmail}", usuario?.Correo);
                throw new TimeoutException("Error de timeout al crear usuario", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al crear usuario: {UserEmail}", usuario?.Correo);
                throw new InvalidOperationException("Error al crear usuario en la base de datos", ex);
            }
        }

        public async Task UpdateAsync(string id, Usuario usuario)
        {
            try
            {
                _logger?.LogInformation("Actualizando usuario: {UserId}", id);
                
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede ser nulo o vacío", nameof(id));
                }
                
                if (usuario == null)
                {
                    throw new ArgumentNullException(nameof(usuario));
                }

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var result = await _usuarios.ReplaceOneAsync(u => u.Id == id, usuario, cancellationToken: cancellationTokenSource.Token);
                
                if (result.MatchedCount == 0)
                {
                    _logger?.LogWarning("No se encontró usuario para actualizar con ID: {UserId}", id);
                    throw new InvalidOperationException($"No se encontró usuario con ID: {id}");
                }
                
                _logger?.LogInformation("Usuario actualizado exitosamente: {UserId}", id);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al actualizar usuario: {UserId}", id);
                throw new TimeoutException($"Error de timeout al actualizar usuario con ID: {id}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al actualizar usuario: {UserId}", id);
                throw new InvalidOperationException($"Error al actualizar usuario con ID: {id}", ex);
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                _logger?.LogInformation("Eliminando usuario: {UserId}", id);
                
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede ser nulo o vacío", nameof(id));
                }

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var result = await _usuarios.DeleteOneAsync(u => u.Id == id, cancellationToken: cancellationTokenSource.Token);
                
                if (result.DeletedCount == 0)
                {
                    _logger?.LogWarning("No se encontró usuario para eliminar con ID: {UserId}", id);
                    throw new InvalidOperationException($"No se encontró usuario con ID: {id}");
                }
                
                _logger?.LogInformation("Usuario eliminado exitosamente: {UserId}", id);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout al eliminar usuario: {UserId}", id);
                throw new TimeoutException($"Error de timeout al eliminar usuario con ID: {id}", ex);
            }
            catch (MongoException ex)
            {
                _logger?.LogError(ex, "Error de MongoDB al eliminar usuario: {UserId}", id);
                throw new InvalidOperationException($"Error al eliminar usuario con ID: {id}", ex);
            }
        }
    }
}