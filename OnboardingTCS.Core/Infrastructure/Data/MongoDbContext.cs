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
            var settings = MongoClientSettings.FromConnectionString(configuration["MongoDB:ConnectionString"]);
            settings.ConnectTimeout = TimeSpan.FromSeconds(10); // Aumentar el tiempo de espera
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
    }
}
