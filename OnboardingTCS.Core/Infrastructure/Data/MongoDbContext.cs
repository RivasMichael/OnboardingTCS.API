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

        public IMongoCollection<Supervisor> Supervisores => _database.GetCollection<Supervisor>("supervisores");
        public IMongoCollection<Documento> Documentos => _database.GetCollection<Documento>("documentos");
    }
}