using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PujaService.Infraestructura.Mongo.Documentos
{
    public class PujaMongoDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string SubastaId { get; set; }
        public string UsuarioId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaHora { get; set; }
        public bool EsAutomatica { get; set; }
    }
}
