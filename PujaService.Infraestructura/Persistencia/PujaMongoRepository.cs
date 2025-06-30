using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Interfaces;
using PujaService.Infraestructura.Mongo;
using PujaService.Infraestructura.Mongo.Documentos;

namespace PujaService.Infraestructura.Repositorios
{
    public class PujaMongoRepository : IPujaMongoRepository
    {
        private readonly IMongoCollection<PujaMongoDto> _pujas;

        public PujaMongoRepository(IMongoDbContext context)
        {
            _pujas = context.Pujas;
        }

        public async Task<List<Puja>> ObtenerPorSubastaAsync(string subastaId)
        {
            var filter = Builders<PujaMongoDto>.Filter.Eq(p => p.SubastaId, subastaId);

            var result = await _pujas.Find(filter)
                .SortByDescending(p => p.FechaHora)
                .ToListAsync();

            return result.Select(p => new Puja
            {
                Id = p.Id,
                SubastaId = p.SubastaId,
                UsuarioId = p.UsuarioId,
                Monto = p.Monto,
                FechaHora = p.FechaHora
            }).ToList();
        }
    }

}