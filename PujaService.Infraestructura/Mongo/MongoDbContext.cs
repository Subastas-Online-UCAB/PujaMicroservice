using Microsoft.Extensions.Configuration;
using PujaService.Infraestructura.Mongo.Documentos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PujaService.Infraestructura.Mongo
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            

            var client = new MongoClient(configuration["MongoSettings:ConnectionString"]);
            _database = client.GetDatabase(configuration["MongoSettings:DatabaseName"]);
        }

        public IMongoCollection<PujaMongoDto> Pujas => _database.GetCollection<PujaMongoDto>("Pujas");
    }
}
