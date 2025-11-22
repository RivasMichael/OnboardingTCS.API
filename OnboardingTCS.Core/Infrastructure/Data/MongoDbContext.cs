using MongoDB.Driver;
using OnboardingTCS.Core.Entities;
using Microsoft.Extensions.Configuration;

namespace OnboardingTCS.Core.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            
            // Configuraciones mejoradas para evitar timeouts
            settings.ConnectTimeout = TimeSpan.FromSeconds(30);
            settings.SocketTimeout = TimeSpan.FromSeconds(30);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
            settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(5);
            settings.MaxConnectionLifeTime = TimeSpan.FromMinutes(30);
            settings.MaxConnectionPoolSize = 100;
            settings.MinConnectionPoolSize = 5;
            
            // Configuración de retry para operaciones
            settings.RetryReads = true;
            settings.RetryWrites = true;
            
            var client = new MongoClient(settings);
            _database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
        }

        public IMongoCollection<Actividades> actividades => _database.GetCollection<Actividades>("actividades");
        public IMongoCollection<MensajesAutomaticos> mensajes_automaticos => _database.GetCollection<MensajesAutomaticos>("mensajes_automaticos");
        public IMongoCollection<Supervisor> Supervisores => _database.GetCollection<Supervisor>("supervisores");

        // 🔵 colecciones de dev_premaster
        public IMongoCollection<LikesCursos> likes_cursos => _database.GetCollection<LikesCursos>("likes_cursos");
        public IMongoCollection<MensajesEnviados> mensajes_enviados => _database.GetCollection<MensajesEnviados>("mensajes_enviados");

        // 🟢 colecciones de master
        public IMongoCollection<Documento> Documentos => _database.GetCollection<Documento>("documentos");
        public IMongoCollection<HistorialChat> HistorialChats => _database.GetCollection<HistorialChat>("historial_chat");
        public IMongoCollection<Curso> Cursos => _database.GetCollection<Curso>("cursos");
        public IMongoCollection<Usuario> Usuarios => _database.GetCollection<Usuario>("usuarios");
        
        public IMongoCollection<Actividades> Actividades => _database.GetCollection<Actividades>("actividades");
    }
}
