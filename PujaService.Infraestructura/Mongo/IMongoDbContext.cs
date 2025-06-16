using MongoDB.Driver;
using PujaService.Infraestructura.Mongo.Documentos;

namespace PujaService.Infraestructura.Mongo
{
    public interface IMongoDbContext
    {
        IMongoCollection<PujaMongoDto> Pujas { get; }
    }
}