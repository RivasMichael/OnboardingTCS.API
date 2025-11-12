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
            var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
            _database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
        }

        public IMongoCollection<Actividades> actividades => _database.GetCollection<Actividades>("actividades");
        public IMongoCollection<MensajesAutomaticos> mensajes_automaticos => _database.GetCollection<MensajesAutomaticos>("mensajes_automaticos");
        public IMongoCollection<Supervisor> Supervisores => _database.GetCollection<Supervisor>("supervisores");
    }
}